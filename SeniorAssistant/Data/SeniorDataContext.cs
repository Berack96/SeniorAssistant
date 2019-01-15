using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using SeniorAssistant.Models;
using SeniorAssistant.Models.Users;

namespace SeniorAssistant.Data
{
    public class SeniorDataContext : DataConnection
    {
        public SeniorDataContext(IDataProvider dataProvider, string connectionString)
            : base(dataProvider, connectionString)
        { }

        public ITable<User> Users => GetTable<User>();
        public ITable<Heartbeat> Heartbeats => GetTable<Heartbeat>();
        public ITable<Sleep> Sleeps => GetTable<Sleep>();
        public ITable<Step> Steps => GetTable<Step>();
        public ITable<Doctor> Doctors => GetTable<Doctor>();
        public ITable<Patient> Patients => GetTable<Patient>();
        public ITable<Notification> Notifications => GetTable<Notification>();
        public ITable<Message> Messages => GetTable<Message>();
    }
}
