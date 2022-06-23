using FormalMethodsAPI.Back_end.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end
{
    /// <summary>
    /// Class for the Regular Expressions
    /// </summary>
    public class RegularExpression
    {
        Operator oper;
        String terminals;
        public enum Operator { PLUS, STAR, OR, DOT, ONE}

        RegularExpression left;
        RegularExpression right;

        public RegularExpression()
        {
            oper = Operator.ONE;
            terminals = "";
            left = null;
            right = null;
        }

        public RegularExpression(String p)
        {
            oper = Operator.ONE;
            terminals = p;
            left = null;
            right = null;
        }

        public RegularExpression plus()
        {
            RegularExpression result = new RegularExpression();
            result.oper = Operator.PLUS;
            result.left = this;
            return result;
        }

        public RegularExpression star()
        {
            RegularExpression result = new RegularExpression();
            result.oper = Operator.STAR;
            result.left = this;
            return result;
        }

        public RegularExpression or(RegularExpression e2)
        {
            RegularExpression result = new RegularExpression();
            result.oper = Operator.OR;
            result.left = this;
            result.right = e2;
            return result;
        }

        public RegularExpression dot(RegularExpression e2)
        {
            RegularExpression result = new RegularExpression();
            result.oper = Operator.DOT;
            result.left = this;
            result.right = e2;
            return result;
        }

        public SortedSet<string> GetLanguage(int maxSteps)
        {
            SortedSet<string> emptyLanguage = new SortedSet<string>();
            SortedSet<string> languageResult = new SortedSet<string>();

            SortedSet<string> languageLeft, languageRight;

            if (maxSteps < 1) return emptyLanguage;

            switch(this.oper)
            {
                case Operator.ONE:
                    languageResult.Add(terminals);
                    break;

                case Operator.OR:
                    if(left != null)
                    {
                        languageLeft = left.GetLanguage(maxSteps -1);
                        foreach(string s in languageLeft)
                        {
                            languageResult.Add(s);
                        }
                    }
                    if(right != null)
                    {
                        languageRight = right.GetLanguage(maxSteps -1);
                        foreach (string s in languageRight)
                        {
                            languageResult.Add(s);
                        }
                    }
                    break;

                case Operator.DOT:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguage(maxSteps -1);
                    languageRight = right == null ? emptyLanguage : right.GetLanguage(maxSteps -1);
                    foreach(string s1 in languageLeft)
                        foreach(string s2 in languageRight)
                        {
                            languageResult.Add(s1 + s2);
                        }
                    break;

                case Operator.STAR:
                case Operator.PLUS:
                    languageLeft = left == null ? emptyLanguage : left.GetLanguage(maxSteps -1);
                    foreach (string s in languageLeft)
                    {
                        languageResult.Add(s);
                    }
                    for(int i =1; i < maxSteps; i++)
                    {
                        HashSet<string> languageTemp = new HashSet<string>(languageResult);
                        foreach(string s1 in languageLeft)
                        {
                            foreach(string s2 in languageTemp)
                            {
                                languageResult.Add(s1 + s2);
                            }
                        }
                    }
                    if(this.oper == Operator.STAR)
                    {
                        languageResult.Add("");
                    }
                    break;

                default:
                    break;
            }
            return languageResult;
        }

        

        public static List<string> GetAlphabet(string expression)
        {
            List<string> returnList = new List<string>();
            string onlyLetters = new String(expression.Where(Char.IsLetter).ToArray());
            foreach (char letter in onlyLetters)
            {
                if (!returnList.Contains(letter.ToString()))
                {
                    returnList.Add(letter.ToString());
                }
            }
            return returnList;
        }

        public static List<char> GetCharAlphabet(string expression)
        {
            List<char> returnList = new List<char>();
            string onlyLetters = new String(expression.Where(Char.IsLetter).ToArray());
            foreach (char letter in onlyLetters)
            {
                if (!returnList.Contains(letter))
                {
                    returnList.Add(letter);
                }
            }
            return returnList;
        }
        
        /// <summary>
        /// Function to go from expression to NDFA
        /// </summary>
        /// <param name="expression"> The expression</param>
        /// <param name="automata"> The automata</param>
        /// <param name="lastState"> The last state</param>
        /// <returns> Tuple of automata and last state</returns>
        public static Tuple<Automata, string> GetNdfa(RegularExpression expression, Automata automata, string lastState)
        {
            switch (expression.oper)
            {
                case Operator.PLUS:
                    if (expression.left.terminals == "")
                    {
                        Automata auto = new Automata(automata.symbols);
                        Tuple<Automata, string> tuple6 = GetNdfa(expression.left, auto, null);
                        auto = tuple6.Item1;
                        Tuple<Automata, string> tuple7 = CreatePlus2(automata, lastState, auto);
                        automata = tuple7.Item1;
                        lastState = tuple7.Item2;
                    }
                    else
                    {
                        Tuple<Automata, string> tuple = CreatePlus(automata, expression.left.terminals, lastState);
                        automata = tuple.Item1;
                        lastState = tuple.Item2;
                    }
                    
                    break;
                case Operator.STAR:
                    
                    if (expression.left.terminals == "")
                    {
                        Automata auto = new Automata(automata.symbols);
                        Tuple<Automata, string> tuple6 = GetNdfa(expression.left, auto, null);
                        auto = tuple6.Item1;
                        Tuple<Automata, string> tuple7 = CreateStar2(automata, lastState, auto);
                        automata = tuple7.Item1;
                        lastState = tuple7.Item2;
                    }
                    else
                    {
                        Tuple<Automata, string> tuple1 = CreateStar(automata, expression.left.terminals, lastState);
                        automata = tuple1.Item1;
                        lastState = tuple1.Item2;
                    }
                    break;
                case Operator.ONE:
                    Tuple<Automata, string> tuple2 = CreateOne(automata, expression.terminals, lastState);
                    automata = tuple2.Item1;
                    lastState = tuple2.Item2;
                    break;
                case Operator.OR:
                    if(expression.left.terminals == "" && expression.right.terminals == "")
                    {
                        Automata auto = new Automata(automata.symbols);
                        if (expression.right.oper == Operator.OR)
                        {
                            Tuple<Automata, string> tuple9 = GetNdfa(expression.right, auto, lastState);
                            auto = tuple9.Item1;
                        }
                        else
                        {
                            Tuple<Automata, string> tuple10 = GetNdfa(expression.right, auto, null);
                            auto = tuple10.Item1;
                        }
                        Automata auto2 = new Automata(automata.symbols);
                        if (expression.left.oper == Operator.OR)
                        {
                            Tuple<Automata, string> tuple9 = GetNdfa(expression.left, auto2, lastState);
                            auto2 = tuple9.Item1;
                        }
                        else
                        {
                            Tuple<Automata, string> tuple10 = GetNdfa(expression.left, auto2, null);
                            auto2 = tuple10.Item1;
                        }
                        Tuple<Automata, string> tuple5 = CreateOr2(automata, auto, auto2, lastState);
                        automata = tuple5.Item1;
                        lastState = tuple5.Item2;
                    }
                    else if(expression.left.terminals == "")
                    {
                        string lastClone = lastState;
                        Automata auto = new Automata(automata.symbols);
                        if (expression.left.oper == Operator.OR)
                        {
                            Tuple<Automata, string> tuple9 = GetNdfa(expression.left, auto, lastClone);
                            auto = tuple9.Item1;
                        }
                        else
                        {
                            Tuple<Automata, string>  tuple10 = GetNdfa(expression.left, auto, null);
                            auto = tuple10.Item1;
                        }
                        Tuple<Automata, string> tuple5 = CreateOr3(automata, auto, expression.right.terminals, lastClone);
                        automata = tuple5.Item1;
                        lastState = tuple5.Item2;

                    }
                    else if(expression.right.terminals == "")
                    {
                        string lastClone = lastState;
                        Automata auto = new Automata(automata.symbols);
                        if (expression.right.oper == Operator.OR)
                        {
                            Tuple<Automata, string> tuple9 = GetNdfa(expression.left, auto, lastClone);
                            auto = tuple9.Item1;
                        }
                        else
                        {
                            Tuple<Automata, string> tuple10 = GetNdfa(expression.left, auto, null);
                            auto = tuple10.Item1;
                        }
                        Tuple<Automata, string> tuple5 = CreateOr4(automata, expression.left.terminals, auto, lastClone);
                        automata = tuple5.Item1;
                        lastState = tuple5.Item2;
                    }
                    else
                    {
                        Tuple<Automata, string> tuple5 = CreateOr(automata, expression.left.terminals, expression.right.terminals, lastState);
                        automata = tuple5.Item1;
                        lastState = tuple5.Item2;
                    }
                    
                    break;
                case Operator.DOT:
                    Tuple<Automata, string> tuple3 = GetNdfa(expression.left, automata, lastState);
                    automata = tuple3.Item1;
                    lastState = tuple3.Item2;
                    Tuple<Automata, string> tuple4 = GetNdfa(expression.right, automata, lastState);
                    automata = tuple4.Item1;
                    lastState = tuple4.Item2;
                    break;
            }

            return Tuple.Create(automata, lastState);
        }

        public static char[] GetAlphabetExp(RegularExpression expression)
        {
            List<string> array = new List<string>();
            List<string> arr = GetNextTerminals(expression, array);
            List<char> list = new List<char>();
            foreach(string s in arr)
            {
                for(int i = 0; i < s.Length; i++)
                {
                    if (!list.Contains(s[i]))
                    {
                        list.Add(s[i]);
                    }
                }
            }
            return list.ToArray();
        }

        public static List<string> GetNextTerminals(RegularExpression expression, List<string> currentArray)
        {
            if (!currentArray.Contains(expression.terminals) && !(expression.terminals == ""))
            {
                currentArray.Add(expression.terminals);
            }
            List<string> left = new List<string>();
            List<string> right = new List<string>();
            if (expression.left != null)
            {
                left = GetNextTerminals(expression.left, currentArray);
            }
            if(expression.right != null)
            {
                right = GetNextTerminals(expression.right, currentArray);
            }

            foreach(string l in left)
            {
                if (!currentArray.Contains(l) && !(l == ""))
                {
                    currentArray.Add(l);
                }
            }
            foreach (string r in right)
            {
                if (!currentArray.Contains(r) && !(r == ""))
                {
                    currentArray.Add(r);
                }
            }
            return currentArray;
        }

        public static Tuple<Automata, string> CreateOne(Automata automata, string c, string lastState)
        {
            if(lastState != null)
            {
                string state0 = lastState;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddTransition(new Transition(state0, c, state1));
                
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddTransition(new Transition(state0, c, state1));
            }
            return Tuple.Create(automata, "q" + (Database.nextName -1));
        }

        public static Tuple<Automata, string> CreatePlus(Automata automata, string c, string lastState)
        {
            
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = "q" + Database.nextName;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state1, c, state2));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state1, c, state2));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                
            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreatePlus2(Automata automata, string lastState, Automata auto2)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state2 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.AddTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state2 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.AddTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreateStar2(Automata automata, string lastState, Automata auto2)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state2 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                automata.AddTransition(new Transition(state0, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.AddTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state2 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                automata.AddTransition(new Transition(state0, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.AddTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreateStar(Automata automata, string c, string lastState)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state1, c, state2));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                automata.AddTransition(new Transition(state0, "$", state3));
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state1, c, state2));
                automata.AddTransition(new Transition(state2, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state3));
                automata.AddTransition(new Transition(state0, "$", state3));

            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreateOr2(Automata automata, Automata auto1, Automata auto2, string lastState)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = GetLowest(auto1.states.ToList());//auto1.states.ElementAt(0);
                string state2 = GetHighest(auto1.states.ToList());//auto1.states.Last();
                string state3 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state4 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state4, "$", state5));
                foreach(Transition t in auto1.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (Transition t in auto2.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto1.states)
                {
                    automata.AddState(state);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = GetLowest(auto1.states.ToList());//auto1.states.ElementAt(0);
                string state2 = GetHighest(auto1.states.ToList());//auto1.states.Last();
                string state3 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state4 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state4, "$", state5));
                foreach (Transition t in auto1.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (Transition t in auto2.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto1.states)
                {
                    automata.AddState(state);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreateOr3(Automata automata, Automata auto1, string d, string lastState)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = GetLowest(auto1.states.ToList());//auto1.states.ElementAt(0);
                string state2 = GetHighest(auto1.states.ToList());//auto1.states.Last();
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                string state4 = "q" + Database.nextName;
                Database.nextName++;
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state4, "$", state5));
                automata.AddTransition(new Transition(state3, d, state4));
                foreach (Transition t in auto1.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto1.states)
                {
                    automata.AddState(state);
                }
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = GetLowest(auto1.states.ToList());//auto1.states.ElementAt(0);
                string state2 = GetHighest(auto1.states.ToList());// auto1.states.Last();
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                string state4 = "q" + Database.nextName;
                Database.nextName++;
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state4, "$", state5));
                automata.AddTransition(new Transition(state3, d, state4));
                foreach (Transition t in auto1.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach(string state in auto1.states)
                {
                    automata.AddState(state);
                }
            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreateOr4(Automata automata, string c, Automata auto2, string lastState)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state4 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state4, "$", state5));
                automata.AddTransition(new Transition(state1, c, state2));
                foreach (Transition t in auto2.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = GetLowest(auto2.states.ToList());//auto2.states.ElementAt(0);
                string state4 = GetHighest(auto2.states.ToList());//auto2.states.Last();
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state4, "$", state5));
                automata.AddTransition(new Transition(state1, c, state2));
                foreach (Transition t in auto2.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.AddState(state);
                }
            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static Tuple<Automata, string> CreateOr(Automata automata, string c, string d, string lastState)
        {
            if (lastState != null)
            {
                string state0 = lastState;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                string state4 = "q" + Database.nextName;
                Database.nextName++;
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state1, c, state2));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state3, d, state4));
                automata.AddTransition(new Transition(state4, "$", state5));
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                string state2 = "q" + Database.nextName;
                Database.nextName++;
                string state3 = "q" + Database.nextName;
                Database.nextName++;
                string state4 = "q" + Database.nextName;
                Database.nextName++;
                string state5 = "q" + Database.nextName;
                Database.nextName++;
                automata.AddState(state0);
                automata.AddState(state1);
                automata.AddState(state2);
                automata.AddState(state3);
                automata.AddState(state4);
                automata.AddState(state5);
                automata.AddTransition(new Transition(state0, "$", state1));
                automata.AddTransition(new Transition(state1, c, state2));
                automata.AddTransition(new Transition(state2, "$", state5));
                automata.AddTransition(new Transition(state0, "$", state3));
                automata.AddTransition(new Transition(state3, d, state4));
                automata.AddTransition(new Transition(state4, "$", state5));

            }
            return Tuple.Create(automata, "q" + (Database.nextName - 1));
        }

        public static string GetHighest(List<string> states)
        {
            string highest = "";
            int highestInt = -1;
            foreach(string state in states)
            {
                string checkStr = state.Remove(0, 1);
                if (int.Parse(checkStr) > highestInt)
                {
                    highestInt = int.Parse(checkStr);
                    highest = state;
                }
            }
            return highest;
        }

        public static string GetLowest(List<string> states)
        {
            string lowest = "";
            int lowestInt = 1000000;
            foreach (string state in states)
            {
                string checkStr = state.Remove(0, 1);
                if (int.Parse(checkStr) < lowestInt)
                {
                    lowestInt = int.Parse(checkStr);
                    lowest = state;
                }
            }
            return lowest;
        }
    }
}
