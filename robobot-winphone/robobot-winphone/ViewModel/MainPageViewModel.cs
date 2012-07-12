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
        private AbstractSensorHandler handler;

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
            DisconnectCommand = new ButtonCommand(Disconnect);
            SendingCommand = new ButtonCommand(SendOrStopSend);

            ConnectionStatus = ViewModel.ConnectionStatus.Disconnected;
            sendingStatus = ViewModel.SendingStatus.StartSending;

            SocketClient.Instance.Subscriber = this;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5);
            timer.Tick += (sender, e) =>
                {
                    if (SocketClient.Instance.IsConnected())
                    {
                        ConnectionStatus = ViewModel.ConnectionStatus.Connected;
                    }
                    else
                    {
                        ConnectionStatus = ViewModel.ConnectionStatus.Disconnected;
                        SendingStatus = ViewModel.SendingStatus.StartSending;
                    }
                };
            timer.Start();

            handler = SensorHandlerManager.GetSensorHandler(0.01, this);
        }

        private void Disconnect(object p)
        {
            SocketClient.Instance.Disconnect();
        }

        private void SendMessage(int turn, int speed)
        {
            SocketClient.Instance.SendData(Encoding.UTF8.GetBytes(String.Format("{0} {1}\n", turn, speed)));
            LogManager.Log(String.Format("{0} {1}\n", turn, speed));
        }

        private void SendOrStopSend(object p)
        {
            if (SendingStatus == ViewModel.SendingStatus.StartSending)
            {
                SendingStatus = ViewModel.SendingStatus.StopSending;
                handler.Start();
            }
            else
            {
                SendingStatus = ViewModel.SendingStatus.StartSending;
                handler.Stop();
            }
        }

        public void ProcessSensorData(int turn, int speed)
        {
            SendMessage(turn, speed);
        }

        public void ResetSensorHandler()
        {
            handler = SensorHandlerManager.GetSensorHandler(0.01, this);
        }

        public void StopSensorHandler()
        {
            handler.Stop();
            SendingStatus = ViewModel.SendingStatus.StartSending;
        }
    }
}
