using FormalMethodsAPI.Back_end.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Networkcontroller : ControllerBase
    {
        [HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        public string Get(string input)
        {
            Automata m = Automata.getExampleSlide14Lesson2();
            Network network = Automata.getVisNodes(m);
            var jsonBoth = JsonConvert.SerializeObject(network);
            return jsonBoth;
        }
    }
}
