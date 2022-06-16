using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Data Class to store the result of a checked word (checked in automata or Regular Expression)
    /// </summary>
    public class WordFormat
    {
        public bool inLanguage { get; set; }
        public string errorMessage { get; set; }
        public string word { get; set; }

        public WordFormat(bool inLanguage, string errorMessage, string word)
        {
            this.inLanguage = inLanguage;
            this.errorMessage = errorMessage;
            this.word = word;
        }
    }
}
