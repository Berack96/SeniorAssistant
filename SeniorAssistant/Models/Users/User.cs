using LinqToDB.Mapping;
using Newtonsoft.Json;
using SeniorAssistant.Models.Users;

namespace SeniorAssistant.Models
{
    public class User : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [Column(CanBeNull = false)]
        public string Email { get; set; }

        [JsonIgnore]
        [Column(CanBeNull = false)]
        public string Password { get; set; }
        
        public string Name { get; set; }
        
        public string LastName { get; set; }

        public string Avatar { get; set; }

        [JsonIgnore]
        [Association(ThisKey = nameof(Username), OtherKey = nameof(Doctor.Username), CanBeNull = true)]
        public Doctor Doc { get; set; }

        [JsonIgnore]
        [Association(ThisKey = nameof(Username), OtherKey = nameof(Patient.Username), CanBeNull = true)]
        public Patient Pat { get; set; }

        public bool IsDoctor() => Doc != null;

        public bool IsPatient() => Pat != null;
    }
}
