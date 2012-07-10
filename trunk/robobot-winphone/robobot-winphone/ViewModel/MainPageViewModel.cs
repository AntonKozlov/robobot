using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Text;

using robobot_winphone.Model;
using robobot_winphone.Model.SensorHandler;

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

    public class MainPageViewModel : BaseViewModel, ISensorView
    {
        private ConnectionStatus connectionStatus;
        private SendingStatus sendingStatus;
        private ISensorHandler handler;

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

            ConnectionStatus = ViewModel.ConnectionStatus.Disconnected;
            sendingStatus = ViewModel.SendingStatus.StartSending;

            handler = SensorHandlerManager.GetSensorHandler(0.01, this);
            handler.Start();
        }

        private void Disconnect(object p)
        {
            SocketClient.Instance.Disconnect();
        }

        private void SendMessage(int turn, int speed)
        {
            SocketClient.Instance.SendData(Encoding.UTF8.GetBytes(String.Format("{0} {1}\n", turn, speed)));
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

        public void ProcessSensorData(int turn, int speed)
        {
            //XLineX = 240 - 100 * Math.Sin((double)data.X);
            //XLineY = 100 - 100 * Math.Cos((double)data.X);
            //YLineX = 240 - 100 * Math.Sin((double)data.Y);
            //YLineY = 100 - 100 * Math.Cos((double)data.Y);
            //ZLineX = 240 - 100 * Math.Sin((double)data.Z);
            //ZLineY = 100 - 100 * Math.Cos((double)data.Z);

            //NotifyPropertyChanged("XLineX");
            //NotifyPropertyChanged("XLineY");
            //NotifyPropertyChanged("YLineX");
            //NotifyPropertyChanged("YLineY");
            //NotifyPropertyChanged("ZLineX");
            //NotifyPropertyChanged("ZLineY");

            try
            {
                if (SocketClient.Instance.IsConnected())
                {
                    ConnectionStatus = ViewModel.ConnectionStatus.Connected;
                    if (sendingStatus == ViewModel.SendingStatus.StopSending)
                    {
                        SendMessage(turn, speed);
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

        public void ResetSensorHandler()
        {
            if (handler == null)
            {
                handler = SensorHandlerManager.GetSensorHandler(0.01, this);
                handler.Start();
            }
            else
            {
                handler.Stop();
                handler = SensorHandlerManager.GetSensorHandler(0.01, this);
                handler.Start();
            }
        }
    }
}
