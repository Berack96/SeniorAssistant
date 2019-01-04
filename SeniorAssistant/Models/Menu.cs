using System.Collections.Generic;

namespace SeniorAssistant.Models
{
    public interface IMenuItem
    {
        string Text { get; set; }
    }

    public class MenuItem : IMenuItem
    {
        public MenuItem(string text, string href = "#")
        {
            Text = text;
            HRef = href;
        }
        public string Text { get; set; }
        public string HRef { get; set; }
    }

    public class SubMenu : IMenuItem
    {
        public string Text { get; set; }
        public IEnumerable<MenuItem> Items { get; set; }
    }
}
