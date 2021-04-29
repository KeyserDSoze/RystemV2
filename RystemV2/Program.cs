using Rystem.BackgroundWork;
using Rystem.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RystemV2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            (new Sample() as IDocument).Build();
        }
        public class Sample : IDocument
        {
            public RystemServices Configure()
            {
                return RystemServices
                    .WithAzure()
                    .UseDocument()
                    .WithTableStorage();
            }
        }
    }
}
