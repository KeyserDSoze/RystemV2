using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rystem.Business;
using Rystem.Business.Document;
using Rystem.Test.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Rystem.Test.WebApi.Controllers.AllApiController;

namespace Rystem.Test.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AllApi2Controller : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly FirstSingleton F;
        
        public AllApi2Controller(ILogger<WeatherForecastController> logger, FirstSingleton f, IDocumentManager<MyLock> manager)
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
    }
}
