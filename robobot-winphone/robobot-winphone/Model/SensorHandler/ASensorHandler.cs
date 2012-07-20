using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ASensorHandler : AbstractSensorHandler
    {
        public ASensorHandler(double frequency, ISensorExecutor sensorView)
        {
            if (Accelerometer.IsSupported)
            {
                Accelerometer = new Accelerometer
                                    {
                                        TimeBetweenUpdates = TimeSpan.FromMilliseconds(frequency)
                                    };

                this.SensorView = sensorView;

                TurnSmoothValueManager = new SmoothValueManager();
                SpeedSmoothValueManager = new SmoothValueManager();

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
            Accelerometer.Start();
            Timer.Start();
        }

        public override void Stop()
        {
            Accelerometer.Stop();
            Timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            SensorView.ProcessSensorData(CalculateTurn(-Accelerometer.CurrentValue.Acceleration.Y * 180),
                            CalculateSpeed(-Accelerometer.CurrentValue.Acceleration.X * 180));
        }
    }
}
