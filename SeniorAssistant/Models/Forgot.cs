using LinqToDB.Mapping;

namespace SeniorAssistant.Models
{
    public class Forgot : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [Column(CanBeNull = false)]
        public string Question { get; set; }

        [Column(CanBeNull = false)]
        public string Answer { get; set; }
    }
}
