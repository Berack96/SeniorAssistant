using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public class Sleep : IHasTime
    {
        [PrimaryKey]
        [NotNull]
        public string Username { get; set; }

        [PrimaryKey]
        [NotNull]
        public DateTime Time { get; set; }

        public long Value { get; set; }
    }
}
