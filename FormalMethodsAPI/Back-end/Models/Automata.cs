using Newtonsoft.Json;
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

        public void addState(string state)
        {
            states.Add(state);
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
                int counter = 0;
                foreach (char symbol in symbols)
                {
                    foreach (Transition transition in this.transitions)
                    {
                        if (transition.getFromState() == from && transition.getSymbol() == symbol.ToString())
                        {
                            counter++;
                        }
                    }
                    if (counter > 1)
                    {
                        isDFA = false;
                    }
                    counter = 0;
                }
            }

            foreach(Transition t in transitions)
            {
                if(t.getSymbol() == "$")
                {
                    isDFA = false;
                }
            }

            return isDFA;
        }

        private List<Object> getToStates(string from, char symbol)
        {
            return null;
        }

        public List<string> getNextStates(string letter, string beginState)
        {
            List<string> nextStates = new List<string>();
            foreach (Transition transition in transitions)
            {
                if (transition.getFromState() == beginState && transition.getSymbol() == letter)
                {
                    nextStates.Add(transition.getToState());
                }
            }
            return nextStates;
        }

        public static Network getVisNodes(int id, Automata m)
        {

            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int number = 1;
            foreach (string state in m.states)
            {
                string border;
                string background;
                if (m.startStates.Contains(state))
                {
                    border = "black";
                    background = "#54BC61";
                }
                else if (m.finalStates.Contains(state))
                {
                    border = "black";
                    background = "#F67358";
                }
                else
                {
                    border = "#336488";
                    background = "#4E97CC";
                }
                Color color = new Color(border, background);
                Node node = new Node(number, state, color);
                dict.Add(state, number);
                nodes.Add(node);
                number++;
            }

            foreach (Transition trans in m.transitions)
            {

                int idFrom = dict[trans.getFromState()];
                int idTo = dict[trans.getToState()];
                string symbol = trans.getSymbol();
                Edge edge = new Edge(idFrom, idTo, symbol);
                edges.Add(edge);
            }
            List<Edge> removeList = new List<Edge>();
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < edges.Count; j++)
                {
                    if (!(i == j) && !removeList.Contains(edges[j]) && !removeList.Contains(edges[i]))
                    {
                        if (edges[i].from == edges[j].from && edges[i].to == edges[j].to)
                        {
                            edges[i].label = edges[i].label + ", " + edges[j].label;
                            removeList.Add(edges[j]);
                        }
                    }
                }
            }
            foreach (Edge i in removeList)
            {
                edges.Remove(i);
            }
            Network returnlist = new Network(id, nodes, edges, m.getAlphabet(), m.states.ToList(), 200, "", Database.getIds(), m.isDFA());
            return returnlist;
        }

        static public Automata getExampleSlide14Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata m = new Automata(alphabet);

            m.addTransition(new Transition("A", "a", "C"));
            m.addTransition(new Transition("A", "b", "B"));
            m.addTransition(new Transition("A", "b", "C"));

            m.addTransition(new Transition("B", "b", "C"));
            m.addTransition(new Transition("B", "C"));

            m.addTransition(new Transition("C", "a", "D"));
            m.addTransition(new Transition("C", "a", "E"));
            m.addTransition(new Transition("C", "b", "D"));

            m.addTransition(new Transition("D", "a", "B"));
            m.addTransition(new Transition("D", "a", "C"));

            m.addTransition(new Transition("E", "a", "E"));
            m.addTransition(new Transition("E", "D"));

            // only on start state in a dfa:
            m.defineAsStartState("A");

            // two final states:
            m.defineAsFinalState("C");
            m.defineAsFinalState("E");

            return m;
        }

        static public Automata Complement(Automata a)
        {
            Automata returnAutomata = new Automata(a.symbols);
            returnAutomata.states = a.states;
            returnAutomata.transitions = a.transitions;
            returnAutomata.startStates = a.startStates;
            foreach(string state in a.states)
            {
                if (!a.finalStates.Contains(state)){
                    returnAutomata.finalStates.Add(state);
                }
            }
            return returnAutomata;
        }

        static public int Concatenation(Automata a1, Automata a2)
        {
            Database.newStates = new List<NewState>();
            string beginState = a1.startStates.ElementAt(0);
            string endState = a2.finalStates.ElementAt(0);
            SortedSet<char> symbols = new SortedSet<char>();
            symbols.Concat(a1.symbols);
            foreach (char c in a2.symbols)
            {
                symbols.Add(c);
            }
            Automata a3 = new Automata(symbols.ToArray());
            List<string> appendedStates = new List<string>();
            List<NewState> newStates = new List<NewState>();
            if (a1.startStates.Count != 0)
            {
                string firstState = a1.startStates.ElementAt(0);
                appendedStates.Add(firstState);
                //newStates.Add(new NewState(firstState, ))
                List<State> states = new List<State>();
                states.Add(new State(firstState, a1));
                newStates = getNewState(newStates, states, symbols.ToList(), 1,a1,  a2, true);
                if(newStates == null)
                {
                    return -9090;
                }

            }
            Database.name = 0;
            Automata a = new Automata(symbols.ToArray());
            foreach (NewState s in Database.newStates)
            {
                a.addState(s.name);
                foreach (KeyValuePair<char, NewState> k in s.transitions)
                {
                    Transition t = new Transition(s.name, k.Key.ToString(), k.Value.name);
                    a.addTransition(t);
                }
                foreach(State q in s.oldStates)
                {
                    if(q.name == beginState)
                    {
                        a.defineAsStartState(s.name);
                    }
                    if(q.name == endState)
                    {
                        a.defineAsFinalState(s.name);
                    }
                }
            }
            Database.Automatas.Add(Database.nextId, a);
            Database.nextId++;
            return Database.nextId - 1;
        }

        static public int Union(Automata a1, Automata a2)
        {
            Database.newStates = new List<NewState>();
            string beginState = a1.startStates.ElementAt(0);
            string beginState2 = a2.startStates.ElementAt(0);
            string finalState = a1.finalStates.ElementAt(0);
            string finalState2 = a2.finalStates.ElementAt(0);
            SortedSet<char> symbols = new SortedSet<char>();
            symbols.Concat(a1.symbols);
            foreach (char c in a2.symbols)
            {
                symbols.Add(c);
            }
            Automata a3 = new Automata(symbols.ToArray());
            List<NewState> newStates = new List<NewState>();

            List<State> states = new List<State>();
            State firstState = new State(beginState, a1);
            State secondState = new State(beginState2, a2);
            states.Add(firstState);
            states.Add(secondState);
            List<NewState> tuple = getNewState(newStates, states, symbols.ToList(), 1, a1, a2, false);
            if (tuple == null)
            {
                return -9090;
            }

            Database.name = 0;
            Automata a = new Automata(symbols.ToArray());
            foreach (NewState s in Database.newStates)
            {
                a.addState(s.name);
                foreach (KeyValuePair<char, NewState> k in s.transitions)
                {
                    Transition t = new Transition(s.name, k.Key.ToString(), k.Value.name);
                    a.addTransition(t);
                }
                foreach (State q in s.oldStates)
                {
                    if (q.name == finalState || q.name == finalState2)
                    {
                        a.defineAsFinalState(s.name);
                    }
                }
            }
            a.defineAsStartState(NewState.getState(Database.newStates, states).name);
            Database.Automatas.Add(Database.nextId, a);
            Database.nextId++;
            return Database.nextId - 1;
        }

        static public List<NewState> getNewState(List<NewState> newStates, List<State> states, List<char> symbols, int newName,Automata first, Automata second, bool concat)
        {
            Tuple<bool, int> contains = NewState.ContainsState(Database.newStates, states);
            if (!contains.Item1) {
                string name = "Z" + Database.name;
                NewState n = new NewState(states, name);
                Database.name++;
                Database.newStates.Add(n);
                foreach (char c in symbols)
                {
                    List<State> nextStates = new List<State>();
                    foreach (State state in states)
                    {
                        nextStates.Add(new State(state.automata.getNextStates(c.ToString(), state.name)[0], state.automata));
                    }
                    newName++;
                    Tuple<NewState, List<NewState>>  tuple = getNewS(newStates, nextStates, symbols, newName,first, second, concat);
                    if(tuple == null)
                    {
                        return null;
                    }
                    NewState l = tuple.Item1;
                    newStates = tuple.Item2;
                    n.transitions.Add(c, l);
                }
            }
            return newStates;
        }

        static public Tuple<NewState, List<NewState>> getNewS(List<NewState> newStates, List<State> states, List<char> symbols, int newName,Automata first, Automata second, bool concat)
        {
            Tuple<bool, int> contains = NewState.ContainsState(Database.newStates, states);
            if (!contains.Item1)
            {
                NewState n = new NewState(states, "Z" + Database.name);
                Database.name++;
                Database.newStates.Add(n);
                if(Database.newStates.Count > 50)
                {
                    return null;
                }
                newStates.Add(n);
                int index = Database.newStates.IndexOf(n);
                foreach (char c in symbols)
                {
                    List<State> nextStates = new List<State>();
                    foreach (State state in states)
                    {
                        string nextstate = state.automata.getNextStates(c.ToString(), state.name)[0];
                        if(!NewState.CheckState(nextStates, nextstate))
                        {
                            nextStates.Add(new State(nextstate, state.automata));
                        }
                        if (first.finalStates.Contains(nextstate) && !NewState.CheckState(nextStates, second.startStates.ElementAt(0)) && concat)
                        {
                            nextStates.Add(new State(second.startStates.ElementAt(0), second));
                        }
                    }
                    Tuple<NewState, List<NewState>> tuple = getNewS(newStates, nextStates, symbols, newName + 1,first, second, concat);
                    if(tuple == null)
                    {
                        return null;
                    }
                    newStates = tuple.Item2;
                    Database.newStates[index].transitions.Add(c, tuple.Item1);
                    //n.transitions.Add(c, tuple.Item1);
                }
                //Database.newStates.Add(n);
                return Tuple.Create(Database.newStates[index], newStates);
            }
            else
            {
                return Tuple.Create(NewState.getState(Database.newStates, states), newStates);
            }
        }

        static public List<State> GetEpsilonClosure(string state, Automata Ndfa)
        {
            List<State> closureStates = new List<State>();
            if (Ndfa.getNextStates("$", state).Count != 0)
            {
                List<string> nextStates = Ndfa.getNextStates("$", state);
                foreach (string s in nextStates)
                {
                    State newState = new State(s, Ndfa);
                    if (!NewState.HasState(closureStates, newState))
                    {
                        closureStates.Add(newState);
                    }
                    List<State> newCloses = GetEpsilonClosure(s, Ndfa);
                    foreach (State s2 in newCloses)
                    {
                        if (!NewState.HasState(closureStates, s2)) ;
                        closureStates.Add(s2);
                    }
                }
            }
            return closureStates;
        }

        public static int TranslateToDfa(Automata Ndfa)
        {

            NewState deadState = new NewState(new List<State>(), "D");
            Database.transStates.Add(deadState);

            List<State> closures = GetEpsilonClosure(Ndfa.startStates.ElementAt(0), Ndfa);
            List<State> oldStates = new List<State>();
            foreach (State s in closures)
            {
                if (!NewState.HasState(oldStates, s))
                {
                    oldStates.Add(s);
                }
            }
            oldStates.Add(new State(Ndfa.startStates.ElementAt(0), Ndfa));
            NewState begin = new NewState(oldStates, "t" + Database.nameIndex);
            Database.nameIndex++;
            Database.transStates.Add(begin);
            int index = Database.transStates.IndexOf(begin);
            foreach (char s in Ndfa.getAlphabet())
            {
                //List<string> nextStatesString = Ndfa.getNextStates(s.ToString(), Ndfa.startStates.ElementAt(0));
                List<State> nextStates = new List<State>();

                foreach (State state in oldStates)
                {
                    List<string> nextStatesString = Ndfa.getNextStates(s.ToString(), state.name);

                    foreach (string s2 in nextStatesString)
                    {
                        State s4 = new State(s2, Ndfa);
                        if (!NewState.HasState(nextStates, s4))
                        {
                            nextStates.Add(s4);
                        }
                        List<State> tempStates = GetEpsilonClosure(s2, Ndfa);
                        foreach (State s7 in tempStates)
                        {
                            State newS = new State(s7.name, Ndfa);
                            if (!NewState.HasState(nextStates, newS))
                            {
                                nextStates.Add(newS);
                            }
                        }

                    }
                    List<State> closureStates = GetEpsilonClosure(state.name, Ndfa);
                    foreach (State cState in closureStates)
                    {
                        List<string> afterClosure = Ndfa.getNextStates(s.ToString(), cState.name);
                        foreach (string s3 in afterClosure)
                        {
                            State newS = new State(s3, Ndfa);
                            if (!NewState.HasState(nextStates, newS))
                            {
                                nextStates.Add(newS);
                            }
                        }
                    }
                }

                if (nextStates.Count == 0)
                {
                    Database.transStates.ElementAt(index).transitions.Add(s, deadState);
                }
                else
                {
                    Tuple<bool, int> tuple = NewState.ContainsState(Database.transStates, nextStates);
                    if (tuple.Item1)
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, NewState.getState(Database.transStates, nextStates));
                    }
                    else
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, Database.transStates.ElementAt(getNewState(nextStates, Ndfa, deadState)));
                    }
                }
            }
            Automata a = new Automata(Ndfa.symbols.ToArray());
            bool used = false;
            foreach (NewState s in Database.transStates)
            {
                {
                    //a.addState(s.name);
                    foreach (KeyValuePair<char, NewState> k in s.transitions)
                    {
                        Transition t = new Transition(s.name, k.Key.ToString(), k.Value.name);
                        a.addTransition(t);
                    }
                    foreach (State state in s.oldStates)
                    {
                        foreach (string finalState in Ndfa.finalStates)
                        {
                            if (state.name == finalState)
                            {
                                a.defineAsFinalState(s.name);
                            }
                        }
                    }
                }
                
            }
            a.defineAsStartState(Database.transStates[1].name);
            
            Database.Automatas.Add(Database.nextId, a);
            Database.nextId++;
            return Database.nextId - 1;
        }

        public static int getNewState(List<State> nextStates2, Automata Ndfa, NewState dead)
        {
            NewState begin = new NewState(nextStates2, "t" + Database.nameIndex);
            Database.nameIndex++;
            Database.transStates.Add(begin);
            int index = Database.transStates.IndexOf(begin);
            foreach (char s in Ndfa.getAlphabet())
            {
                List<State> nextStates = new List<State>();
                foreach (State state in nextStates2)
                {
                    List<string> nextStatesString = Ndfa.getNextStates(s.ToString(), state.name);

                    foreach (string s2 in nextStatesString)
                    {
                        State s4 = new State(s2, Ndfa);
                        if (!NewState.HasState(nextStates, s4))
                        {
                            nextStates.Add(s4);
                        }
                        List<State> tempStates = GetEpsilonClosure(s2, Ndfa);
                        foreach (State s7 in tempStates)
                        {
                            State newS = new State(s7.name, Ndfa);
                            if (!NewState.HasState(nextStates, newS))
                            {
                                nextStates.Add(newS);
                            }
                        }

                    }
                    List<State> closureStates = GetEpsilonClosure(state.name, Ndfa);
                    foreach (State cState in closureStates)
                    {
                        List<string> afterClosure = Ndfa.getNextStates(s.ToString(), cState.name);
                        foreach (string s3 in afterClosure)
                        {
                            State newS = new State(s3, Ndfa);
                            if (!NewState.HasState(nextStates, newS))
                            {
                                nextStates.Add(newS);
                            }
                        }
                    }
                }


                if (nextStates.Count == 0)
                {
                    Database.transStates.ElementAt(index).transitions.Add(s, dead);
                }
                else
                {
                    Tuple<bool, int> tuple = NewState.ContainsState(Database.transStates, nextStates);
                    if (tuple.Item1)
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, NewState.getState(Database.transStates, nextStates));
                    }
                    else
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, Database.transStates.ElementAt(getNewState(nextStates, Ndfa, dead)));
                    }
                }
            }
            return index;
        }

        public static int MinimiseDfa(Automata dfa)
        {
            EquivalenceTable table = new EquivalenceTable();
            //First seperate final state
            List<string> finals = dfa.finalStates.ToList();
            List<string> nonFinals = new List<string>();
            foreach(string state in dfa.states)
            {
                if (!dfa.finalStates.Contains(state))
                {
                    nonFinals.Add(state);
                }
            }
            table.addGroup(finals);
            table.addGroup(nonFinals);

            EquivalenceTable newTable = table.getNextEquivelance(table, dfa, table);

            while (!table.tablesAreSame(newTable, dfa))
            {
                table = newTable;
                newTable = table.getNextEquivelance(table, dfa, table);
            }

            Automata newDfa = new Automata(dfa.symbols);
            for(int i = 0; i < newTable.equivelences.Count; i++)
            {
                string fromName = newTable.getListName(i);
                foreach(char symbol in dfa.symbols)
                {
                    int index = newTable.getListIndex(dfa.getNextStates(symbol.ToString(), newTable.equivelences[i][0])[0]);
                    string toName = newTable.getListName(index);
                    newDfa.addTransition(new Transition(fromName, symbol.ToString(), toName));
                }
                
            }
            List<string> startStates = new List<string>();
            List<string> finalStates = new List<string>();
            foreach (string state in newDfa.states)
            {
                foreach(string final in dfa.finalStates)
                {
                    if (state.Contains(final))
                    {
                        finalStates.Add(state);
                    }
                }
                foreach (string start in dfa.startStates)
                {
                    if (state.Contains(start))
                    {
                        startStates.Add(state);
                    }
                }
            }
            foreach(string s in startStates)
            {
                newDfa.defineAsStartState(s);
            }
            foreach(string s in finalStates)
            {
                newDfa.defineAsFinalState(s);
            }
            Database.Automatas.Add(Database.nextMinimisedId, newDfa);
            Database.nextMinimisedId++;

            return Database.nextMinimisedId - 1;
        }

        
    }
}
