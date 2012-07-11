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
    public class ACSensorHandler : ISensorHandler
    {
        private Accelerometer accelerometer;
        private Compass compass;
        private ISensorView sensorView;
        private DispatcherTimer timer;

        public ACSensorHandler(double frequency, ISensorView sensorView)
        {
            if (Accelerometer.IsSupported && Compass.IsSupported)
            {
                accelerometer = new Accelerometer();
                compass = new Compass();

                this.sensorView = sensorView;

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

        public void Start()
        {
            compass.Start();
            accelerometer.Start();
            timer.Start();
        }

        public void Stop()
        {
            compass.Stop();
            accelerometer.Stop();
            timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            sensorView.ProcessSensorData(CalculateTurn((int)compass.CurrentValue.TrueHeading),
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

        private int CalculateTurn(int value)
        {
            int outPutValue = (value + (180 - 100)) % 360;
            if (outPutValue < 0)
            {
                outPutValue += 360;
            }
            outPutValue -= 180;
            outPutValue *= 2;
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
