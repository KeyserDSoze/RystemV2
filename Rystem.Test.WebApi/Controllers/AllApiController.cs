using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            => (new Service<FirstSingleton>().Value.Ale, F.Ale);
    }
}
