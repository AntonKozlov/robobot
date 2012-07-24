using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ACGSensorHandler : AbstractTurnCompassSensorHandler
    {
        private Gyroscope gyroscope;

        private ComplementaryFilter filter;

        private DateTime startTime;

        public ACGSensorHandler(double frequency, ISensorExecutor sensorView)
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

                Compass.Calibrate += CompassCalibrate;

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
            try
            {
                if (SensorExecutor != null && filter != null)
                {
                    Compass.Start();
                    gyroscope.Start();
                    Accelerometer.Start();
                    Timer.Start();
                }
                else
                {
                    LogManager.Log("SensorExecutor of Filter isn't initializated");
                }
            }
            catch (Exception)
            {
                LogManager.Log("Start sensor or timer trouble");
            }

            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            try
            {
                Compass.Stop();
                gyroscope.Stop();
                Accelerometer.Stop();
                Timer.Stop();
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
        private const double CompassValueFactor = 3;

        private void TimerTick(object sender, EventArgs e)
        {
            if (isFixComassDataDetected)
            {
                SensorExecutor.ProcessSensorData(CalculateTurn(Compass.CurrentValue.TrueHeading, CompassValueFactor),
                    CalculateSpeed(-filter.CummulativeValue.X * 60, GyroscopeValueFactor));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                fixCompassData = Compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }
    }
}
