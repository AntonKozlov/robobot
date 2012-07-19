using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model.Utils;

namespace robobot_winphone.Model.SensorHandler
{
    public class ASensorHandler : AbstractSensorHandler
    {
        private Accelerometer accelerometer;
        private ISensorView sensorView;
        private DispatcherTimer timer;
        private SmoothValueManager speedSmoothValueManager;
        private SmoothValueManager turnSmoothValueManager;

        public ASensorHandler(double frequency, ISensorView sensorView)
        {
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer();
                this.sensorView = sensorView;
                speedSmoothValueManager = new SmoothValueManager();
                turnSmoothValueManager = new SmoothValueManager();

                accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

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
            sensorView.ProcessSensorData(CalculateValue(-accelerometer.CurrentValue.Acceleration.Y, turnSmoothValueManager),
                            CalculateValue(-accelerometer.CurrentValue.Acceleration.X, speedSmoothValueManager));
        }

        private const int MaxValue = 100;

        private int CalculateValue(double value, SmoothValueManager manager)
        {
            var outPutValue = (int)(value * MaxValue * 1.8);

            outPutValue = (int)manager.GetSmoothValue(outPutValue);

            if (outPutValue >= MaxValue)
            {
                return MaxValue;
            }

            if (outPutValue <= -MaxValue)
            {
                return -MaxValue;
            }

            return outPutValue;
        }
    }
}
