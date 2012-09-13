﻿using System;
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
using robobot_winphone.Model.EventManager;

namespace robobot_winphone.View.Connections
{
    public partial class ConnectionsPage : PhoneApplicationPage
    {
        public ConnectionsPage()
        {
            InitializeComponent();
            DataContext = new ConnectionsPageViewModel();
        }

        private void ConnectionListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}