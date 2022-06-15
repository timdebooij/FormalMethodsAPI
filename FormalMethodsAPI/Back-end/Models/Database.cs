using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Database
    {
        public static Dictionary<int, Automata> Automatas = new Dictionary<int, Automata>();
        public static int name = 0;
        public static List<NewState> newStates = new List<NewState>();
        public static List<NewState> transStates = new List<NewState>();
        public static int nextId = 200;
        public static int nextCompId = 600;
        public static int nextMinimisedId = 900;
        public static int nextName = 0;
        public static int nameIndex = 0;

        public static List<int> getIds()
        {
            List<int> ids = new List<int>();
            foreach(KeyValuePair<int, Automata> keyValue in Automatas)
            {
                ids.Add(keyValue.Key);
            }
            return ids;
        }

        public static void ResetDatabase()
        {
            Automatas = new Dictionary<int, Automata>();
            getAutomatas();
            name = 0;
            newStates = new List<NewState>();
            transStates = new List<NewState>();
            nextId = 200;
            nextCompId = 600;
            nextMinimisedId = 900;
            nextName = 0;
            nameIndex = 0;
        }

        public static List<int> getDfaIds()
        {
            List<int> ids = new List<int>();
            foreach (KeyValuePair<int, Automata> keyValue in Automatas)
            {
                if (keyValue.Value.isDFA())
                {
                    ids.Add(keyValue.Key);
                }
            }
            return ids;
        }

        public static List<int> getNdfaIds()
        {
            List<int> ids = new List<int>();
            foreach (KeyValuePair<int, Automata> keyValue in Automatas)
            {
                if (!keyValue.Value.isDFA())
                {
                    ids.Add(keyValue.Key);
                }
            }
            return ids;
        }

        public static Dictionary<int, Automata> getAutomatas()
        {
            if(Database.Automatas.Count == 0)
            {
                SortedSet<string> ends = new SortedSet<string>();
                ends.Add("q4");
                SortedSet<string> starts = new SortedSet<string>();
                starts.Add("q0");
                char[] symbols = { 'a', 'b' };
                SortedSet<string> states = new SortedSet<string>();
                states.Add("q0");
                states.Add("q1");
                states.Add("q2");
                states.Add("q3");
                states.Add("q4");
                List<Transition> transitions = new List<Transition>();
                transitions.Add(new Transition("q0", "a", "q0"));
                transitions.Add(new Transition("q0", "b", "q0"));
                transitions.Add(new Transition("q0", "a", "q1"));
                transitions.Add(new Transition("q0", "b", "q3"));
                transitions.Add(new Transition("q1", "b", "q2"));
                transitions.Add(new Transition("q2", "b", "q4"));
                transitions.Add(new Transition("q3", "a", "q4"));
                transitions.Add(new Transition("q4", "a", "q4"));
                transitions.Add(new Transition("q4", "b", "q4"));
                Automata a1 = new Automata(transitions, states, starts, ends, symbols);
                Database.Automatas.Add(100, a1);

                SortedSet<string> ends2 = new SortedSet<string>();
                ends2.Add("q0");
                SortedSet<string> starts2 = new SortedSet<string>();
                starts2.Add("q3");
                char[] symbols2 = { 'a', 'b' };
                SortedSet<string> states2 = new SortedSet<string>();
                states2.Add("q0");
                states2.Add("q1");
                states2.Add("q2");
                states2.Add("q3");
                List<Transition> transitions2 = new List<Transition>();
                transitions2.Add(new Transition("q0", "a", "q1"));
                transitions2.Add(new Transition("q0", "b", "q2"));
                transitions2.Add(new Transition("q1", "b", "q0"));
                transitions2.Add(new Transition("q2", "a", "q0"));
                transitions2.Add(new Transition("q1", "a", "q3"));
                transitions2.Add(new Transition("q2", "b", "q3"));
                transitions2.Add(new Transition("q3", "a", "q3"));
                transitions2.Add(new Transition("q3", "b", "q1"));
                Automata a2 = new Automata(transitions2, states2, starts2, ends2, symbols2);
                Database.Automatas.Add(101, a2);

                SortedSet<string> ends3 = new SortedSet<string>();
                ends3.Add("q0");
                SortedSet<string> starts3 = new SortedSet<string>();
                starts3.Add("q3");
                char[] symbols3 = { 'a', 'b' };
                SortedSet<string> states3 = new SortedSet<string>();
                states3.Add("q0");
                states3.Add("q1");
                states3.Add("q2");
                states3.Add("q3");
                states3.Add("q4");
                List<Transition> transitions3 = new List<Transition>();
                transitions3.Add(new Transition("q0", "a", "q1"));
                transitions3.Add(new Transition("q0", "b", "q4"));
                transitions3.Add(new Transition("q1", "a", "q4"));
                transitions3.Add(new Transition("q1", "b", "q2"));
                transitions3.Add(new Transition("q2", "a", "q3"));
                transitions3.Add(new Transition("q3", "b", "q2"));
                transitions3.Add(new Transition("q3", "a", "q1"));
                transitions3.Add(new Transition("q2", "b", "q4"));
                transitions3.Add(new Transition("q4", "a", "q4"));
                transitions3.Add(new Transition("q4", "b", "q4"));
                Automata a3 = new Automata(transitions3, states3, starts3, ends3, symbols3);
                Database.Automatas.Add(102, a3);

                SortedSet<string> ends4 = new SortedSet<string>();
                ends4.Add("x3");
                SortedSet<string> starts4 = new SortedSet<string>();
                starts4.Add("x1");
                char[] symbols4 = { 'a', 'b' };
                SortedSet<string> states4 = new SortedSet<string>();
                states4.Add("x1");
                states4.Add("x2");
                states4.Add("x3");
                List<Transition> transitions4 = new List<Transition>();
                transitions4.Add(new Transition("x1", "a", "x2"));
                transitions4.Add(new Transition("x1", "b", "x1"));
                transitions4.Add(new Transition("x2", "a", "x3"));
                transitions4.Add(new Transition("x2", "b", "x1"));
                transitions4.Add(new Transition("x3", "a", "x3"));
                transitions4.Add(new Transition("x3", "b", "x3"));
                Automata a4 = new Automata(transitions4, states4, starts4, ends4, symbols4);
                Database.Automatas.Add(103, a4);

                SortedSet<string> ends5 = new SortedSet<string>();
                ends5.Add("y2");
                SortedSet<string> starts5 = new SortedSet<string>();
                starts5.Add("y1");
                char[] symbols5 = { 'a', 'b' };
                SortedSet<string> states5 = new SortedSet<string>();
                states5.Add("y1");
                states5.Add("y2");
                List<Transition> transitions5 = new List<Transition>();
                transitions5.Add(new Transition("y1", "a", "y1"));
                transitions5.Add(new Transition("y1", "b", "y2"));
                transitions5.Add(new Transition("y2", "a", "y1"));
                transitions5.Add(new Transition("y2", "b", "y2"));
                Automata a5 = new Automata(transitions5, states5, starts5, ends5, symbols5);
                Database.Automatas.Add(104, a5);

                SortedSet<string> ends6 = new SortedSet<string>();
                ends6.Add("t3");
                SortedSet<string> starts6 = new SortedSet<string>();
                starts6.Add("t1");
                char[] symbols6 = { 'a', 'b' };
                SortedSet<string> states6 = new SortedSet<string>();
                states6.Add("t1");
                states6.Add("t2");
                states6.Add("t3");
                List<Transition> transitions6 = new List<Transition>();
                transitions6.Add(new Transition("t1", "a", "t3"));
                transitions6.Add(new Transition("t1", "b", "t2"));
                transitions6.Add(new Transition("t2", "a", "t2"));
                transitions6.Add(new Transition("t2", "b", "t2"));
                transitions6.Add(new Transition("t3", "a", "t3"));
                transitions6.Add(new Transition("t3", "b", "t3"));
                Automata a6 = new Automata(transitions6, states6, starts6, ends6, symbols6);
                Database.Automatas.Add(105, a6);

                char[] alphabet = { 'a', 'b', 'c' };
                Automata m = new Automata(alphabet);
                m.addTransition(new Transition("q0", "a", "q0"));
                m.addTransition(new Transition("q0", "q1"));
                m.addTransition(new Transition("q1", "b", "q1"));
                m.addTransition(new Transition("q1", "q2"));
                m.addTransition(new Transition("q2", "c", "q2"));
                m.defineAsStartState("q0");
                m.defineAsFinalState("q2");
                Database.Automatas.Add(106, m);

                char[] alphabet1 = { 'a', 'b'};
                Automata m1 = new Automata(alphabet1);
                m1.addTransition(new Transition("A", "a", "B"));
                m1.addTransition(new Transition("A", "b", "C"));
                m1.addTransition(new Transition("B", "a", "B"));
                m1.addTransition(new Transition("B", "b", "D"));
                m1.addTransition(new Transition("C", "a", "B"));
                m1.addTransition(new Transition("C", "b", "C"));
                m1.addTransition(new Transition("D", "a", "B"));
                m1.addTransition(new Transition("D", "b", "E"));
                m1.addTransition(new Transition("E", "a", "B"));
                m1.addTransition(new Transition("E", "b", "C"));
                m1.defineAsStartState("A");
                m1.defineAsFinalState("E");
                Database.Automatas.Add(107, m1);
            }
            return Database.Automatas;
        }
    }
}
