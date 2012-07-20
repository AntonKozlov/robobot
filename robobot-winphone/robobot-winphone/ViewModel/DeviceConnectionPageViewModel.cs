using System;
using System.Net;
using System.Windows.Input;
using System.Windows.Navigation;
using robobot_winphone.Model;

namespace robobot_winphone.ViewModel
{
    public class DeviceConnectionPageViewModel : BaseViewModel
    {
        public NavigationService NService { get; set; }
        public string Port { get; set; }
        public string IP { get; set; }
        public ICommand ConnectCommand { get; private set; }

        public DeviceConnectionPageViewModel()
        {
            var settings = new Settings();
            ConnectCommand = new ButtonCommand(Connect);
            IP = settings.IP;
            Port = settings.Port;
        }
       
        private void Connect(object p)
        {
            try
            {
                var port = Convert.ToInt32(Port);
                var address = IPAddress.Parse(IP);

                SocketClient.Instance.ConnectToDevice(address, Convert.ToInt32(port));

                if (NService != null)
                {
                    NService.GoBack();
                }
            }
            catch(Exception)
            {
                //TODO ShowMessage
                LogManager.Log("Error...");
            }
        }
    }
}
