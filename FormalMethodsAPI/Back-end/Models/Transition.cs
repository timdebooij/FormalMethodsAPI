using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Class that represents a transition between states of an automata
    /// </summary>
    public class Transition
    {
        public static string EPSILON = "$";

        private string fromState;
        private string symbol;
        private string toState;

        public Transition(string from, string to)
        {
            fromState = from;
            toState = to;
            symbol = EPSILON;
        }


        public Transition(string from, string s, string to)
        {
            this.fromState = from;
            this.symbol = s;
            this.toState = to;
        }


        public string getFromState()
        {
            return fromState;
        }

        public string getToState()
        {
            return toState;
        }

        public string getSymbol()
        {
            return symbol;
        }
    }
}
