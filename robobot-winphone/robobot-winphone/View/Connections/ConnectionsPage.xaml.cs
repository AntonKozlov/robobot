using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using robobot_winphone.ViewModel;

namespace robobot_winphone.View.Connections
{
    public partial class ConnectionsPage : PhoneApplicationPage
    {
        private ConnectionsPageViewModel viewModel;

        public ConnectionsPage()
        {
            InitializeComponent();
            viewModel = new ConnectionsPageViewModel();          
            DataContext = viewModel;
        }

        private void ConnectionListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void PageLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigated += viewModel.NavigatedFromEvent;
        }
    }
}