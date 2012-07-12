using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using System.Text;
using Microsoft.Devices.Sensors;
using robobot_winphone.ViewModel;

namespace robobot_winphone.Model.SensorHandler
{
    public class ACGSensorHandler : AbstractSensorHandler
    {
        private Gyroscope gyroscope;
        private Accelerometer accelerometer;
        private Compass compass;
        private ComplementaryFilter filter;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        private DateTime startTime;
        private int fixCompassData;
        private bool isFixComassDataDetected;

        public ACGSensorHandler(double frequency, ISensorView sensorView)
        {
            if (Gyroscope.IsSupported && Accelerometer.IsSupported && Compass.IsSupported)
            {
                filter = new ComplementaryFilter((float)frequency);
                gyroscope = new Gyroscope();
                accelerometer = new Accelerometer();
                compass = new Compass();
                this.sensorView = sensorView;

                accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                compass.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                gyroscope.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

                accelerometer.CurrentValueChanged += accelerometer_CurrentValueChanged;

                timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(frequency) };
                timer.Tick += TimerTick;
            }
            else
            {
                LogManager.Log("Some sensor is not supported on this device");
            }
        }

        public override void Start()
        {
            compass.Start();
            gyroscope.Start();
            accelerometer.Start();
            timer.Start();
            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            compass.Stop();
            gyroscope.Stop();
            accelerometer.Stop();
            timer.Stop();
        }

        private void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            try
            {
                filter.UpdateCummulativeValue(gyroscope.CurrentValue.RotationRate, e.SensorReading.Acceleration,
                    compass.CurrentValue.MagnetometerReading, e.SensorReading.Timestamp);
            }
            catch (Exception)
            {
                LogManager.Log("Sensors reading error");
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (isFixComassDataDetected)
            {
                sensorView.ProcessSensorData(CalculateTurn((int)compass.CurrentValue.TrueHeading),
                    CalculateSpeed((double)(-filter.CummulativeValue.X)));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                fixCompassData = (int)compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }

        private const int MaxValue = 100;

        private int CalculateSpeed(double value)
        {
            int outPutValue = (int)(value * MaxValue);
            if (outPutValue >= MaxValue)
            {
                return MaxValue;
            }
            if (outPutValue <= -MaxValue)
            {
                return -MaxValue;
            }
            return outPutValue;
        }

        private int CalculateTurn(int value)
        {
            int outPutValue = (value + (180 - fixCompassData)) % 360;
            if (outPutValue < 0)
            {
                outPutValue += 360;
            }
            outPutValue -= 180;
            outPutValue *= 2;
            if (outPutValue >= MaxValue)
            {
                return MaxValue;
            }
            if (outPutValue <= -MaxValue)
            {
                return -MaxValue;
            }
            return outPutValue;
        }
    }
}
