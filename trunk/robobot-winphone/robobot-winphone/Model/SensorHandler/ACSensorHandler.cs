using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ACSensorHandler : AbstractTurnCompassSensorHandler
    {
        private DateTime startTime;

        public ACSensorHandler(double frequency, ISensorView sensorView)
        {
            if (Accelerometer.IsSupported && Compass.IsSupported)
            {
                Accelerometer = new Accelerometer();
                Compass = new Compass();

                TurnSmoothValueManager = new SmoothValueManager();
                SpeedSmoothValueManager = new SmoothValueManager();

                this.SensorView = sensorView;

                Compass.Calibrate += CompassCalibrate;

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
            Compass.Start();
            Accelerometer.Start();
            Timer.Start();
            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            Compass.Stop();
            Accelerometer.Stop();
            Timer.Stop();
        }

        private const double CompassValueFactor = 3;

        private void TimerTick(object sender, EventArgs e)
        {
            if (isFixComassDataDetected)
            {
                SensorView.ProcessSensorData(CalculateTurn(Compass.CurrentValue.TrueHeading, CompassValueFactor),
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
