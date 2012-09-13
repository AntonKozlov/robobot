using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using robobot_winphone.Model.Helpers;

namespace robobot_winphone.View.Connections
{
    public partial class ConnectionItem : UserControl
    {
        public ConnectionItem()
        {
            InitializeComponent();
        }

        private void ChooseClick(object sender, RoutedEventArgs e)
        {
            FindParentHelper.FindParentOfType<PhoneApplicationPage>(this).NavigationService.GoBack();
        }      
    }
}
