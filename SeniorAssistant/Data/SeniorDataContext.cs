using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using SeniorAssistant.Models;

namespace SeniorAssistant.Data
{
    public class SeniorDataContext : DataConnection
    {
        public SeniorDataContext(IDataProvider dataProvider, string connectionString)
            : base(dataProvider, connectionString)
        { }

        public ITable<User> User => GetTable<User>();

        public ITable<Heartbeat> Heartbeats => GetTable<Heartbeat>();
    }
}
