using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Helpers
{
    public class RegularExpressionHelper
    {
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

                foreach (string q in plusArray)
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
                    seperatedArray = seperatedArray.Concat(splitOperator(z, "(\\+)")).ToList();
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

        public static List<string> GetRandomWord(RegularExpression expression, List<string> alphabet)
        {
            List<string> language = expression.getLanguage(6).ToList();
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



    }
}
