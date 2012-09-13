using System;
using System.Net;
using System.Windows.Input;
using System.Windows.Navigation;
using robobot_winphone.Model;
using robobot_winphone.Model.EventManager;

namespace robobot_winphone.ViewModel
{
    public class DeviceConnectionPageViewModel : BaseViewModel
    {
        private string port;
        private string ip;

        public NavigationService NService { get; set; }
        public string Port 
        { 
            get
            {
                return port;
            }
            set 
            { 
                port = value;
                NotifyPropertyChanged("Port");
            }
        }
        public string IP
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
                NotifyPropertyChanged("IP");
            }
        }
        public bool IsDisconnectEnable { get; private set; }

        public ICommand DisconnectCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand ChooseConnectionCommand { get; private set; }

        public DeviceConnectionPageViewModel()
        {
            var settings = new Settings();
            ConnectCommand = new ButtonCommand(Connect);
            DisconnectCommand = new ButtonCommand(Disconnect);
            ChooseConnectionCommand = new ButtonCommand(ChooseConnection);
            IP = settings.IP;
            Port = settings.Port;
            IsDisconnectEnable = SocketClient.Instance.IsConnected();
            DataBaseEventManager.Instance.AddHandler(ChangeConnection);
        }
       
        private void Connect(object p)
        {
            try
            {
                var port = Convert.ToInt32(Port);
                var address = IPAddress.Parse(IP);

                SocketClient.Instance.ConnectToDevice(address, Convert.ToInt32(port));
                GoBack();
            }
            catch(Exception)
            {
                //TODO ShowMessage
                LogManager.Log("Error...");
            }
        }

        private void Disconnect(object p)
        {
            SocketClient.Instance.Disconnect();
            GoBack();
        }

        private void ChooseConnection(object p)
        {
            NavigationManager.Instance.NavigateToConnectionsPage();
        }

        private void GoBack()
        {
            if (NService != null)
            {
                NService.GoBack();
            }
        }


        private void ChangeConnection(object sender, DataBaseEventArgs e)
        {
            if (e.Type != DataBaseEventType.Choose) return;
            IP = e.Item.Ip;
            Port = e.Item.Port.ToString();
        }
    }
}
