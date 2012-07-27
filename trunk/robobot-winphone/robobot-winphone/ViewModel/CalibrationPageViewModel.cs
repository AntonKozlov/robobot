using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using robobot_winphone.Model;

namespace robobot_winphone.ViewModel
{
    public class CalibrationPageViewModel : BaseViewModel
    {
        private Compass compass;
        private DispatcherTimer timer;
        private double headingAccuracy;

        public double HeadingAccuracy
        {
            get
            {
                return headingAccuracy;
            }
            set
            {
                headingAccuracy = value;
                NotifyPropertyChanged("HeadingAccuracy");
            }
        }

        public CalibrationPageViewModel()
        {
            if(Compass.IsSupported)
            {
                compass = new Compass
                              {
                                  TimeBetweenUpdates = TimeSpan.FromMilliseconds(10)
                              };

                timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(10)
                };
                timer.Tick += TimerTick;

                try
                {
                    compass.Start();
                    timer.Start();
                }
                catch (Exception)
                {
                    LogManager.Log("Start compass or timer trouble");
                }
            }
        }

        public void StopTimerAndCompass()
        {
            try
            {
                compass.Stop();
                timer.Stop();
            }
            catch (Exception)
            {
                LogManager.Log("Stop compass or timer trouble");
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            HeadingAccuracy = compass.CurrentValue.HeadingAccuracy;
        }
    }
}
