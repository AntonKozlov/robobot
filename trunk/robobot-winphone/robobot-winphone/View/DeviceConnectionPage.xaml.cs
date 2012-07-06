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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

namespace robobot_winphone.View
{
    public partial class DeviceConnectionPage : PhoneApplicationPage
    {
        DeviceConnectionPageViewModel deviceConnectionPageViewModel;

        public DeviceConnectionPage()
        {
            InitializeComponent();
            deviceConnectionPageViewModel = new DeviceConnectionPageViewModel();
            this.DataContext = deviceConnectionPageViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deviceConnectionPageViewModel.nService = this.NavigationService;
            base.OnNavigatedTo(e);
        }
    }
}