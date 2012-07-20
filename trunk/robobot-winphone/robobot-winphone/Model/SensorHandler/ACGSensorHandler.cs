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

        public ACGSensorHandler(double frequency, ISensorView sensorView)
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
            try
            {
                compass.Start();
                gyroscope.Start();
                accelerometer.Start();
                timer.Start();
            }
            catch (Exception)
            {
                LogManager.Log("Some sensor or compass isn't initializated");
            }

            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            try
            {
                compass.Stop();
                gyroscope.Stop();
                accelerometer.Stop();
                timer.Stop();
            }
            catch (Exception)
            {
                LogManager.Log("Some sensor or compass trouble");
            }
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
            if (isFixComassDataDetected)
            {
                sensorView.ProcessSensorData(CalculateTurn(compass.CurrentValue.TrueHeading),
                    CalculateSpeed(-filter.CummulativeValue.X * 60));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                fixCompassData = compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }
    }
}
