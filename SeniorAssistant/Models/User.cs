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
        public string Name { get; set; }
    }
}
