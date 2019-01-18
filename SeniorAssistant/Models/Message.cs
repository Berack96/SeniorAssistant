using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public class Message : IHasMessage
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsIdentity = true)]
        public int Id { get; set; }

        [NotNull]
        public DateTime Time { get; set; }
        
        [NotNull]
        public string Username { get; set; }
        
        [NotNull]
        public string Receiver { get; set; }

        public string Body { get; set; }

        public DateTime Seen { get; set; }
    }
}
