using System.Windows.Controls;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

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