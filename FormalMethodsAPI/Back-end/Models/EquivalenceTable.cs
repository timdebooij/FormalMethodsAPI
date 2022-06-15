using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
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

        public void addGroup(List<string> group)
        {
            equivelences.Add(group);
        }

        public int getListIndex(string state)
        {
            foreach(List<string> list in equivelences)
            {
                if (list.Contains(state))
                {
                    return equivelences.IndexOf(list);
                }
            }
            return -1;
        }

        public bool compareEnds(string state1, string state2, Automata automata)
        {
            bool same = true;
            foreach(char c in automata.symbols)
            {
                string endState1 = automata.getNextStates(c.ToString(), state1)[0];
                string endState2 = automata.getNextStates(c.ToString(), state2)[0];
                if (!(getListIndex(endState1) == getListIndex(endState2)))
                {
                    same = false;
                }
            }
            return same;
        }

        public int getEquivelantIndex(string state, Automata automata, EquivalenceTable original)
        {
            int index = -1;
            foreach(List<string> list in equivelences)
            {
                foreach(string checkState in list)
                {
                    if(checkState!= state && !automata.finalStates.Contains(checkState))
                    {
                        if (original.compareEnds(state, checkState, automata))
                        {
                            index = equivelences.IndexOf(list);
                        }
                    }
                }
            }
            return index;
        }

        public EquivalenceTable getNextEquivelance(EquivalenceTable current, Automata dfa, EquivalenceTable original)
        {
            EquivalenceTable newTable = new EquivalenceTable();

            foreach (List<string> list in current.equivelences)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        List<string> newList = new List<string>();
                        newList.Add(list[i]);
                        newTable.addGroup(newList);
                    }
                    else
                    {
                        if (current.compareEnds(list[i], list[i - 1], dfa))
                        {
                            int index = newTable.getListIndex(list[i - 1]);
                            newTable.equivelences[index].Add(list[i]);
                        }
                        else
                        {
                            int index = newTable.getEquivelantIndex(list[i], dfa, original);
                            if (index == -1)
                            {
                                List<string> newList = new List<string>();
                                newList.Add(list[i]);
                                newTable.addGroup(newList);
                            }
                            else
                            {
                                newTable.equivelences[index].Add(list[i]);
                            }
                        }
                    }
                }
            }
            return newTable;
        }

        public bool tablesAreSame(EquivalenceTable newTable, Automata dfa) 
        {
            bool same = true;
            foreach(string state in dfa.states)
            {
                int currentIndex = this.getListIndex(state);
                int newIndex = newTable.getListIndex(state);
                if(!(this.equivelences[currentIndex].Count == newTable.equivelences[newIndex].Count))
                {
                    same = false;
                }
            }
            return same;
        }

        public string getListName(int index)
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
