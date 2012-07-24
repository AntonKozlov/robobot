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

                SensorExecutor = sensorView;

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
            try
            {
                if (SensorExecutor != null)
                {
                    Accelerometer.Start();
                    Timer.Start();
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
        }

        public override void Stop()
        {
            try
            {
                Accelerometer.Stop();
                Timer.Stop();
            }
            catch (Exception)
            {
                LogManager.Log("Stop sensor or timer trouble");
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            SensorExecutor.ProcessSensorData(CalculateTurn(-Accelerometer.CurrentValue.Acceleration.Y * 180),
                            CalculateSpeed(-Accelerometer.CurrentValue.Acceleration.X * 180));
        }
    }
}
