using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

namespace robobot_winphone.View.Settings
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsPageViewModel();
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