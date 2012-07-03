using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using robobot_winphone.Model;
using System.ComponentModel;

namespace robobot_winphone.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private Gyroscope gyroscope;
        private Accelerometer accelerometer;
        private Compass compass;
        private ComplementaryFilter filter;

        private double xLineX = 240;
        private double xLineY = 0;
        private double yLineX = 240;
        private double yLineY = 0;
        private double zLineX = 240;
        private double zLineY = 0;

        public double XLineX
        {
            get
            {
                return this.xLineX;
            }
        }
        public double XLineY
        {
            get
            {
                return this.xLineY;
            }
        }
        public double YLineX
{
            get
            {
                return this.yLineX;
            }
        }
        public double YLineY
        {
            get
            {
                return this.yLineY;
            }
        }
        public double ZLineX
        {
            get
            {
                return this.zLineX;
            }
        }
        public double ZLineY
        {
            get
            {
                return this.zLineY;
            }
        }

        public MainPageViewModel()
        {
            if (Gyroscope.IsSupported && Accelerometer.IsSupported && Compass.IsSupported)
            {
                filter = new ComplementaryFilter((float)0.01);
                gyroscope = new Gyroscope();
                accelerometer = new Accelerometer();
                compass = new Compass();

                accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1);
                compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1);
                gyroscope.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1);

                gyroscope.CurrentValueChanged += gyroscope_CurrentValueChanged;

                accelerometer.Start();
                compass.Start();
                gyroscope.Start();

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(10);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                xLineX = 240 - 100 * Math.Sin((double)filter.CummulativeValue.X);
                xLineY = 100 - 100 * Math.Cos((double)filter.CummulativeValue.X);
                yLineX = 240 - 100 * Math.Sin((double)filter.CummulativeValue.Y);
                yLineY = 100 - 100 * Math.Cos((double)filter.CummulativeValue.Y);
                zLineX = 240 - 100 * Math.Sin((double)filter.CummulativeValue.Z);
                zLineY = 100 - 100 * Math.Cos((double)filter.CummulativeValue.Z);

                NotifyPropertyChanged("XLineX");
                NotifyPropertyChanged("XLineY");
                NotifyPropertyChanged("YLineX");
                NotifyPropertyChanged("YLineY");
                NotifyPropertyChanged("ZLineX");
                NotifyPropertyChanged("ZLineY");
            }
            catch (Exception)
            {
                LogManager.Log("Update cummulative value error");
            }
        }

        void gyroscope_CurrentValueChanged(object sender, SensorReadingEventArgs<GyroscopeReading> e)
        {
            try
            {
                filter.UpdateCummulativeValue(e.SensorReading.RotationRate, accelerometer.CurrentValue.Acceleration,
                    compass.CurrentValue.MagnetometerReading, e.SensorReading.Timestamp);
            }
            catch (Exception)
            {
                LogManager.Log("Sensors reading error");
            }
        }
    }
}
