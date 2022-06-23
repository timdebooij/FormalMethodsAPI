using FormalMethodsAPI.Back_end;
using FormalMethodsAPI.Back_end.Helpers;
using FormalMethodsAPI.Back_end.Models;
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
        [HttpGet("{input}")]
        public string Get(string input)
        {
            if(input == null)
            {
                return "False data";
            }
            input = input.Replace('@', '+');
            RegularExpression exp = RegularExpressionHelper.Generate(input);
            RegExpData data = new RegExpData();
            data.expression = input;
            data.language = exp.GetLanguage(5).ToList().OrderBy(x => x.Length).ToList();
            data.size = data.language.Count;
            data.message = "Succesfully send an expression";
            data.nonLanguage = RegularExpressionHelper.GetRandomWord(exp, RegularExpression.GetAlphabet(input));
            Console.WriteLine(data.language);
            string jsonString = JsonSerializer.Serialize(data);
            return jsonString;
        }

        
    }
}
