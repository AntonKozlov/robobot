using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ASensorHandler : AbstractSensorHandler
    {
        public ASensorHandler(double frequency, ISensorView sensorView)
        {
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer
                                    {
                                        TimeBetweenUpdates = TimeSpan.FromMilliseconds(frequency)
                                    };

                this.sensorView = sensorView;

                turnSmoothValueManager = new SmoothValueManager();
                speedSmoothValueManager = new SmoothValueManager();

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
            accelerometer.Start();
            timer.Start();
        }

        public override void Stop()
        {
            accelerometer.Stop();
            timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            sensorView.ProcessSensorData(CalculateTurn(-accelerometer.CurrentValue.Acceleration.Y * 180),
                            CalculateSpeed(-accelerometer.CurrentValue.Acceleration.X * 180));
        }
    }
}
