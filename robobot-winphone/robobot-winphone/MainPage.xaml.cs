using System;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;
using robobot_winphone.Model;

namespace robobot_winphone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel();
            DataContext = viewModel;
        }

        private void ConnectClick(object sender, EventArgs e)
        {
            NavigationManager.Instance.NavigateToDeviceConnectionPage();
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeRight)
            {
                Orientation = PageOrientation.LandscapeLeft;
            }
            else
            {
                base.OnOrientationChanged(e);
            }
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            NavigationManager.Instance.NavigateToSettingsPage();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            viewModel.ResetSensorHandler();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            viewModel.StopSensorHandler();
            base.OnNavigatingFrom(e);
        }
    }
}