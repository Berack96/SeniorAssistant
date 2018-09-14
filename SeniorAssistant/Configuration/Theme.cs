using System.ComponentModel;

namespace SeniorAssistant.Configuration
{
    public class Theme
    {
        public Skin Skin { get; set; }
        public Layout Layout { get; set; }
    }
    public enum Layout
    {
        [Description("fixed")]
        Fixed,

        [Description("layout-boxed")]
        LayoutBoxed,

        [Description("layout-top-nav")]
        LayoutTopNav,

        [Description("sidebar-collapse")]
        SidebarCollapse,

        [Description("sidebar-mini")]
        SidebarMini
    }

    public enum Skin
    {
        [Description("skin-blue")]
        Blue,

        [Description("skin-black")]
        Black,

        [Description("skin-purple")]
        Purple,

        [Description("skin-yellow")]
        Yellow,

        [Description("skin-red")]
        Red,

        [Description("skin-green")]
        Green
    }
}
