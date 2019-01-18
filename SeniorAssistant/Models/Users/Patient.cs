using LinqToDB.Mapping;

namespace SeniorAssistant.Models.Users
{
    public class Patient : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [NotNull]
        public string Doctor { get; set; }

        public string Notes { get; set; }

        public int MaxHeart { get; set; }

        public int MinHeart { get; set; }
    }
}
