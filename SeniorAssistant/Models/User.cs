using LinqToDB.Mapping;
using Microsoft.AspNetCore.Identity;

namespace SeniorAssistant.Models
{
    public class User : IHasUsername
    {
        [PrimaryKey]
        [NotNull]
        public string Username { get; set; }

        [NotNull]
        public string Email { get; set; }

        [NotNull]
        public string Password { get; set; }

        [NotNull]
        public bool Doctor { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

    }
}
