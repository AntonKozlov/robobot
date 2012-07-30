using System.Data.Linq;

namespace robobot_winphone.Model.DataBase
{
    public class ConnectionDataContext : DataContext
    {

        public ConnectionDataContext(string sConnectionString) : base(sConnectionString) { }

        public Table<ConnectionDataBaseItem> Connections
        {
            get
            {
                return this.GetTable<ConnectionDataBaseItem>();
            }
        }
    }
}
