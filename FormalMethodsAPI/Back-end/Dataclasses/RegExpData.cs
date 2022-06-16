using FormalMethodsAPI.Back_end;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Data
{
    /// <summary>
    /// Data Class for sending regular expression to the frontend
    /// </summary>
    public class RegExpData
    {
        public string message { get; set; }
        public string expression { get; set; }
        public int size { get; set; }
        public List<string> language { get; set; }
        public List<string> nonLanguage { get; set; }
    }
}
