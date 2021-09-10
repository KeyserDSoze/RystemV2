using Rystem.Background;
using Rystem.Business;
using Rystem.Cloud;
using Rystem.Text;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Test.WebApi
{
    public class MonthlyReviewImport : IBackgroundOptionedWork
    {
        public BackgroundWorkOptions Options { get; }
        public MonthlyReviewImport()
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
