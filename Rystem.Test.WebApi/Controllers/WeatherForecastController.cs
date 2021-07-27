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
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            bool isOk = await new TenantData { Name = $"99169c38-99a6-4511-9596-51869cca9f6e/202107(3).json" }.WriteAsync("x").NoContext();
            var x = await new TenantData() { Name = $"99169c38-99a6-4511-9596-51869cca9f6e/202107(2).json" }.ReadAsync().NoContext();
            var t = await x.ConvertToStringAsync().NoContext();
            int q = t.Length;
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
