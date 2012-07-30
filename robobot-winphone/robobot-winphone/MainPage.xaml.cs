using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using robobot_winphone.ViewModel;
using robobot_winphone.Model;
using robobot_winphone.Model.SensorHandler;

namespace robobot_winphone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel(ChangeAppBarVisibility);
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

        private void ChangeAppBarVisibility(SendingStatus sendingStatus)
        {
            ApplicationBar.IsVisible = sendingStatus == SendingStatus.StartSending;
        }

        private void SliderManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            viewModel.StartSendingValue = Slider.Value;
        }

        private void SliderManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            viewModel.SendingCommand.Execute(Slider.Value);
        }
    }
}