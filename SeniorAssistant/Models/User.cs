using LinqToDB.Mapping;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace SeniorAssistant.Models
{
    public class User : IHasUsername
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Username { get; set; }

        [NotNull]
        public string Email { get; set; }

        [NotNull]
        [JsonIgnore]
        public string Password { get; set; }

        [NotNull]
        public string Role { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
    }
}
