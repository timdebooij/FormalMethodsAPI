using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Class to present Regular Grammar
    /// </summary>
    public class Grammar
    {
        public List<string> grammar { get; set; }

        public Grammar()
        {
            grammar = new List<string>();
        }

        /// <summary>
        /// Function to parse a NDFA to Regular Grammar
        /// </summary>
        /// <param name="automata"></param>
        public void GenerateGrammar(Automata automata)
        {
            // Variables for the first line of grammar
            string firstStatement = automata.startStates.ElementAt(0) + " -->";
            string end = " | ";
            foreach(Transition transition in automata.transitions)
            {
                // Checking for the start state
                if(transition.GetFromState() == automata.startStates.ElementAt(0))
                {
                    // Adding the transition to the grammar line
                    firstStatement = firstStatement + " " + transition.GetSymbol() + transition.GetToState() + " |";

                    // Checking if the transition goes to the final state
                    if (automata.finalStates.Contains(transition.GetToState()))
                    {
                        end = end + " " + transition.GetSymbol() + " |";
                    }
                }
                
            }

            // Adding first line to the grammar after removing the last " |"
            firstStatement = firstStatement.Remove(firstStatement.Length - 1);
            firstStatement = firstStatement.Remove(firstStatement.Length - 1);
            if (end.Length > 3)
            {
                firstStatement = firstStatement + end;
            }
            grammar.Add(firstStatement);

            // Creating a new line for all other states
            foreach(string state in automata.states)
            {
                // Checking if it's not the startstate
                if (!automata.startStates.Contains(state))
                {
                    // Creating variables
                    string statement = state + " --> ";
                    string end2 = " | ";
                    foreach (Transition transition in automata.transitions)
                    {
                        // Checking if the transition goes from the state
                        if (transition.GetFromState() == state)
                        {
                            // Adding the transition to the grammar line
                            statement = statement + " " + transition.GetSymbol() + transition.GetToState() + " |";

                            // Checking if the transition goes to the final state
                            if (automata.finalStates.Contains(transition.GetToState()))
                            {
                                end2 = end2 + transition.GetSymbol();
                            }
                        }

                    }
                    // Adding the line to the grammar after removing the last " |"
                    statement = statement.Remove(statement.Length - 1);
                    statement = statement.Remove(statement.Length - 1);
                    if (end2.Length > 3)
                    {
                        statement = statement + end2;
                    }
                    grammar.Add(statement);
                }
                
            }

        }
    }
}
