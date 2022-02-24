using FormalMethodsAPI.Back_end;
using FormalMethodsAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegExpController : ControllerBase
    {
        [HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        public string Get(string input)
        {
            if(input == null)
            {
                return "False data";
            }
            RegularExpression exp = RegularExpression.generate(input);
            RegExpData data = new RegExpData();
            data.expression = input;
            data.language = exp.getLanguage(5).ToList();
            data.size = data.language.Count;
            data.message = "Succesfully send an expression";
            Console.WriteLine(data.language);

            string jsonString = JsonSerializer.Serialize(data);
            return jsonString;
        }
    }
}
