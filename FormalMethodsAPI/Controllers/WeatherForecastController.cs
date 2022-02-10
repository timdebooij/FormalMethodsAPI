using FormalMethodsAPI.Back_end;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Controllers
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
        //public IEnumerable<WeatherForecast> Get()
        public string Get()
        {
            {
                var rng = new Random();
                Calculator c = new Calculator();
                return c.getString();
                /*return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Summary = c.getString()
                    //Date = DateTime.Now.AddDays(index),
                    //TemperatureC = rng.Next(-20, 55),
                    //Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();*/
            }

        }
    }
}
