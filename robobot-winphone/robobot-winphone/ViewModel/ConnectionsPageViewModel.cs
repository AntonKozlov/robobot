using System.Collections.ObjectModel;
using System.Windows.Navigation;
using robobot_winphone.Model.DataBase;
using robobot_winphone.Model.EventManager;

namespace robobot_winphone.ViewModel
{
    public class ConnectionsPageViewModel : BaseViewModel
    {
        private ConnectionDataBaseItem selectedItem;
        private ObservableCollection<ConnectionDataBaseItem> dataSource;

        public ObservableCollection<ConnectionDataBaseItem> DataSource
        {
            get
            {
                return dataSource;
            }
            private set
            {
                dataSource = value;
                NotifyPropertyChanged("DataSource");
            }
        }

        public ConnectionDataBaseItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (value != null)
                {
                    DataBaseEventManager.Instance.NotifyDataBaseChanged(value, DataBaseEventType.Choose);
                }
            }
        }

        public ConnectionsPageViewModel()
        {
            UpdateDataSource();
            DataBaseEventManager.Instance.AddHandler(DeleteConnection);
        }

        private void UpdateDataSource()
        {
            var db = new ConnectionDataBase();
            DataSource = db.GetItemCollection();
        }

        private void DeleteConnection(object sender, DataBaseEventArgs e)
        {
            if (e.Type != DataBaseEventType.Delete) return;
            var db = new ConnectionDataBase();
            db.DeleteConnection(e.Item);
            UpdateDataSource();
        }

        public void NavigatedFromEvent(object sender, NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                DataBaseEventManager.Instance.RemoveHandler(DeleteConnection);
            }
        }
    }
}
