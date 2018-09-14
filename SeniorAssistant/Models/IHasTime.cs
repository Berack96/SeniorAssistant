using System;

namespace SeniorAssistant.Models
{
    public interface IHasTime : IHasUsername
    {
        DateTime Time { get; set; }
    }
}
