using LinqToDB.Mapping;

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
