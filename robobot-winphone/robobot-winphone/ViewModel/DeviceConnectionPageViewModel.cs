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
using robobot_winphone.Model;

namespace robobot_winphone.ViewModel
{
    public class DeviceConnectionPageViewModel : BaseViewModel
    {
        private IPAddress address;

        public string Port { get; set; }
        public string IP { get; set; }
        public ICommand ConnectCommand { get; private set; }

        public DeviceConnectionPageViewModel()
        {
            ConnectCommand = new ButtonCommand(Connect);
        }
       
        private void Connect(object p)
        {
            if ((Port != null) && (IP != null))
            {
                if (IPAddress.TryParse(IP, out address))
                {
                    address = IPAddress.Parse(IP);
                }
            }

            //ToDo: Connect
        }
    }
}
