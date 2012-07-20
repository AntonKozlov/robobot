using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class AGSensorHandler : AbstractCompassSensorHandler
    {
        private Gyroscope gyroscope;
        private ComplementaryFilter filter;
        private DateTime startTime;

        public AGSensorHandler(double frequency, ISensorView sensorView)
        {
            if (Gyroscope.IsSupported && Accelerometer.IsSupported && Compass.IsSupported)
            {
                filter = new ComplementaryFilter((float)frequency);

                gyroscope = new Gyroscope
                {
                    TimeBetweenUpdates = TimeSpan.FromSeconds(frequency)
                };
                accelerometer = new Accelerometer
                {
                    TimeBetweenUpdates = TimeSpan.FromSeconds(frequency)
                };
                compass = new Compass
                {
                    TimeBetweenUpdates = TimeSpan.FromSeconds(frequency)
                };

                turnSmoothValueManager = new SmoothValueManager();
                speedSmoothValueManager = new SmoothValueManager();

                this.sensorView = sensorView;

                compass.Calibrate += CompassCalibrate;
               
                accelerometer.CurrentValueChanged += AccelerometerCurrentValueChanged;

                timer = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(frequency)
                            };
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
        }

        public override void Stop()
        {
            compass.Stop();
            gyroscope.Stop();
            accelerometer.Stop();
            timer.Stop();
        }

        private void AccelerometerCurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
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
            if ((DateTime.Now - startTime).Seconds > 1)
            {
                sensorView.ProcessSensorData(CalculateSpeed(filter.CummulativeValue.Y * 60),
                                CalculateTurn(-filter.CummulativeValue.X * 60));
            }
        }
    }
}
