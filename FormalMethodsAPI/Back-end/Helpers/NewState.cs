using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Class for a new state of an automata that is the result of an operation
    /// </summary>
    public class NewState
    {
        // The old states that together become this state
        public List<State> oldStates { get; set; }
        public string name { get; set; }

        // The newstate where this state goes, the key is the symbol
        public Dictionary<char, NewState> transitions { get; set; }

        public NewState(List<State> oldState, string name, Dictionary<char, NewState> transitions)
        {
            this.oldStates = oldState;
            this.name = name;
            this.transitions = transitions;
        }

        public NewState(List<State> oldStates, string name)
        {
            this.oldStates = oldStates;
            this.name = name;
            this.transitions = new Dictionary<char, NewState>();
        }

        /// <summary>
        /// Function that checks if the newstates conatain a certain combination of states
        /// </summary>
        /// <param name="newStates"> The newstates</param>
        /// <param name="states"> The states to check</param>
        /// <returns> Tuple of the result and index</returns>
        static public Tuple<bool,int> ContainsState(List<NewState> newStates, List<State> states)
        {
            // Variables
            List<State> clone = new List<State>(states);
            bool contains = false;
            int index = -1;

            // Looping through all the new states
            foreach(NewState n in newStates)
            {
                clone = new List<State>(states);

                // Checking if the list contains the same amount of items, otherwise it cannot be the same
                if (n.oldStates.Count == clone.Count)
                {
                    // checking if the lists contain the same items
                    if (NewState.AreSame(n.oldStates, clone))
                    {
                        contains = true;
                        index = newStates.IndexOf(n);
                    }
                }
                
            }
            return Tuple.Create(contains, index);
        }

        /// <summary>
        /// Function to verify if two lists of states contain the same items
        /// </summary>
        /// <param name="states1"> First state list</param>
        /// <param name="states2">Second state list</param>
        /// <returns> result</returns>
        static public bool AreSame(List<State> states1, List<State> states2)
        {
            // Variables
            int amount = states1.Count;
            int verified = 0;

            // Looping through the states of list 1
            for(int j = 0; j<states1.Count; j++)
                {
                    // Looping throught the states of list 2
                    for (int i = states2.Count - 1; i > -1; i--)
                    {
                        // checking if they have the same name
                        if (states1[j].name == states2[i].name)
                        {
                            verified++;
                            states2.RemoveAt(i);
                        }
                    }
                }

            // When amount and verified are the same all items in both lists are the same
            if(amount == verified)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Function that checks if a list of states contains a certain state
        /// </summary>
        /// <param name="states"> The list</param>
        /// <param name="state"> The state</param>
        /// <returns Result></returns>
        static public bool HasState(List<State> states, State state)
        {
            bool has = false;
            foreach(State s in states)
            {
                if(s.name == state.name)
                {
                    has = true;
                }
            }
            return has;
        }

        /// <summary>
        /// Function that returns the NewState that has a certain list of states
        /// </summary>
        /// <param name="newStates"> The Newstates</param>
        /// <param name="states"> The states to check</param>
        /// <returns> The NewState that contains the list</returns>
        static public NewState GetState(List<NewState> newStates, List<State> states)
        {
            List<State> clone = new List<State>(states);
            foreach (NewState n in newStates)
            {
                clone = new List<State>(states);
                if (n.oldStates.Count == states.Count)
                {
                    if (NewState.AreSame(n.oldStates, clone))
                    {
                        return n;
                    }
                }
                
            }
            return null;
        }

        /// <summary>
        /// Function that checks if a list of states contains a certain state
        /// </summary>
        /// <param name="states"> List</param>
        /// <param name="state"> State</param>
        /// <returns> result</returns>
        static public bool CheckState(List<State> states, string state)
        {
            foreach(State s in states)
            {
                if(s.name == state)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
