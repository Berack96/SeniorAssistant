using LinqToDB.Mapping;
using System;

namespace SeniorAssistant.Models
{
    public interface IHasMessage : IHasTime
    {
        string Receiver { get; set; }

        string Body { get; set; }

        DateTime Seen { get; set; }
    }
}
