using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class State
    {
        public string name { get; set; }
        public Automata automata { get; set; }

        public State(string name, Automata automata)
        {
            this.name = name;
            this.automata = automata;
        }
    }
}
