using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public class Notification : IHasTime
    {
        [Column(IsPrimaryKey = true, CanBeNull = false, IsIdentity = true)]
        public int Id { get; set; }

        [NotNull]
        public string Username { get; set; }

        [NotNull]
        public DateTime Time { get; set; }
        
        public bool Seen { get; set; }
        
        public string Message { get; set; }
    }
}
