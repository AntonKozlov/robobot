using Microsoft.Phone.Controls;

namespace robobot_winphone.View
{
    public partial class CalibrationPage : PhoneApplicationPage
    {
        public CalibrationPage()
        {
            InitializeComponent();
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