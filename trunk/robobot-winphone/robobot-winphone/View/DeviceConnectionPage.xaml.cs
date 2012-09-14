using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

namespace robobot_winphone.View
{
    public partial class DeviceConnectionPage : PhoneApplicationPage
    {
        private DeviceConnectionPageViewModel viewModel;

        public DeviceConnectionPage()
        {
            InitializeComponent();
            viewModel = new DeviceConnectionPageViewModel();
            DataContext = viewModel;
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

        private void PageLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.NService = NavigationService;
            NavigationService.Navigated += viewModel.NavigatedFromEvent;
        }
    }
}