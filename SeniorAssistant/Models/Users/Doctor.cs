using LinqToDB.Mapping;

namespace SeniorAssistant.Models.Users
{
    public class Doctor : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [Association(ThisKey = "Username", OtherKey = nameof(User.Username), CanBeNull = false)]
        public User UserData { get; set; }

        public string Location { get; set; }

        public string Schedule { get; set; }
    }
}
