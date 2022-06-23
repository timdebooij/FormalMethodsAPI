using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Calss to represent an equivalencetable to minimise a DFA
    /// </summary>
    public class EquivalenceTable
    {
        public List<List<string>> equivelences { get; set; }

        public EquivalenceTable(List<List<string>> equivelences)
        {
            this.equivelences = equivelences;
        }

        public EquivalenceTable()
        {
            equivelences = new List<List<string>>();
        }

        public void AddGroup(List<string> group)
        {
            equivelences.Add(group);
        }

        /// <summary>
        /// Getting the index of the list that the state is in
        /// </summary>
        /// <param name="state"> State to check</param>
        /// <returns> Index</returns>
        public int GetListIndex(string state)
        {
            // Looping through the tables
            foreach(List<string> list in equivelences)
            {
                // If the list contains the state, return the index
                if (list.Contains(state))
                {
                    return equivelences.IndexOf(list);
                }
            }

            // Not found, so returning -1
            return -1;
        }

        /// <summary>
        /// Function that compares if the nextstate of two states are in the same table
        /// </summary>
        /// <param name="state1"> First state</param>
        /// <param name="state2"> Second state</param>
        /// <param name="automata"> The automata</param>
        /// <returns> Result</returns>
        public bool CompareEnds(string state1, string state2, Automata automata)
        {
            bool same = true;

            // Checking all symbols
            foreach(char c in automata.symbols)
            {
                // Getting the next states
                string endState1 = automata.GetNextStates(c.ToString(), state1)[0];
                string endState2 = automata.GetNextStates(c.ToString(), state2)[0];

                // Checking if they are in the same table
                if (!(GetListIndex(endState1) == GetListIndex(endState2)))
                {
                    same = false;
                }
            }
            return same;
        }

        /// <summary>
        /// Function to return the index of the table that the  states equivelance is in
        /// </summary>
        /// <param name="state"> The state</param>
        /// <param name="automata"> The automata</param>
        /// <param name="original"> the original tables</param>
        /// <returns> The index</returns>
        public int GetEquivelantIndex(string state, Automata automata, EquivalenceTable original)
        {
            int index = -1;

            // Looping through all tables
            foreach(List<string> list in equivelences)
            {
                // Looping throught the states
                foreach(string checkState in list)
                {
                    // Checking if it is not the current state or one of the final states
                    if(checkState!= state && !automata.finalStates.Contains(checkState))
                    {
                        // checking for equivalence
                        if (original.CompareEnds(state, checkState, automata))
                        {
                            index = equivelences.IndexOf(list);
                        }
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// Getting the new equivalence table
        /// </summary>
        /// <param name="current"> current table</param>
        /// <param name="dfa"> The automata</param>
        /// <param name="original"> Copy of the current table</param>
        /// <returns></returns>
        public EquivalenceTable GetNextEquivelance(EquivalenceTable current, Automata dfa, EquivalenceTable original)
        {
            // Creating the new table
            EquivalenceTable newTable = new EquivalenceTable();

            // Looping through the sets
            foreach (List<string> list in current.equivelences)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    // If it is the first add a new group to the newe table
                    if (i == 0)
                    {
                        List<string> newList = new List<string>();
                        newList.Add(list[i]);
                        newTable.AddGroup(newList);
                    }
                    else
                    {
                        // Comparing with another item in the same set
                        if (current.CompareEnds(list[i], list[i - 1], dfa))
                        {
                            //Adding to the same set
                            int index = newTable.GetListIndex(list[i - 1]);
                            newTable.equivelences[index].Add(list[i]);
                        }
                        else
                        {
                            // Checking for equivalence in the new set
                            int index = newTable.GetEquivelantIndex(list[i], dfa, original);
                            if (index == -1)
                            {
                                // Is not equivalent, so adding a new set
                                List<string> newList = new List<string>();
                                newList.Add(list[i]);
                                newTable.AddGroup(newList);
                            }
                            else
                            {
                                // Is equivalent, so adding to the equivalent set
                                newTable.equivelences[index].Add(list[i]);
                            }
                        }
                    }
                }
            }
            return newTable;
        }

        /// <summary>
        /// Checking if the two tables are the same to indicate the end of minimisation
        /// </summary>
        /// <param name="newTable"> the new table</param>
        /// <param name="dfa"> The automata</param>
        /// <returns> Result</returns>
        public bool TablesAreSame(EquivalenceTable newTable, Automata dfa) 
        {
            bool same = true;

            // Looping through the states
            foreach(string state in dfa.states)
            {
                // Checking amounts
                int currentIndex = this.GetListIndex(state);
                int newIndex = newTable.GetListIndex(state);
                if(!(this.equivelences[currentIndex].Count == newTable.equivelences[newIndex].Count))
                {
                    same = false;
                }
            }
            return same;
        }

        // Getting the name for the new state of the minimised DFA
        public string GetListName(int index)
        {
            string name = "";
            foreach(string n in equivelences[index])
            {
                name = name + n;
            }
            return name;
        }
    }
}
