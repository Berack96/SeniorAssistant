using System;
using System.ComponentModel;

namespace SeniorAssistant.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum @enum)
        {
            var type = @enum.GetType();
            var memInfo = type.GetMember(@enum.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return @enum.ToString();
        }
    }
}
