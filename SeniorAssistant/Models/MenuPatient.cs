using LinqToDB.Mapping;
using Newtonsoft.Json;

namespace SeniorAssistant.Models.Users
{
    public class MenuPatient : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string PatientUsername { get; set; }

        [JsonIgnore]
        [Association(ThisKey = nameof(PatientUsername), OtherKey = nameof(User.Username), CanBeNull = false)]
        public User Usr { get; set; }
    }
}
