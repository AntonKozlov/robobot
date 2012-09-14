using System;
using robobot_winphone.Model.DataBase;

namespace robobot_winphone.Model.EventManager
{
    public enum DataBaseEventType
    {
        Delete,
        Choose
    }

    public class DataBaseEventArgs : EventArgs
    {
        public DataBaseEventType Type;
        public ConnectionDataBaseItem Item;
    }

    public delegate void DataBaseEventHandler(object sender, DataBaseEventArgs e);

    public class DataBaseEventManager
    {
        private static DataBaseEventManager instance;
        private event DataBaseEventHandler dataBaseChanged;

        public static DataBaseEventManager Instance
        {
            get { return instance ?? (instance = new DataBaseEventManager()); }
        }

        private DataBaseEventManager()
        {}

        public void NotifyDataBaseChanged(ConnectionDataBaseItem item, DataBaseEventType type)
        {
            var eventArgs = new DataBaseEventArgs
                                {
                                    Item = item,
                                    Type = type,
                                };

            if (dataBaseChanged != null)
            {
                dataBaseChanged(this, eventArgs);
            }
        }

        public void AddHandler(DataBaseEventHandler handler)
        {
            dataBaseChanged += handler;
        }

        public void RemoveHandler(DataBaseEventHandler handler)
        {
            dataBaseChanged -= handler;
        }
    }
}
