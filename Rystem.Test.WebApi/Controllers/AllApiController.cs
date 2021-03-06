using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rystem.Business;
using Rystem.Business.Document;
using Rystem.Test.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rystem.Test.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AllApiController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly FirstSingleton F;
        public AllApiController(ILogger<WeatherForecastController> logger, FirstSingleton f)
        {
            _logger = logger;
            F = f;
        }
        [HttpGet]
        public (string, string) Get()
        {
            var q = new Service<IDocumentManager<MyLock>>();
            _ = new MyLock().Configure("A");
            q = new Service<IDocumentManager<MyLock>>();
            return (new Service<FirstSingleton>().Value.Ale, F.Ale);
        }

        public class MyLock : IConfigurableDocument
        {
            public string A { get; set; }
            public string N { get; set; }
            public DateTime C { get; set; }
            public RystemDocumentServiceProvider Configure(string callerName)
            {
                return this.StartConfiguration()
                    .WithAzure()
                    .WithTableStorage()
                    .WithPrimaryKey(x => x.A)
                    .WithSecondaryKey(x => x.N)
                    .WithTimestamp(x => x.C)
                    .ConfigureAfterBuild();
            }
        }
    }
}
