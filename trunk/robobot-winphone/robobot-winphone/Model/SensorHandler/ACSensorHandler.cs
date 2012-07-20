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
                accelerometer = new Accelerometer();
                compass = new Compass();

                turnSmoothValueManager = new SmoothValueManager();
                speedSmoothValueManager = new SmoothValueManager();

                this.sensorView = sensorView;

                compass.Calibrate += CompassCalibrate;

                accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);
                compass.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

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
            accelerometer.Start();
            timer.Start();
            startTime = DateTime.Now;
            isFixComassDataDetected = false;
        }

        public override void Stop()
        {
            compass.Stop();
            accelerometer.Stop();
            timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (isFixComassDataDetected)
            {
                sensorView.ProcessSensorData(CalculateTurn(compass.CurrentValue.TrueHeading),
                                CalculateSpeed(-accelerometer.CurrentValue.Acceleration.X * 180));
            }
            else if ((DateTime.Now - startTime).Seconds > 1)
            {
                fixCompassData = compass.CurrentValue.TrueHeading;
                isFixComassDataDetected = true;
            }
        }
    }
}
