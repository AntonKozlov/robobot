using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
                    //todo remove tostring
                    DataBaseEventManager.Instance.NotifyDataBaseChanged(value.Port.ToString(), value.Ip);
                }
            }
        }

        public ConnectionsPageViewModel()
        {
            FillDataSource();
        }

        private void FillDataSource()
        {
            var db = new ConnectionDataBase();
            DataSource = db.GetItemCollection();
        }

    }
}
