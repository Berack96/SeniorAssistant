using LinqToDB.Mapping;

namespace SeniorAssistant.Models.Users
{
    public class Doctor : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        public string Location { get; set; }

        public string Schedule { get; set; }

        public string PhoneNumber { get; set; }
    }
}
