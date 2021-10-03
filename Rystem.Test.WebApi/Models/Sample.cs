using Rystem.Business;
using System;

namespace Rystem.Test.WebApi.Models
{
    public class Sample : IDocument
    {
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class FirstSingleton
    {
        public string Ale { get; set; } = Guid.NewGuid().ToString();
    }
}