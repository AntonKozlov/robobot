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
    public class ACGSensorHandler : ISensorHandler
    {
        private Gyroscope gyroscope;
        private Accelerometer accelerometer;
        private Compass compass;
        private ComplementaryFilter filter;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        private int fixCompassData;

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

        public void Start()
        {
            compass.Start();
            gyroscope.Start();
            accelerometer.Start();
            timer.Start();
        }

        public void Stop()
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
                fixCompassData = (int)compass.CurrentValue.TrueHeading;
            }
            catch (Exception)
            {
                LogManager.Log("Sensors reading error");
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            int temporaryRotationValue = (int)(filter.CummulativeValue.Z * 100);
            sensorView.ProcessSensorData(CalculateTurn(temporaryRotationValue), 
                CalculateSpeed((double)(-filter.CummulativeValue.X)));
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
            if (value > 300) value = 300;
            if (value < -300) value = -299;
            int outPutValue = (value + 300 - 170) % 600;
            if (outPutValue < 0)
            {
                outPutValue += 600;
            }
            outPutValue -= 300;
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
