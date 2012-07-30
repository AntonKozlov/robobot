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

        public AGSensorHandler(double frequency, ISensorExecutor sensorView)
        {
            if (Gyroscope.IsSupported && Accelerometer.IsSupported && Compass.IsSupported)
            {
                filter = new ComplementaryFilter((float)frequency);

                gyroscope = new Gyroscope
                {
                    TimeBetweenUpdates = TimeSpan.FromSeconds(frequency)
                };
                Accelerometer = new Accelerometer
                {
                    TimeBetweenUpdates = TimeSpan.FromSeconds(frequency)
                };
                Compass = new Compass
                {
                    TimeBetweenUpdates = TimeSpan.FromSeconds(frequency)
                };

                TurnSmoothValueManager = new SmoothValueManager();
                SpeedSmoothValueManager = new SmoothValueManager();

                SensorExecutor = sensorView;

                Accelerometer.CurrentValueChanged += AccelerometerCurrentValueChanged;

                Timer = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(frequency)
                            };
                Timer.Tick += TimerTick;
            }
            else
            {
                LogManager.Log("Some sensor is not supported on this device");
            }
        }

        public override void Start()
        {
            if (SensorExecutor != null && filter != null)
            {
                Compass.Start();
                gyroscope.Start();
                Accelerometer.Start();
                Timer.Start();
                TurnSmoothValueManager.Start();
                SpeedSmoothValueManager.Start();
            }
            else
            {
                LogManager.Log("SensorExecutor of Filter isn't initializated");
            }

            startTime = DateTime.Now;
        }

        public override void Stop()
        {
            try
            {
                Compass.Stop();
                gyroscope.Stop();
                Accelerometer.Stop();
                Timer.Stop();
                TurnSmoothValueManager.Stop();
                SpeedSmoothValueManager.Stop();
            }
            catch (Exception)
            {
                LogManager.Log("Stop sensor or timer trouble");
            }
        }

        private void AccelerometerCurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            try
            {
                filter.UpdateCummulativeValue(gyroscope.CurrentValue.RotationRate, e.SensorReading.Acceleration,
                                                  Compass.CurrentValue.MagnetometerReading, e.SensorReading.Timestamp);
            }
            catch (Exception)
            {
                LogManager.Log("Sensors reading error");
            }
        }

        private const double GyroscopeValueFactor = 3.2;

        private void TimerTick(object sender, EventArgs e)
        {
            if (Compass.CurrentValue.HeadingAccuracy > 20)
            {
                CompassCalibrate();
            }
            else if ((DateTime.Now - startTime).Seconds <= 1)
            {
                return;
            }
            SensorExecutor.ProcessSensorData(CalculateSpeed(filter.CummulativeValue.Y * 60, GyroscopeValueFactor),
                                             CalculateTurn(-filter.CummulativeValue.X * 60, GyroscopeValueFactor));

        }
    }
}
