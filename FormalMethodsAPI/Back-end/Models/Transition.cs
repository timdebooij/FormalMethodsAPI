using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Transition
    {
        public static char EPSILON = '$';

        private string fromState;
        private char symbol;
        private string toState;

        /**
        // this constructor can be used to define loops:
        public Transition(char fromOrTo, char s)
        {
            this(fromOrTo, s, fromOrTo);
        }
        **/

        public Transition(string from, string to)
        {
            fromState = from;
            toState = to;
            symbol = EPSILON;
        }


        public Transition(string from, char s, string to)
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

        public char getSymbol()
        {
            return symbol;
        }

        public String toString()
        {
            return "(" + this.getFromState() + ", " + this.getSymbol() + ")" + "-->" + this.getToState();
        }
    }
}
