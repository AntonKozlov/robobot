using System;
using System.Windows;
using Microsoft.Phone.Controls;

namespace robobot_winphone.Model
{
    public class NavigationManager
    {
        private static NavigationManager instance;

        public static NavigationManager Instance
        {
            get { return instance ?? (instance = new NavigationManager()); }
        }

        private NavigationManager() { }

        private static void Navigate(string uri)
        {
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            
            if (frame == null)
            {
                return;
            }

            frame.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute));

        }

        public void NavigateToDeviceConnectionPage()
        {
            Navigate("//View/DeviceConnectionPage.xaml");
        }

        public void NavigateToSettingsPage()
        {
            Navigate("//View/SettingsPage.xaml");
        }

        public void NavigateToCalibrationPage()
        {
            Navigate("//View/CalibrationPage.xaml");
        }
    }
}
