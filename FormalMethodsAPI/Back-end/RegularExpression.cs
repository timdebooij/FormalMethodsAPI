using FormalMethodsAPI.Back_end.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end
{
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

        public SortedSet<string> getLanguage(int maxSteps)
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
                        languageLeft = left.getLanguage(maxSteps -1);
                        foreach(string s in languageLeft)
                        {
                            languageResult.Add(s);
                        }
                    }
                    if(right != null)
                    {
                        languageRight = right.getLanguage(maxSteps -1);
                        foreach (string s in languageRight)
                        {
                            languageResult.Add(s);
                        }
                    }
                    break;

                case Operator.DOT:
                    languageLeft = left == null ? emptyLanguage : left.getLanguage(maxSteps -1);
                    languageRight = right == null ? emptyLanguage : right.getLanguage(maxSteps -1);
                    foreach(string s1 in languageLeft)
                        foreach(string s2 in languageRight)
                        {
                            languageResult.Add(s1 + s2);
                        }
                    break;

                case Operator.STAR:
                case Operator.PLUS:
                    languageLeft = left == null ? emptyLanguage : left.getLanguage(maxSteps -1);
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
                    Console.WriteLine("getLanguage is nog niet gedefinieerd voor de operator: " + this.oper);
                    break;
            }
            return languageResult;
        }

        public static RegularExpression getTestExpression()
        {
            RegularExpression expr1, expr2, expr3, expr4, expr5, a, b, all;
            a = new RegularExpression("a");
            b = new RegularExpression("b");
            // expr1: "baa"
            expr1 = new RegularExpression("baa");
            // expr2: "bb"
            expr2 = new RegularExpression("bb");
            // expr3: "baa | baa"
            expr3 = expr1.or(expr2);

            // all: "(a|b)*"
            all = (a.or(b)).star();

            // expr4: "(baa | baa)+"
            expr4 = expr3.plus();
            // expr5: "(baa | baa)+ (a|b)*"
            expr5 = expr4.dot(all);
            return expr5;
        }

        public static string getExpressionString(RegularExpression exp)
        {
            RegularExpression current = exp;
            string result = exp.terminals;
            while(current.left != null)
            {
                result = current.left.terminals + operToString(current.left.oper) + result;
                current = current.left;
            }
            current = exp;
            while(current.right != null)
            {
                result = result + operToString(current.right.oper) + current.right.terminals;
                current = current.right;
            }
            return result;
        }

        public static string operToString(Operator o)
        {
            switch (o)
            {
                case Operator.ONE:
                    return " ";
                case Operator.DOT:
                    return " ";
                case Operator.STAR:
                    return "*";
                case Operator.PLUS:
                    return "+";
                case Operator.OR:
                    return "|";
                default:
                    return " ";
            }
        }
        public static RegularExpression generate(string input)
        {
            input = Regex.Replace(input, @"\s+", "");
            List<RegularExpression> expressions = new List<RegularExpression>();
            string[] arr = input.Split('!');
            foreach (string s in arr)
            {

                string[] plusArrayTemp = s.Split('(', ')');
                List<string> plusArray = new List<string>();
                foreach (string item in plusArrayTemp)
                {
                    if (item.Count() > 0)
                    {
                        plusArray.Add(item);
                    }
                }

                foreach (string q in plusArray)//seperateStarPlus(plusArray))
                {
                    if (q.Contains('|'))
                    {
                        string[] orSplit = q.Split('|');
                        RegularExpression splitExp = new RegularExpression(orSplit[0]);
                        if (orSplit[0].Contains("*"))
                        {
                            splitExp = new RegularExpression(orSplit[0][0].ToString());
                            splitExp = splitExp.star();
                        }
                        else if (orSplit[0].Contains("+"))
                        {
                            splitExp = new RegularExpression(orSplit[0][0].ToString());
                            splitExp = splitExp.plus();
                        }
                        
                        for (int i = 1; i < orSplit.Count(); i++)
                        {
                            RegularExpression tempExp = new RegularExpression(orSplit[i]);
                            if (orSplit[i].Contains("*"))
                            {
                                tempExp = new RegularExpression(orSplit[i][0].ToString());
                                tempExp = tempExp.star();
                            }
                            else if (orSplit[i].Contains("+"))
                            {
                                tempExp = new RegularExpression(orSplit[i][0].ToString());
                                tempExp = tempExp.plus();
                            }
                            splitExp = splitExp.or(tempExp);
                        }
                        expressions.Add(splitExp);
                    }
                    else if (String.Compare("*", q) == 0)
                    {
                        expressions[expressions.Count - 1] = expressions[expressions.Count - 1].star();
                    }
                    else if (String.Compare("+", q) == 0)
                    {
                        expressions[expressions.Count - 1] = expressions[expressions.Count - 1].plus();
                    }
                    else
                    {
                        expressions.Add(new RegularExpression(q));
                    }
                }
            }
            RegularExpression returnValue = expressions[0];
            for (int j = 1; j < expressions.Count; j++)
            {
                returnValue = returnValue.dot(expressions[j]);
            }
            return returnValue;
        }

        public static List<string> seperateStarPlus(List<string> conjoinedArray)
        {
            List<string> seperatedArray = new List<string>();
            foreach (string z in conjoinedArray)
            {
                if (z.Contains('*'))
                {
                    seperatedArray = seperatedArray.Concat(splitOperator(z, "(\\*)")).ToList();
                }
                else if (z.Contains('+'))
                {
                    seperatedArray =  seperatedArray.Concat(splitOperator(z, "(\\+)")).ToList();
                }
                else
                {
                    seperatedArray.Add(z);
                }
            }
            return seperatedArray;
        }

        public static List<string> splitOperator(string input, string oper)
        {
            var regEx = new Regex(oper);
            string[] substrings = regEx.Split(input);
            List<string> returnList = new List<string>();
            foreach (string n in substrings)
            {
                if (!String.Equals(n, ""))
                {
                    returnList.Add(n);
                }
            }
            return returnList;
        }

        public static bool checkInput(string input)
        {
            int freqOpening = input.Count(f => (f == '('));
            int freqClosing = input.Count(f => (f == ')'));
            if (freqClosing % 2 != 0 || freqOpening % 2 != 0)
            {
                return false;
            }

            return true;
        }

        public static List<string> GetRandomWord(RegularExpression expression, List<string> alphabet)
        {
            List<string> language = expression.getLanguage(6).ToList();
            foreach (string l in language)
            {
                Console.WriteLine(l);
            }
            Console.WriteLine("");
            List<string> nonLanguage = new List<string>(0);
            string word = "";
            string tempWord = "";

            foreach (string c in alphabet)
            {
                word = word + c;
                tempWord = word;

                foreach (string t in alphabet)
                {
                    tempWord = tempWord + t;
                    if (!language.Contains(tempWord))
                    {
                        nonLanguage.Add(tempWord);
                    }
                }


            }
            return nonLanguage;
        }

        public static List<string> getAlphabet(string expression)
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

        public static List<char> getCharAlphabet(string expression)
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

        public static Tuple<Automata, string> CreateOne(Automata automata, string c, string lastState)
        {
            if(lastState != null)
            {
                string state0 = lastState;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                automata.addState(state0);
                automata.addState(state1);
                automata.addTransition(new Transition(state0, c, state1));
                
            }
            else
            {
                string state0 = "q" + Database.nextName;
                Database.nextName++;
                string state1 = "q" + Database.nextName;
                Database.nextName++;
                automata.addState(state0);
                automata.addState(state1);
                automata.addTransition(new Transition(state0, c, state1));
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state1, c, state2));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state1, c, state2));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.addTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.addTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                automata.addTransition(new Transition(state0, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.addTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                automata.addTransition(new Transition(state0, "$", state3));
                foreach (Transition t in auto2.transitions)
                {
                    automata.addTransition(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state1, c, state2));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                automata.addTransition(new Transition(state0, "$", state3));
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state1, c, state2));
                automata.addTransition(new Transition(state2, "$", state1));
                automata.addTransition(new Transition(state2, "$", state3));
                automata.addTransition(new Transition(state0, "$", state3));

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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state4, "$", state5));
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
                    automata.addState(state);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state4, "$", state5));
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
                    automata.addState(state);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state4, "$", state5));
                automata.addTransition(new Transition(state3, d, state4));
                foreach (Transition t in auto1.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto1.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state4, "$", state5));
                automata.addTransition(new Transition(state3, d, state4));
                foreach (Transition t in auto1.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach(string state in auto1.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state4, "$", state5));
                automata.addTransition(new Transition(state1, c, state2));
                foreach (Transition t in auto2.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state4, "$", state5));
                automata.addTransition(new Transition(state1, c, state2));
                foreach (Transition t in auto2.transitions)
                {
                    automata.transitions.Add(t);
                }
                foreach (string state in auto2.states)
                {
                    automata.addState(state);
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state1, c, state2));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state3, d, state4));
                automata.addTransition(new Transition(state4, "$", state5));
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
                automata.addState(state0);
                automata.addState(state1);
                automata.addState(state2);
                automata.addState(state3);
                automata.addState(state4);
                automata.addState(state5);
                automata.addTransition(new Transition(state0, "$", state1));
                automata.addTransition(new Transition(state1, c, state2));
                automata.addTransition(new Transition(state2, "$", state5));
                automata.addTransition(new Transition(state0, "$", state3));
                automata.addTransition(new Transition(state3, d, state4));
                automata.addTransition(new Transition(state4, "$", state5));

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
