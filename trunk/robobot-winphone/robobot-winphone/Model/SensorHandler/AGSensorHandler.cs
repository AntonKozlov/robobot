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
    public class AGSensorHandler
    {
        private Gyroscope gyroscope;
        private Accelerometer accelerometer;
        private Compass compass;
        private ComplementaryFilter filter;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        public AGSensorHandler(double frequency, ISensorView sensorView)
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
            }
            catch (Exception)
            {
                LogManager.Log("Sensors reading error");
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            sensorView.ProcessSensorData(CalculateValue((double)filter.CummulativeValue.Y),
                            CalculateValue((double)(-filter.CummulativeValue.X)));
        }

        private const int MaxValue = 100;

        private int CalculateValue(double value)
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
    }
}
