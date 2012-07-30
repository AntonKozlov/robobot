using System;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;
using System.Text;

using robobot_winphone.Model;
using robobot_winphone.Model.SensorHandler;

namespace robobot_winphone.ViewModel
{
    public enum ConnectionStatus
    {
        Connected = 0,
        Disconnected = 1
    }

    public enum SendingStatus
    {
        StartSending = 0,
        StopSending = 1
    }

    public class MainPageViewModel : BaseViewModel, ISensorExecutor
    {
        private ConnectionStatus connectionStatus;
        private SendingStatus sendingStatus;
        private AbstractSensorHandler handler;
        private double speedRotation;
        private double turnRotation;

        public double StartSendingValue{ private get; set; }
    
        public ICommand SendingCommand { get; private set; }

        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return connectionStatus;
            }
            set
            {
                if (value == connectionStatus)
                {
                    return;
                }
                connectionStatus = value;
                NotifyPropertyChanged("ConnectionStatus");
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
                if (value == sendingStatus)
                {
                    return;
                }
                sendingStatus = value;
                NotifyPropertyChanged("SendingStatus");
            }
        }

        public double SpeedRotation
        {
            get
            {
                return speedRotation;
            }
            set
            {
                speedRotation = value;
                NotifyPropertyChanged("SpeedRotation");
            }
        }

        public double TurnRotation
        {
            get
            {
                return turnRotation;
            }
            set
            {
                turnRotation = value;
                NotifyPropertyChanged("TurnRotation");
            }
        }

        public MainPageViewModel(Action<SendingStatus> sendingStatusChangedAction)
        {
            PropertyChanged += (sender, args) =>
                                   {
                                       sendingStatusChangedAction(SendingStatus);
                                   };

            SendingCommand = new ButtonCommand(SendOrStopSend);

            ConnectionStatus = ConnectionStatus.Disconnected;
            sendingStatus = SendingStatus.StartSending;

            SocketClient.Instance.Subscriber = this;

            var timer = new DispatcherTimer
                            {
                                Interval = TimeSpan.FromMilliseconds(5)
                            };

            timer.Tick += (sender, e) =>
                {
                    if (SocketClient.Instance.IsConnected())
                    {
                        ConnectionStatus = ConnectionStatus.Connected;
                    }
                    else
                    {
                        ConnectionStatus = ConnectionStatus.Disconnected;
                        StopSensorHandler();
                    }
                };
            timer.Start();

            handler = SensorHandlerManager.GetSensorHandler(0.01, this);
        }


        private void SendMessage(int turn, int speed)
        {
            SocketClient.Instance.SendData(Encoding.UTF8.GetBytes(String.Format("{0} {1}\n", turn, speed)));
            LogManager.Log(String.Format("{0} {1}\n", turn, speed));
        }

        private void SendOrStopSend(object p)
        {
            if ((SendingStatus == SendingStatus.StartSending) && ((double)p - StartSendingValue >= 0))
            {
                StartSensorHandler();
            }
            else if ((double)p - StartSendingValue <= 0)
            {
                StopSensorHandler();
            }
        }

        public void ProcessSensorData(int turn, int speed)
        {
            SpeedRotation = speed * 1.5;
            TurnRotation = turn * 0.5;
            SendMessage(turn, speed);
        }

        public void ResetSensorHandler()
        {
            handler = SensorHandlerManager.GetSensorHandler(0.01, this);
        }

        public void StopSensorHandler()
        {
            handler.Stop();
            SendingStatus = SendingStatus.StartSending;
        }

        public void StartSensorHandler()
        {
            handler.Start();
            SendingStatus = SendingStatus.StopSending;
        }
    }
}
