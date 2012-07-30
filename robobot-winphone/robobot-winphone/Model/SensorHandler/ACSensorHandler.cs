using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ACSensorHandler : AbstractTurnCompassSensorHandler
    {
        private DateTime startTime;

        public ACSensorHandler(double frequency, ISensorExecutor sensorView)
        {
            if (Accelerometer.IsSupported && Compass.IsSupported)
            {
                Accelerometer = new Accelerometer();
                Compass = new Compass();

                TurnSmoothValueManager = new SmoothValueManager();
                SpeedSmoothValueManager = new SmoothValueManager();

                SensorExecutor = sensorView;

                Accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                Compass.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

                Timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(frequency) };
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
                if (SensorExecutor != null)
                {
                    Compass.Start();
                    Accelerometer.Start();
                    Timer.Start();
                    TurnSmoothValueManager.Start();
                    SpeedSmoothValueManager.Start();
                }
                else
                {
                    LogManager.Log("SensorExecutor isn't initializated");
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

        private const double CompassValueFactor = 3;

        private void TimerTick(object sender, EventArgs e)
        {
            if (Compass.CurrentValue.HeadingAccuracy > 20)
            {
                CompassCalibrate();
            }
            else if (isFixComassDataDetected)
            {
                SensorExecutor.ProcessSensorData(
                    CalculateTurn(Compass.CurrentValue.TrueHeading, CompassValueFactor),
                    CalculateSpeed(-Accelerometer.CurrentValue.Acceleration.X * 180));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                fixCompassData = Compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }
    }
}
