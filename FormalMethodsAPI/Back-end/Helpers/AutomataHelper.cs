using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Helper class for the Automata to perform actions
    /// </summary>
    public class AutomataHelper
    {
        /// <summary>
        /// Function to parse an automata to a network
        /// </summary>
        /// <param name="id"> The id of the new automata within the database</param>
        /// <param name="m"> The automata to parse</param>
        /// <returns></returns>
        public static Network GetVisNodes(int id, Automata m)
        {
            // Variables for the network
            List<Node> nodes = new List<Node>();
            List<Edge> edges = new List<Edge>();

            // Variables for the transition phase
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int number = 1;

            // Adding all the states
            foreach (string state in m.states)
            {
                string border;
                string background;
                string shape = "circle";
                // Putting different color if it is a startstate
                if (m.startStates.Contains(state))
                {
                    border = "black";
                    if (m.finalStates.Contains(state))
                    {
                        background = "yellow";
                    }
                    else
                    {
                        background = "#54BC61";
                    }
                    
                }
                // Putting different color if it is a finalstate
                else if (m.finalStates.Contains(state))
                {
                    border = "black";
                    if (m.startStates.Contains(state))
                    {
                        background = "yellow";
                    }
                    else
                    {
                        background = "#F67358";
                    }
                    
                }
                else
                {
                    border = "#336488";
                    background = "#4E97CC";
                }
                Color color = new Color(border, background);
                Node node = new Node(number, state, color, shape);
                dict.Add(state, number);
                nodes.Add(node);
                number++;
            }

            // Adding all the edges to the network
            foreach (Transition trans in m.transitions)
            {
                int idFrom = dict[trans.GetFromState()];
                int idTo = dict[trans.GetToState()];
                string symbol = trans.GetSymbol();
                Edge edge = new Edge(idFrom, idTo, symbol);
                edges.Add(edge);
            }

            // Merging edges that have the same from and to state, so that the symbols are together on one arrow
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

            // Creating the network
            Network returnlist = new Network(id, nodes, edges, m.GetAlphabet(), m.states.ToList(), 200, "", Database.GetIds(), m.IsDFA());
            return returnlist;
        }

        /// <summary>
        /// Building a standard automata from the lessons
        /// </summary>
        /// <returns> Build automata</returns>
        static public Automata GetExampleSlide14Lesson2()
        {
            char[] alphabet = { 'a', 'b' };
            Automata m = new Automata(alphabet);

            m.AddTransition(new Transition("A", "a", "C"));
            m.AddTransition(new Transition("A", "b", "B"));
            m.AddTransition(new Transition("A", "b", "C"));

            m.AddTransition(new Transition("B", "b", "C"));
            m.AddTransition(new Transition("B", "C"));

            m.AddTransition(new Transition("C", "a", "D"));
            m.AddTransition(new Transition("C", "a", "E"));
            m.AddTransition(new Transition("C", "b", "D"));

            m.AddTransition(new Transition("D", "a", "B"));
            m.AddTransition(new Transition("D", "a", "C"));

            m.AddTransition(new Transition("E", "a", "E"));
            m.AddTransition(new Transition("E", "D"));

            m.DefineAsStartState("A");
            m.DefineAsFinalState("C");
            m.DefineAsFinalState("E");

            return m;
        }

        static public List<string> GetLanguage(Automata a)
        {
            List<string> language = new List<string>();
            string start = a.startStates.ElementAt(0);
            
            foreach(char symbol in a.symbols)
            {
                string word = symbol.ToString();
                List<string> nextStates = a.GetNextStates(symbol.ToString(), start);
                //foreach(string next in nextStates)
                //{
                    language = language.Concat(GetNextWord(word, a, nextStates[0], 5)).ToList();
                //}
            }
            return language;
        }

        static public List<string> GetNextWord(string currentWord, Automata a, string state, int amount)
        {
            List<string> returnList = new List<string>();
            if (amount > 0)
            {
                foreach (char symbol in a.symbols)
                {
                    //returnList.Add(currentWord + symbol);
                    List<string> nextStates = a.GetNextStates(symbol.ToString(), state);
                    foreach (string next in nextStates)
                    {
                        if (a.finalStates.Contains(next))
                        {
                            returnList.Add(currentWord + symbol);
                            returnList = returnList.Concat(GetNextWord(currentWord + symbol, a, next, amount - 1)).ToList();
                        }
                        else
                        {
                            returnList = returnList.Concat(GetNextWord(currentWord + symbol, a, next, amount - 1)).ToList();
                        }
                    }
                }
            }

            return returnList;
        }

        

        /// <summary>
        /// Creating the complement (Reverse) of an automata
        /// </summary>
        /// <param name="a"> The automata to reverse</param>
        /// <returns> Reversed automata</returns>
        static public Automata Complement(Automata a)
        {
            // New Automata
            Automata returnAutomata = new Automata(a.symbols);
            returnAutomata.states = a.states;
            returnAutomata.transitions = a.transitions;
            returnAutomata.startStates = a.startStates;

            // Switching the final states
            foreach (string state in a.states)
            {
                if (!a.finalStates.Contains(state))
                {
                    returnAutomata.finalStates.Add(state);
                }
            }
            return returnAutomata;
        }

        /// <summary>
        /// Performing a concatenation (AND) operation on two automatas
        /// </summary>
        /// <param name="a1"> First automata</param>
        /// <param name="a2"> Second automata</param>
        /// <returns> Concatenated automata index in the database</returns>
        static public int Concatenation(Automata a1, Automata a2)
        {
            // Variables needed for the new automata
            Database.newStates = new List<NewState>();
            string beginState = a1.startStates.ElementAt(0);
            string endState = a2.finalStates.ElementAt(0);

            // Creating new alphabet with from the two automatas
            SortedSet<char> symbols = new SortedSet<char>();
            symbols.Concat(a1.symbols);
            foreach (char c in a2.symbols)
            {
                symbols.Add(c);
            }

            // creating the new states for the automata
            List<string> appendedStates = new List<string>();
            List<NewState> newStates = new List<NewState>();

            // Starting from the startState from the first automata
            if (a1.startStates.Count != 0)
            {
                string firstState = a1.startStates.ElementAt(0);
                appendedStates.Add(firstState);
                List<State> states = new List<State>();
                states.Add(new State(firstState, a1));

                // getting the NewStates
                newStates = GetNewState(newStates, states, symbols.ToList(), 1, a1, a2, true);

                // If there are no NewStates return an error 
                if (newStates == null)
                {
                    return -9090;
                }

            }

            // Building the new automata
            Database.name = 0;
            Automata a = new Automata(symbols.ToArray());

            // Looping through the new states
            foreach (NewState s in Database.newStates)
            {
                // Adding states
                a.AddState(s.name);

                // Adding transitions
                foreach (KeyValuePair<char, NewState> k in s.transitions)
                {
                    Transition t = new Transition(s.name, k.Key.ToString(), k.Value.name);
                    a.AddTransition(t);
                }
                foreach (State q in s.oldStates)
                {
                    // Defining start states
                    if (q.name == beginState)
                    {
                        a.DefineAsStartState(s.name);
                    }
                    // Defining end states
                    if (q.name == endState)
                    {
                        a.DefineAsFinalState(s.name);
                    }
                }
            }

            // Adding to the database and returning its index
            Database.Automatas.Add(Database.nextId, a);
            Database.nextId++;
            return Database.nextId - 1;
        }

        /// <summary>
        /// Function to perform a Union (OR) operation on two automatas
        /// </summary>
        /// <param name="a1"> First automata</param>
        /// <param name="a2"> Second automata</param>
        /// <returns> The id of the new automata</returns>
        static public int Union(Automata a1, Automata a2)
        {
            // Variables for the creation of the new automata later on
            Database.newStates = new List<NewState>();
            string beginState = a1.startStates.ElementAt(0);
            string beginState2 = a2.startStates.ElementAt(0);
            string finalState = a1.finalStates.ElementAt(0);
            string finalState2 = a2.finalStates.ElementAt(0);

            // Creating new alphabet with from the two automatas
            SortedSet<char> symbols = new SortedSet<char>();
            symbols.Concat(a1.symbols);
            foreach (char c in a2.symbols)
            {
                symbols.Add(c);
            }
            
            // Creating variables for union operation
            List<NewState> newStates = new List<NewState>();
            List<State> states = new List<State>();
            State firstState = new State(beginState, a1);
            State secondState = new State(beginState2, a2);
            states.Add(firstState);
            states.Add(secondState);

            // Getting the new NewStates
            List<NewState> tuple = GetNewState(newStates, states, symbols.ToList(), 1, a1, a2, false);

            // If there are no NewStates return an error 
            if (tuple == null)
            {
                return -9090;
            }

            // Building the new automata
            Database.name = 0;
            Automata a = new Automata(symbols.ToArray());

            // Looping through the new states
            foreach (NewState s in Database.newStates)
            {
                // Adding states
                a.AddState(s.name);

                // Adding transitions
                foreach (KeyValuePair<char, NewState> k in s.transitions)
                {
                    Transition t = new Transition(s.name, k.Key.ToString(), k.Value.name);
                    a.AddTransition(t);
                }

                // Defining end states
                foreach (State q in s.oldStates)
                {
                    if (q.name == finalState || q.name == finalState2)
                    {
                        a.DefineAsFinalState(s.name);
                    }
                }
            }

            // Defining the start state
            a.DefineAsStartState(NewState.GetState(Database.newStates, states).name);

            // Adding to the database and returning its index
            Database.Automatas.Add(Database.nextId, a);
            Database.nextId++;
            return Database.nextId - 1;
        }

        /// <summary>
        /// Function for retrieving the new NewState
        /// </summary>
        /// <param name="newStates"> Current newstates</param>
        /// <param name="states"> Old states of the newState</param>
        /// <param name="symbols"> Alphabet</param>
        /// <param name="newName"> The name for the newstate</param>
        /// <param name="first"> First automata</param>
        /// <param name="second"> Second Automata</param>
        /// <param name="concat"> Boolean to indicate if concatenation is happening</param>
        /// <returns> The new states</returns>
        static public List<NewState> GetNewState(List<NewState> newStates, List<State> states, List<char> symbols, int newName, Automata first, Automata second, bool concat)
        {
            // Checking if the state to add is already present
            Tuple<bool, int> contains = NewState.ContainsState(Database.newStates, states);
            if (!contains.Item1)
            {
                // Adding the new state
                string name = "Z" + Database.name;
                NewState n = new NewState(states, name);
                Database.name++;
                Database.newStates.Add(n);

                // Looping through all symbols to get the new states where they transition to
                foreach (char c in symbols)
                {
                    // Adding all the next states together to create a new NewState (if it doesnt already exists)
                    List<State> nextStates = new List<State>();
                    foreach (State state in states)
                    {
                        nextStates.Add(new State(state.automata.GetNextStates(c.ToString(), state.name)[0], state.automata));
                    }

                    // Getting the new next state from the current state with symbol c
                    newName++;
                    Tuple<NewState, List<NewState>> tuple = GetNewS(newStates, nextStates, symbols, newName, first, second, concat);
                    if (tuple == null)
                    {
                        return null;
                    }

                    // Adding the transtion for the new automata
                    NewState l = tuple.Item1;
                    newStates = tuple.Item2;
                    n.transitions.Add(c, l);
                }
            }
            return newStates;
        }

        /// <summary>
        /// Function to get the new NewState for a specific symbol of the first NewState, this function recursivly gets all the others
        /// </summary>
        /// <param name="newStates"> Current newstates</param>
        /// <param name="states"> Old states of the newState</param>
        /// <param name="symbols"> Alphabet</param>
        /// <param name="newName"> The name for the newstate</param>
        /// <param name="first"> First automata</param>
        /// <param name="second"> Second Automata</param>
        /// <param name="concat"> Boolean to indicate if concatenation is happening</param>
        /// <returns> A tuple of the new NewState and the current newStates</returns>
        static public Tuple<NewState, List<NewState>> GetNewS(List<NewState> newStates, List<State> states, List<char> symbols, int newName, Automata first, Automata second, bool concat)
        {
            // Checking if the current newStates contain the state that is trying to be added
            Tuple<bool, int> contains = NewState.ContainsState(Database.newStates, states);
            if (!contains.Item1)
            {
                // Creating a new NewState
                NewState n = new NewState(states, "Z" + Database.name);
                Database.name++;
                Database.newStates.Add(n);
                if (Database.newStates.Count > 50)
                {
                    return null;
                }

                // Adding an empty NewState and saving its index in the database
                newStates.Add(n);
                int index = Database.newStates.IndexOf(n);

                // Looping through the symbols
                foreach (char c in symbols)
                {
                    // Getting all the next states
                    List<State> nextStates = new List<State>();
                    foreach (State state in states)
                    {
                        string nextstate = state.automata.GetNextStates(c.ToString(), state.name)[0];

                        // checking if the nextstate is not present yet
                        if (!NewState.CheckState(nextStates, nextstate))
                        {
                            nextStates.Add(new State(nextstate, state.automata));
                        }

                        // Part for the concatenation algorithm
                        if (first.finalStates.Contains(nextstate) && !NewState.CheckState(nextStates, second.startStates.ElementAt(0)) && concat)
                        {
                            nextStates.Add(new State(second.startStates.ElementAt(0), second));
                        }
                    }

                    // Getting the new NewState for the symbol transition
                    Tuple<NewState, List<NewState>> tuple = GetNewS(newStates, nextStates, symbols, newName + 1, first, second, concat);
                    if (tuple == null)
                    {
                        return null;
                    }

                    // Adding the transtion
                    newStates = tuple.Item2;
                    Database.newStates[index].transitions.Add(c, tuple.Item1);
                }
                return Tuple.Create(Database.newStates[index], newStates);
            }
            else
            {
                return Tuple.Create(NewState.GetState(Database.newStates, states), newStates);
            }
        }

        /// <summary>
        /// Function to get the epsilon closure from a specific state
        /// </summary>
        /// <param name="state"> The state</param>
        /// <param name="Ndfa"> The automata</param>
        /// <returns> A list of States where following epsilon the automata can get</returns>
        static public List<State> GetEpsilonClosure(string state, Automata Ndfa)
        {
            // Return variable
            List<State> closureStates = new List<State>();

            // Checking if there is an epsilon leaving the current state
            if (Ndfa.GetNextStates("$", state).Count != 0)
            {
                // Getting the next state with the epsilon
                List<string> nextStates = Ndfa.GetNextStates("$", state);

                // Looping through the next states and recursivly getting all end states
                foreach (string s in nextStates)
                {
                    // Adding to state to the return variable
                    State newState = new State(s, Ndfa);
                    if (!NewState.HasState(closureStates, newState))
                    {
                        closureStates.Add(newState);
                    }

                    // Checking the epsilonclosure from the next state
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

        /// <summary>
        /// Function to translate a NDFA to a DFA
        /// </summary>
        /// <param name="Ndfa"> The NDFA to translate</param>
        /// <returns> The translated DFA</returns>
        public static int TranslateToDfa(Automata Ndfa)
        {
            // Creating a deadstate
            NewState deadState = new NewState(new List<State>(), "D");
            Database.transStates.Add(deadState);

            // Getting the closures from the startstate and creating variable for temp saving
            List<State> closures = GetEpsilonClosure(Ndfa.startStates.ElementAt(0), Ndfa);
            List<State> oldStates = new List<State>();

            // Adding the states to the save variable
            foreach (State s in closures)
            {
                if (!NewState.HasState(oldStates, s))
                {
                    oldStates.Add(s);
                }
            }

            // Creating variables
            oldStates.Add(new State(Ndfa.startStates.ElementAt(0), Ndfa));
            NewState begin = new NewState(oldStates, "t" + Database.nameIndex);
            Database.nameIndex++;
            Database.transStates.Add(begin);
            int index = Database.transStates.IndexOf(begin);

            // Looping through the symbols
            foreach (char s in Ndfa.GetAlphabet())
            {
                List<State> nextStates = new List<State>();

                // Looping through the temp save variable
                foreach (State state in oldStates)
                {
                    // Getting the next states
                    List<string> nextStatesString = Ndfa.GetNextStates(s.ToString(), state.name);

                    // Looping through the next states
                    foreach (string s2 in nextStatesString)
                    {
                        // creating new state and adding it to the list.
                        State s4 = new State(s2, Ndfa);
                        if (!NewState.HasState(nextStates, s4))
                        {
                            nextStates.Add(s4);
                        }

                        // getting the epsilonclosures for the next state
                        List<State> tempStates = GetEpsilonClosure(s2, Ndfa);
                        foreach (State s7 in tempStates)
                        {
                            // creating new state and adding it to the list.
                            State newS = new State(s7.name, Ndfa);
                            if (!NewState.HasState(nextStates, newS))
                            {
                                nextStates.Add(newS);
                            }
                        }

                    }

                    // Getting the epsilonclosures for the current state
                    List<State> closureStates = GetEpsilonClosure(state.name, Ndfa);

                    // Looping throught the epsilonclosure
                    foreach (State cState in closureStates)
                    {
                        // Getting next state and adding them to the list
                        List<string> afterClosure = Ndfa.GetNextStates(s.ToString(), cState.name);
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

                // If there are no next states, create transition to the deadstate
                if (nextStates.Count == 0)
                {
                    Database.transStates.ElementAt(index).transitions.Add(s, deadState);
                }
                else
                {
                    // Adding the transition to the correct NewState
                    Tuple<bool, int> tuple = NewState.ContainsState(Database.transStates, nextStates);
                    if (tuple.Item1)
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, NewState.GetState(Database.transStates, nextStates));
                    }
                    else
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, Database.transStates.ElementAt(getNewState(nextStates, Ndfa, deadState)));
                    }
                }
            }

            // Creating the new automata
            Automata a = new Automata(Ndfa.symbols.ToArray());
            foreach (NewState s in Database.transStates)
            {
                {
                    // Adding the transitions
                    foreach (KeyValuePair<char, NewState> k in s.transitions)
                    {
                        Transition t = new Transition(s.name, k.Key.ToString(), k.Value.name);
                        a.AddTransition(t);
                    }

                    // Defining the final states
                    foreach (State state in s.oldStates)
                    {
                        foreach (string finalState in Ndfa.finalStates)
                        {
                            if (state.name == finalState)
                            {
                                a.DefineAsFinalState(s.name);
                            }
                        }
                    }
                }

            }
            // Defining the start state
            a.DefineAsStartState(Database.transStates[1].name);

            // Adding to the database and returning the index
            Database.Automatas.Add(Database.nextId, a);
            Database.nextId++;
            return Database.nextId - 1;
        }

        /// <summary>
        /// Recursive option to get the newStates
        /// </summary>
        /// <param name="nextStates2"> new states</param>
        /// <param name="Ndfa"> The automata</param>
        /// <param name="dead"> The dead state</param>
        /// <returns> The index</returns>
        public static int getNewState(List<State> nextStates2, Automata Ndfa, NewState dead)
        {
            NewState begin = new NewState(nextStates2, "t" + Database.nameIndex);
            Database.nameIndex++;
            Database.transStates.Add(begin);
            int index = Database.transStates.IndexOf(begin);
            foreach (char s in Ndfa.GetAlphabet())
            {
                List<State> nextStates = new List<State>();
                foreach (State state in nextStates2)
                {
                    List<string> nextStatesString = Ndfa.GetNextStates(s.ToString(), state.name);

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
                        List<string> afterClosure = Ndfa.GetNextStates(s.ToString(), cState.name);
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
                        Database.transStates.ElementAt(index).transitions.Add(s, NewState.GetState(Database.transStates, nextStates));
                    }
                    else
                    {
                        Database.transStates.ElementAt(index).transitions.Add(s, Database.transStates.ElementAt(getNewState(nextStates, Ndfa, dead)));
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// Function to mimimise a DFA
        /// </summary>
        /// <param name="dfa"> DFA to minimise</param>
        /// <returns> The id of the minimised DFA</returns>
        public static int MinimiseDfa(Automata dfa)
        {
            // Creating table
            EquivalenceTable table = new EquivalenceTable();

            // Splitting the final states and nonfinal states for the table sets
            List<string> finals = dfa.finalStates.ToList();
            List<string> nonFinals = new List<string>();
            foreach (string state in dfa.states)
            {
                if (!dfa.finalStates.Contains(state))
                {
                    nonFinals.Add(state);
                }
            }

            // Adding the sets
            table.AddGroup(finals);
            table.AddGroup(nonFinals);

            // Getting the next equivelance of the current table
            EquivalenceTable newTable = table.GetNextEquivelance(table, dfa, table);

            // While the tables are not the same, the program keeps looking for the next equivelance
            while (!table.TablesAreSame(newTable, dfa))
            {
                table = newTable;
                newTable = table.GetNextEquivelance(table, dfa, table);
            }

            // Creating the automata
            Automata newDfa = new Automata(dfa.symbols);
            for (int i = 0; i < newTable.equivelences.Count; i++)
            {
                // Creating the transitions
                string fromName = newTable.GetListName(i);
                foreach (char symbol in dfa.symbols)
                {
                    int index = newTable.GetListIndex(dfa.GetNextStates(symbol.ToString(), newTable.equivelences[i][0])[0]);
                    string toName = newTable.GetListName(index);
                    newDfa.AddTransition(new Transition(fromName, symbol.ToString(), toName));
                }

            }

            // Finding the start and final states
            List<string> startStates = new List<string>();
            List<string> finalStates = new List<string>();
            foreach (string state in newDfa.states)
            {
                foreach (string final in dfa.finalStates)
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

            // Adding the start and final states to the automata
            foreach (string s in startStates)
            {
                newDfa.DefineAsStartState(s);
            }
            foreach (string s in finalStates)
            {
                newDfa.DefineAsFinalState(s);
            }

            // Adding to the database and returning the index
            Database.Automatas.Add(Database.nextMinimisedId, newDfa);
            Database.nextMinimisedId++;
            return Database.nextMinimisedId - 1;
        }

        public static void ConstructNetwork(int id, string start, string contain, string end)
        {
            int stateIteration = 0;
            List<char> symbols = new List<char>();
            foreach (char c in start)
            {
                if (!(c == ' ' || c == '-' || symbols.Contains(c)))
                {
                    symbols.Add(c);
                }
            }
            foreach (char c in contain)
            {
                if (!(c == ' ' || c == '-' || symbols.Contains(c)))
                {
                    symbols.Add(c);
                }
            }
            foreach (char c in end)
            {
                if (!(c == ' ' || c == '-' || symbols.Contains(c)))
                {
                    symbols.Add(c);
                }
            }
            Automata network = new Automata(symbols.ToArray());
            string currentState = GetNextState(stateIteration);
            if (!(start == "-"))
            {
                network.DefineAsStartState(GetNextState(stateIteration));
                for (int i = 0; i < start.Length; i++)
                {
                    stateIteration++;
                    network.AddState(GetNextState(stateIteration));
                    network.AddTransition(new Transition(currentState, start[i].ToString(), GetNextState(stateIteration)));
                    currentState = GetNextState(stateIteration);
                }
                stateIteration++;
            }
            if (!(contain == "-"))
            {
                for (int i = 0; i < contain.Length; i++)
                {

                    network.AddState(GetNextState(stateIteration));
                    network.AddTransition(new Transition(currentState, contain[i].ToString(), GetNextState(stateIteration)));
                    currentState = GetNextState(stateIteration);
                    stateIteration++;
                }
            }

            if (!(end == "-"))
            {
                for (int i = 0; i < end.Length; i++)
                {
                    network.AddState(GetNextState(stateIteration));
                    network.AddTransition(new Transition(currentState, end[i].ToString(), GetNextState(stateIteration)));
                    currentState = GetNextState(stateIteration);
                    stateIteration++;
                }
                network.DefineAsFinalState(currentState);
            }


            Database.GetAutomatas();
            Database.Automatas.Add(id, network);
        }

        public static string GetNextState(int iteration)
        {
            List<string> states = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H" };
            return states[iteration];
        }

        public static bool CheckWord(int id, string word)
        {
            // Initializing variables
            Automata automata = Database.Automatas[id];
            List<string> nextStates = new List<string>();
            List<string> tempStates = new List<string>();

            // Getting the next states
            foreach (string state in automata.startStates)
            {
                nextStates = nextStates.Concat(automata.GetNextStates(word[0].ToString(), state)).ToList();
            }
            if (nextStates.Count == 0)
            {
                return false;
            }
            if(word.Length == 1)
            {
                foreach (string state in automata.finalStates)
                {
                    if (nextStates.Contains(state))
                    {
                        return true;
                    }
                }
                return false;
            }
            for (int i = 1; i < word.Length; i++)
            {
                foreach (string state in nextStates)
                {
                    tempStates = tempStates.Concat(automata.GetNextStates(word[i].ToString(), state)).ToList();
                }
                if (tempStates.Count == 0)
                {
                    return false;
                }
                else
                {
                    nextStates = tempStates;
                    tempStates = new List<string>();
                    if (i == word.Length - 1)
                    {
                        if (automata.finalStates.Count != 0)
                        {
                            foreach (string state in automata.finalStates)
                            {
                                if (nextStates.Contains(state))
                                {
                                    return true;
                                }
                                
                            }
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                }
            }
            return true;

        }


    }
}
