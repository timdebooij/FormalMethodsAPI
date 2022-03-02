using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Automata
    {
        // Or use a Map structure
        public List<Transition> transitions { get; set; }

        public SortedSet<string> states { get; set; }
        private SortedSet<string> startStates;
        private SortedSet<string> finalStates;
        private char[] symbols;


        public Automata(char[] symbols)
        {
            transitions = new List<Transition>();
            states = new SortedSet<string>();
            startStates = new SortedSet<string>();
            finalStates = new SortedSet<string>();
            this.setAlphabet(symbols);
        }


        public void setAlphabet(char[] symbols)
        {
            this.symbols = symbols;
        }

        public char[] getAlphabet()
        {
            return symbols;
        }

        public void addTransition(Transition t)
        {
            transitions.Count();
            transitions.Add(t);
            states.Add(t.getFromState());
            states.Add(t.getToState());
        }

        public void defineAsStartState(string t)
        {
            // if already in states no problem because a Set will remove duplicates.
            states.Add(t);
            startStates.Add(t);
        }

        public void defineAsFinalState(string t)
        {
            // if already in states no problem because a Set will remove duplicates.
            states.Add(t);
            finalStates.Add(t);
        }

        public void printTransitions()
        {

            foreach (Transition t in transitions)
            {
                Console.WriteLine("(" + t.getFromState() + ", " + t.getSymbol() + ")-->" + t.getToState());
            }
        }

        public bool isDFA()
        {
            bool isDFA = true;

            foreach (string from in states)
            {
                foreach (char symbol in symbols)
                {
                    isDFA = isDFA && getToStates(from, symbol).Count() == 1;
                }
            }

            return isDFA;
        }

        private List<Object> getToStates(string from, char symbol)
        {
            return null;
        }

        public static Network getVisNodes(Automata m)
        {

            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int number = 1;
            foreach (string state in m.states)
            {
                Node node = new Node(number, state);
                dict.Add(state, number);
                nodes.Add(node);
                number++;
            }

            foreach (Transition trans in m.transitions)
            {

                int idFrom = dict[trans.getFromState()];
                int idTo = dict[trans.getToState()];
                char symbol = trans.getSymbol();
                Edge edge = new Edge(idFrom, idTo, symbol);
                edges.Add(edge);
            }
            Network returnlist = new Network(nodes, edges);
            return returnlist;
        }

        static public Automata getExampleSlide14Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata m = new Automata(alphabet);

            m.addTransition(new Transition("A", 'a', "C"));
            m.addTransition(new Transition("A", 'b', "B"));
            m.addTransition(new Transition("A", 'b', "C"));

            m.addTransition(new Transition("B", 'b', "C"));
            m.addTransition(new Transition("B", "C"));

            m.addTransition(new Transition("C", 'a', "D"));
            m.addTransition(new Transition("C", 'a', "E"));
            m.addTransition(new Transition("C", 'b', "D"));

            m.addTransition(new Transition("D", 'a', "B"));
            m.addTransition(new Transition("D", 'a', "C"));

            m.addTransition(new Transition("E", 'a', "E"));
            m.addTransition(new Transition("E", "D"));

            // only on start state in a dfa:
            m.defineAsStartState("A");

            // two final states:
            m.defineAsFinalState("C");
            m.defineAsFinalState("E");

            return m;
        }
    }
}
