﻿using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace robobot_winphone.Model.DataBase
{
    public class ConnectionDataBase
    {
        private const string ConnectionString = "Data Source=isostore:/DataBase.sdf";
        private const string DefaultName = "New connection";

        public ConnectionDataBase()
        {
            using (var db = new ConnectionDataContext(ConnectionString))
            {
                if (db.DatabaseExists())
                {
                    return;
                }
                db.CreateDatabase();
                db.SubmitChanges();
            }
        }

        public string GetDefaultName()
        {
            using (var db = new ConnectionDataContext(ConnectionString))
            {
                var i = -1;
                foreach (var nameEnd in from p in db.Connections
                                        where p.Name.Length > DefaultName.Length &&
                                              p.Name.StartsWith(DefaultName)
                                        select p.Name.Substring(DefaultName.Length))
                {
                    int parsed;
                    i = Int32.TryParse(nameEnd, out parsed) ? Math.Max(parsed, i) : i;
                }
                return String.Format("{0} {1}", DefaultName, i + 1);
            }
        }

        public void AddConnection(string name, string ip, int port)
        {
            using (var db = new ConnectionDataContext(ConnectionString))
            {
                var newItem = new ConnectionDataBaseItem
                                  {
                                      Name = name,
                                      Ip = ip,
                                      Port = port
                                  };
                try
                {
                    db.Connections.InsertOnSubmit(newItem);
                    db.SubmitChanges();
                }
                catch (Exception)
                {
                    LogManager.Log("Add to DB error");
                }
            }
        }

        public ObservableCollection<ConnectionDataBaseItem> GetItemCollection()
        {
            using (var db = new ConnectionDataContext(ConnectionString))
            {
                var collection = new ObservableCollection<ConnectionDataBaseItem>();
                foreach (var p in db.Connections)
                {
                    collection.Add(p);
                }
                return collection;
            }
        }

        public void DeleteConnection(ConnectionDataBaseItem item)
        {
            using (var db = new ConnectionDataContext(ConnectionString))
            {
                db.Connections.Attach(item);
                db.Connections.DeleteOnSubmit(item);
                db.SubmitChanges();
            }
        }
    }
}
