using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

namespace robobot_winphone.View
{
    public partial class DeviceConnectionPage : PhoneApplicationPage
    {
        private DeviceConnectionPageViewModel deviceConnectionPageViewModel;

        public DeviceConnectionPage()
        {
            InitializeComponent();
            deviceConnectionPageViewModel = new DeviceConnectionPageViewModel();
            DataContext = deviceConnectionPageViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceConnectionPageViewModel.NService = NavigationService;
            base.OnNavigatedTo(e);
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
    }
}