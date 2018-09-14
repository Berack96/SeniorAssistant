using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public class Heartbeat : IHasTime
    {
        [PrimaryKey]
        [NotNull]
        public string Username { get; set; }

        [PrimaryKey]
        [NotNull]
        public DateTime Time { get; set; }
        
        [Association(ThisKey = nameof(Username), OtherKey = nameof(User.Username), CanBeNull = false)]
        public User user { get; set; }

        public double Value { get; set; }
    }
}
