using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Grammar
    {
        public List<string> grammar { get; set; }

        public Grammar()
        {
            grammar = new List<string>();
        }

        public void generateGrammar(Automata automata)
        {
            string firstStatement = automata.startStates.ElementAt(0) + " -->";
            string end = " | ";
            foreach(Transition transition in automata.transitions)
            {
                if(transition.getFromState() == automata.startStates.ElementAt(0))
                {
                    firstStatement = firstStatement + " " + transition.getSymbol() + transition.getToState() + " |";
                    if (automata.finalStates.Contains(transition.getToState()))
                    {
                        end = end + " " + transition.getSymbol() + " |";
                    }
                }
                
            }
            firstStatement = firstStatement.Remove(firstStatement.Length - 1);
            firstStatement = firstStatement.Remove(firstStatement.Length - 1);
            if (end.Length > 3)
            {
                firstStatement = firstStatement + end;
            }
            
            
            grammar.Add(firstStatement);
            foreach(string state in automata.states)
            {
                if (!automata.startStates.Contains(state))
                {
                    string statement = state + " --> ";
                    string end2 = " | ";
                    foreach (Transition transition in automata.transitions)
                    {
                        if (transition.getFromState() == state)
                        {
                            statement = statement + " " + transition.getSymbol() + transition.getToState() + " |";
                            if (automata.finalStates.Contains(transition.getToState()))
                            {
                                end2 = end2 + transition.getSymbol();
                            }
                        }

                    }
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
