using System;
using System.Linq;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using SeniorAssistant.Models;
using SeniorAssistant.Models.Data;
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
        public ITable<Forgot> Forgot => GetTable<Forgot>();

        public T[] GetLastMessages<T>(ITable<T> table, string receiver, ref int numNotSeen, int max = 10)
            where T : IHasMessage
        {
            var notSeen = (from t in table
                           where t.Receiver.Equals(receiver) && t.Seen == default
                           orderby t.Time descending
                           select t).Take(max).ToArray();
            var messages = new T[max];
            numNotSeen = notSeen.Length;

            int i;
            for (i = 0; i < numNotSeen; i++)
            {
                messages[i] = notSeen[i];
            }

            if (numNotSeen < max)
            {
                var messSeen = (from t in table
                                where t.Receiver.Equals(receiver) && t.Seen != default
                                orderby t.Time descending
                                select t).Take(max - numNotSeen).ToArray();

                foreach (var m in messSeen)
                {
                    messages[i] = m;
                    i++;
                }
            }
            return messages;
        }
    }
}
