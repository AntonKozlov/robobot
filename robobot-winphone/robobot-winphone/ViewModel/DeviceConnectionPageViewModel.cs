﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using robobot_winphone.Model;
using System.Text;

namespace robobot_winphone.ViewModel
{
    public class DeviceConnectionPageViewModel : BaseViewModel
    {
        private IPAddress address;

        public NavigationService nService { get; set; }
        public string Port { get; set; }
        public string IP { get; set; }
        public ICommand ConnectCommand { get; private set; }

        public DeviceConnectionPageViewModel()
        {
            Settings settings = new Settings();
            ConnectCommand = new ButtonCommand(Connect);
            IP = settings.IP;
            Port = settings.Port;
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

            SocketClient.Instance.ConnectToDevice(address, Convert.ToInt32(Port));

            if (nService != null)
            {
                nService.GoBack();
            }
        }
    }
}