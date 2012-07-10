using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;
using robobot_winphone.Model;

namespace robobot_winphone
{
    public partial class MainPage : PhoneApplicationPage
    {
        MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel();
            this.DataContext = viewModel;
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            NavigationManager.Instance.NavigateToDeviceConnectionPage();
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeRight)
            {
                this.Orientation = PageOrientation.LandscapeLeft;
            }
            else
            {
                base.OnOrientationChanged(e);
            }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationManager.Instance.NavigateToSettingsPage();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            viewModel.ResetSensorHandler();
            base.OnNavigatedTo(e);
        }
    }
}