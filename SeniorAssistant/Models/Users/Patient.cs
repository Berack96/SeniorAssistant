using LinqToDB.Mapping;

namespace SeniorAssistant.Models.Users
{
    public class Patient : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [Association(ThisKey = "Username", OtherKey = nameof(User.Username), CanBeNull = false)]
        public User UserData { get; set; }

        [NotNull]
        public string Doctor { get; set; }

        public string Notes { get; set; }
    }
}
