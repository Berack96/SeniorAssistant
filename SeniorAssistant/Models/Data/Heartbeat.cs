using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models.Data
{
    public class Heartbeat : IHasTime
    {
        [PrimaryKey]
        [NotNull]
        public string Username { get; set; }

        [PrimaryKey]
        [NotNull]
        public DateTime Time { get; set; }

        public double Value { get; set; }
    }
}
