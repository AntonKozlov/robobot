using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using robobot_winphone.Model;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Text;

namespace robobot_winphone.ViewModel
{
    public enum ConnectionStatus : int
    {
        Connected = 0,
        Disconnected = 1
    }

    public enum SendingStatus : int
    {
        StartSending = 0,
        StopSending = 1
    }

    public class MainPageViewModel : BaseViewModel
    {
        private Gyroscope gyroscope;
        private Accelerometer accelerometer;
        private Compass compass;
        private ComplementaryFilter filter;
        private ConnectionStatus connectionStatus;
        private SendingStatus sendingStatus;

        public double XLineX { get; private set; }
        public double XLineY { get; private set; }
        public double YLineX { get; private set; }
        public double YLineY { get; private set; }
        public double ZLineX { get; private set; }
        public double ZLineY { get; private set; }

        public ICommand DisconnectCommand { get; private set; }
        public ICommand SendingCommand { get; private set; }

        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return connectionStatus;
            }
            set
            {
                if (value != connectionStatus)
                {
                    connectionStatus = value;
                    NotifyPropertyChanged("ConnectionStatus");
                }
            }
        }

        public SendingStatus SendingStatus
        {
            get
            {
                return sendingStatus;
            }
            set
            {
                if (value != sendingStatus)
                {
                    sendingStatus = value;
                    NotifyPropertyChanged("SendingStatus");
                }
            }
        }

        public MainPageViewModel()
        {
            XLineX = 240;
            XLineY = 0;
            YLineX = 240;
            YLineY = 0;
            ZLineX = 240;
            ZLineY = 0;
            DisconnectCommand = new ButtonCommand(Disconnect);
            SendingCommand = new ButtonCommand(SendOrStopSend);

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
                XLineX = 240 - 100 * Math.Sin((double)filter.CummulativeValue.X);
                XLineY = 100 - 100 * Math.Cos((double)filter.CummulativeValue.X);
                YLineX = 240 - 100 * Math.Sin((double)filter.CummulativeValue.Y);
                YLineY = 100 - 100 * Math.Cos((double)filter.CummulativeValue.Y);
                ZLineX = 240 - 100 * Math.Sin((double)filter.CummulativeValue.Z);
                ZLineY = 100 - 100 * Math.Cos((double)filter.CummulativeValue.Z);

                NotifyPropertyChanged("XLineX");
                NotifyPropertyChanged("XLineY");
                NotifyPropertyChanged("YLineX");
                NotifyPropertyChanged("YLineY");
                NotifyPropertyChanged("ZLineX");
                NotifyPropertyChanged("ZLineY");

                if (SocketClient.Instance.IsConnected())
                {
                    ConnectionStatus = ViewModel.ConnectionStatus.Connected;
                    if (SendingStatus == ViewModel.SendingStatus.StopSending)
                    {
                        SendMessage(String.Format("Speed: {0}, Turn: {1}", CalculateSpeed((double)filter.CummulativeValue.X),
                            CalculateTurn((double)filter.CummulativeValue.Y)));
                    }
                }
                else
                {
                    ConnectionStatus = ViewModel.ConnectionStatus.Disconnected;
                    SendingStatus = ViewModel.SendingStatus.StartSending;
                }
            }
            catch (Exception)
            {
                LogManager.Log("Update cummulative value error");
            }
        }

        #region SensorDataCalculationTest
        private int CalculateSpeed(double value)
        {

            int speed = (int)(value * 60);
            if (speed >= 90)
            {
                return 90;
            }
            if (speed <= -90)
            {
                return -90;
            }
            return speed;
        }

        private int CalculateTurn(double value)
        {
            int turn = (int)(value * 120);
            if (turn >= 100)
            {
                return 100;
            }
            if (turn <= -100)
            {
                return -100;
            }
            return turn;
        }
        #endregion

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

        private void Disconnect(object p)
        {
            SocketClient.Instance.Disconnect();
        }

        private void SendMessage(string message)
        {
            SocketClient.Instance.SendData(Encoding.UTF8.GetBytes(String.Format("{0}\n", message)));
        }
        private void SendOrStopSend(object p)
        {
            if (SendingStatus == ViewModel.SendingStatus.StartSending)
            {
                SendingStatus = ViewModel.SendingStatus.StopSending;
            }
            else
            {
                SendingStatus = ViewModel.SendingStatus.StartSending;
            }
        }
    }
}
