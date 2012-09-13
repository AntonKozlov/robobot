using System;

namespace robobot_winphone.Model.EventManager
{
    public class DataBaseEventArgs : EventArgs
    {
        public string Port;
        public string IP;
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

        public void NotifyDataBaseChanged(string port, string ip)
        {
            var eventArgs = new DataBaseEventArgs
                                {
                                    Port = port, 
                                    IP = ip
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
    }
}
