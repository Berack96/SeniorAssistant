using System.ComponentModel;

namespace SeniorAssistant.Configuration
{
    public class Kendo
    {
        public string Version { get; set; }
        public Style Style { get; set; }
    }

    public enum Style
    {
        [Description("black")]
        Black,

        [Description("blueopal")]
        Blue,

        [Description("bootstrap-v4")]
        Bootstrap,

        [Description("default-v2")]
        Default,

        [Description("fiori")]
        Fiori,

        [Description("flat")]
        Flat,

        [Description("highcontrast")]
        HighContrast,

        [Description("material")]
        Material,

        [Description("materialblack")]
        MaterialBlack,

        [Description("metro")]
        Metro,

        [Description("metroblack")]
        MetroBlack,

        [Description("nova")]
        Nova,

        [Description("office365")]
        Office,

        [Description("rtl")]
        RTL,

        [Description("silver")]
        Silver,

        [Description("uniform")]
        Uniform,
    }
}
