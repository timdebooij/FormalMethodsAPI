using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class NewState
    {
        public List<State> oldStates { get; set; }
        public string name { get; set; }
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

        static public Tuple<bool,int> ContainsState(List<NewState> newStates, List<State> states)
        {
            List<State> clone = new List<State>(states);
            bool contains = false;
            int index = -1;
            foreach(NewState n in newStates)
            {
                clone = new List<State>(states);
                if (n.oldStates.Count == clone.Count)
                {
                    if (NewState.AreSame(n.oldStates, clone))
                    {
                        contains = true;
                        index = newStates.IndexOf(n);
                    }
                }
                
            }
            return Tuple.Create(contains, index);
        }

        static public bool AreSame(List<State> states1, List<State> states2)
        {
            int amount = states1.Count;
            int verified = 0;

            for(int j = 0; j<states1.Count; j++)
                {
                    for (int i = states2.Count - 1; i > -1; i--)
                    {
                        if (states1[j].name == states2[i].name)
                        {
                            verified++;
                            states2.RemoveAt(i);
                        }
                    }
                }

            if(amount == verified)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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

        static public NewState getState(List<NewState> newStates, List<State> states)
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
