﻿using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

namespace robobot_winphone.View
{
    public partial class CalibrationPage : PhoneApplicationPage
    {
        public CalibrationPage()
        {
            InitializeComponent();
            DataContext = new CalibrationPageViewModel();
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