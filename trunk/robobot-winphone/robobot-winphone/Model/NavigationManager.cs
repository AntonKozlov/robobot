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
using Microsoft.Phone.Controls;

namespace robobot_winphone.Model
{
    public class NavigationManager
    {
        private static NavigationManager instance;

        public static NavigationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NavigationManager();
                }
                return instance;
            }
        }

        private NavigationManager() { }

        private void Navigate(string uri)
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            
            if (frame == null)
            {
                return;
            }
            frame.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }
        public void NavigateToDeviceConnectionPage()
        {
            this.Navigate("//View/DeviceConnectionPage.xaml");
        }
    }
}
