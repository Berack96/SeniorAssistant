using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public class Message : IHasTime
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsIdentity = true)]
        public int Id { get; set; }

        [NotNull]
        public DateTime Time { get; set; }
        
        [NotNull]
        public string Username { get; set; }

        [NotNull]
        public string Reciver { get; set; }

        [NotNull]
        public string Body { get; set; }

        public bool Seen { get; set; }
        
    }
}
