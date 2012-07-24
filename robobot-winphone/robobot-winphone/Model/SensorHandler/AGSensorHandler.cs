﻿using System;
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

                this.SensorExecutor = sensorView;

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
            Compass.Start();
            gyroscope.Start();
            Accelerometer.Start();
            Timer.Start();
            startTime = DateTime.Now;
        }

        public override void Stop()
        {
            Compass.Stop();
            gyroscope.Stop();
            Accelerometer.Stop();
            Timer.Stop();
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
            if ((DateTime.Now - startTime).Seconds > 1)
            {
                SensorExecutor.ProcessSensorData(CalculateSpeed(filter.CummulativeValue.Y * 60, GyroscopeValueFactor),
                                CalculateTurn(-filter.CummulativeValue.X * 60, GyroscopeValueFactor));
            }
        }
    }
}
