using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public class Heartbeat : IHasTime
    {
        [PrimaryKey]
        [NotNull]
        [Association(ThisKey = nameof(Username), OtherKey = nameof(User.Username), CanBeNull = false)]
        public string Username { get; set; }

        [PrimaryKey]
        [NotNull]
        public DateTime Time { get; set; }

        public double Value { get; set; }
    }
}
