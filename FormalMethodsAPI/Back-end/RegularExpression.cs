using System;
using System.Collections.Generic;
using System.Linq;
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
            RegularExpression expression = null;
            string op = "";
            if (!input.Contains('('))
            {
                Console.WriteLine("Detected 1 expression statement");
                expression = new RegularExpression(input);
                return expression;
            }
            else
            {
                string[] arr = input.Split(')', '(');
                bool newExp = true;
                foreach (string s in arr)
                {

                    if (s is not "")
                    {
                        Console.WriteLine("s is: " + s);
                        if (s.Contains("|"))
                        {
                            int freq = s.Count(f => (f == '|'));
                            string[] arr2 = s.Split("|");
                            List<RegularExpression> expressions = new List<RegularExpression>();
                            foreach (string terminal in arr2)
                            {
                                string insert = terminal.Trim();
                                expressions.Add(new RegularExpression(insert));
                            }
                            Console.WriteLine("Expression 0: " + expressions[0].terminals);
                            for (int i = 1; i <= freq; i++)
                            {
                                expressions[0] = expressions[0].or(expressions[i]);
                                Console.WriteLine("Expression " + i + ": " + expressions[i].terminals);
                            }
                            if (expression == null)
                            {
                                expression = expressions[0];
                            }
                            else
                            {
                                Console.WriteLine("Added a dot");
                                newExp = false;
                                expression = expression.dot(expressions[0]);
                            }
                        }
                        else if (s.Contains("*"))
                        {
                            if (expression.right is not null && !newExp)
                            {
                                expression.right = expression.right.star();
                            }
                            else
                            {
                                expression = expression.star();
                            }
                            Console.WriteLine("Added a star");

                        }
                        else if (s.Contains("+"))
                        {
                            if (expression.right is not null && !newExp)
                            {
                                expression.right = expression.right.plus();
                            }
                            else
                            {
                                expression = expression.plus();
                            }
                            Console.WriteLine("Added a plus");

                        }
                        else
                        {
                            expression = new RegularExpression(s);
                        }
                    }
                }
                 return expression;
            }
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
    }
}
