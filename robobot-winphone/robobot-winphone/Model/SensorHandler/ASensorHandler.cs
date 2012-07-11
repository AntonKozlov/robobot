using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using System.Text;
using Microsoft.Devices.Sensors;
using robobot_winphone.ViewModel;

namespace robobot_winphone.Model.SensorHandler
{
    public class ASensorHandler : ISensorHandler
    {
        private Accelerometer accelerometer;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        public ASensorHandler(double frequency, ISensorView sensorView)
        {
            if (Accelerometer.IsSupported)
            {
                accelerometer = new Accelerometer();
                this.sensorView = sensorView;

                accelerometer.TimeBetweenUpdates = TimeSpan.FromSeconds(frequency);

                timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(frequency) };
                timer.Tick += TimerTick;
            }
            else
            {
                LogManager.Log("Some sensor is not supported on this device");
            }            
        }

        public void Start()
        {
            accelerometer.Start();
            timer.Start();
        }

        public void Stop()
        {
            accelerometer.Stop();
            timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            sensorView.ProcessSensorData(CalculateValue((double)-accelerometer.CurrentValue.Acceleration.Y),
                            CalculateValue((double)(-accelerometer.CurrentValue.Acceleration.X)));
        }

        private const int MaxValue = 100;

        private int CalculateValue(double value)
        {
            int outPutValue = (int)(value * MaxValue * 1.8);
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
