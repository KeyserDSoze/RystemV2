using Rystem.Background;
using Rystem.Business;
using Rystem.Cloud;
using Rystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Test.WebApi
{
    public class DailyImport : IBackgroundOptionedJob
    {
        public BackgroundJobOptions Options { get; init; }
        public DailyImport()
        {
        }
        public async Task ActionToDoAsync()
        {
            await Task.Delay(0);
            Console.WriteLine("I'm doing that");
        }
        public Task OnException(Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}