using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Class that represents an Automata
    /// </summary>
    public class Automata
    {
        public List<Transition> transitions { get; set; }
        public SortedSet<string> states { get; set; }
        public SortedSet<string> startStates { get; set; }
        public SortedSet<string> finalStates { get; set; }
        public char[] symbols { get; set; }

        public Automata(List<Transition> transitions, SortedSet<string> states, SortedSet<string> startStates, SortedSet<string> finalStates, char[] symbols)
        {
            this.transitions = transitions;
            this.states = states;
            this.startStates = startStates;
            this.finalStates = finalStates;
            this.symbols = symbols;
        }

        public Automata(char[] symbols)
        {
            transitions = new List<Transition>();
            states = new SortedSet<string>();
            startStates = new SortedSet<string>();
            finalStates = new SortedSet<string>();
            this.setAlphabet(symbols);
        }

        /// <summary>
        /// Setter for alphabet/putting the symbols
        /// </summary>
        /// <param name="symbols">The symbols used in the automata</param>
        public void setAlphabet(char[] symbols)
        {
            this.symbols = symbols;
        }

        /// <summary>
        /// Getter for the Alphabet
        /// </summary>
        /// <returns>The current alphabet from the automata</returns>
        public char[] getAlphabet()
        {
            return symbols;
        }

        /// <summary>
        /// Adds a transition to the automata
        /// </summary>
        /// <param name="t"> The transition to add</param>
        public void addTransition(Transition t)
        {
            transitions.Count();
            transitions.Add(t);
            states.Add(t.getFromState());
            states.Add(t.getToState());
        }

        /// <summary>
        /// Adds a single state to the automata
        /// </summary>
        /// <param name="state"> The state to add</param>
        public void addState(string state)
        {
            states.Add(state);
        }

        /// <summary>
        /// Defines a state within the automata to be a start state
        /// </summary>
        /// <param name="t"> The state to define</param>
        public void defineAsStartState(string t)
        {
            states.Add(t);
            startStates.Add(t);
        }

        /// <summary>
        /// Defines a state within the automata to be a final/end state
        /// </summary>
        /// <param name="t"> The state to define</param>
        public void defineAsFinalState(string t)
        {
            // if already in states no problem because a Set will remove duplicates.
            states.Add(t);
            finalStates.Add(t);
        }

        /// <summary>
        /// Checks the automata to validate if it is a DFA
        /// </summary>
        /// <returns> True in case of DFA and False in case of NDFA</returns>
        public bool isDFA()
        {
            // return variable
            bool isDFA = true;

            // Loopint through all the states
            foreach (string from in states)
            {
                int counter = 0;
                // Checking all the symbols in the automata
                foreach (char symbol in symbols)
                {
                    foreach (Transition transition in this.transitions)
                    {
                        // Checks if the transition starts from the current state with the current symbol
                        if (transition.getFromState() == from && transition.getSymbol() == symbol.ToString())
                        {
                            counter++;
                        }
                    }
                    // If the counter is bigger than 1 it cannot be a DFA, since one state cannot have two leaving transitions with the same input 
                    if (counter > 1)
                    {
                        isDFA = false;
                    }
                    counter = 0;
                }
            }

            // checking all the transitions for epsilon, since DFA's cannot have epsilon transitions.
            foreach(Transition t in transitions)
            {
                if(t.getSymbol() == "$")
                {
                    isDFA = false;
                }
            }

            return isDFA;
        }

        /// <summary>
        /// Retrieves the following states within the automata when a certain input is given
        /// </summary>
        /// <param name="letter"> The input</param>
        /// <param name="beginState"> The current state</param>
        /// <returns> The next states after the input was given</returns>
        public List<string> getNextStates(string letter, string beginState)
        {
            // Return variable, which is a list for the fact that in a NDFA there could be multiple nextstates
            List<string> nextStates = new List<string>();

            // Checking all the transitions
            foreach (Transition transition in transitions)
            {
                // Checking if the transition goes from the beginState with the input letter, if so the toState will be one of the next states
                if (transition.getFromState() == beginState && transition.getSymbol() == letter)
                {
                    nextStates.Add(transition.getToState());
                }
            }
            return nextStates;
        }
    }
}
