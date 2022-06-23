using FormalMethodsAPI.Back_end;
using FormalMethodsAPI.Back_end.Helpers;
using FormalMethodsAPI.Back_end.Models;
using FormalMethodsAPI.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Networkcontroller : ControllerBase
    {
        [HttpGet]
        public string Get(string input)
        {
            Automata m = AutomataHelper.GetExampleSlide14Lesson2();
            Network network = AutomataHelper.GetVisNodes(0, m);
            var jsonBoth = JsonConvert.SerializeObject(network);
            return jsonBoth;
        }

        [HttpGet("{id}")]
        public string GetAutomata(int id)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                return GetNetworkString(id);
            }
            else
            {
                return GetErrorString(404, "Given key not found");
            }
            
        }

        [HttpGet("id")]
        public string GetIds()
        {
            Database.GetAutomatas();
            Network network = new Network(0, null, null, null, null, 300, "", Database.GetIds(), false);
            return JsonConvert.SerializeObject(network);
        }

        [HttpGet("dfaId")]
        public string GetDfaIds()
        {
            Database.GetAutomatas();
            Network network = new Network(0, null, null, null, null, 300, "", Database.GetDfaIds(), false);
            return JsonConvert.SerializeObject(network);
        }

        [HttpGet("ndfaId")]
        public string GetNDfaIds()
        {
            Database.GetAutomatas();
            Network network = new Network(0, null, null, null, null, 300, "", Database.GetNdfaIds(), false);
            return JsonConvert.SerializeObject(network);
        }

        [HttpGet("create/{id}/{alphabet}")]
        public string CreateNewAutomata(int id, string alphabet)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                return GetErrorString(406, "Given key already exists");
            }
            else if(string.Equals(alphabet, "-"))
            {
                Network network = new Network(0, null, null, null, null, 406, "Fill in the alphabet for the new network", Database.GetIds(), false);
                return JsonConvert.SerializeObject(network);
            }
            else
            {
                char[] symbols = new char[alphabet.Length];
                for(int i = 0; i <alphabet.Length; i++)
                {
                    symbols[i] = alphabet[i];
                }
                Database.Automatas.Add(id, new Automata(symbols));
                return GetNetworkString(id);
            }
        }

        [HttpGet("addState/{id}/{state}")]
        public string AddState(int id, string state)
        {
            if (state.Length == 0 || string.Equals(state, "-"))
            {
                return GetErrorString(406, "Empty state is not allowed");
            }
            if (Database.Automatas.ContainsKey(id))
            {
                Database.Automatas[id].AddState(state);
                return GetNetworkString(id);
            }
            else
            {
                return GetErrorString(404, "Given key not found");
            }
        }

        [HttpGet("add/{id}/{from}/{symbol}/{to}")]
        public string AddTransition(int id, string from, string symbol, string to)
        {
            if (!Database.Automatas.ContainsKey(id))
            {
                return GetErrorString(404, "Given key not found");
            }
            else if(from.Length == 0 || symbol.Length == 0|| to.Length == 0|| string.Equals(from, "-") || string.Equals(symbol, "-")|| string.Equals(to, "-"))
            {
                return GetNetworkString(id, 406, "Choose states and a letter");
            }
            else
            {
                Transition t = new Transition(from, symbol, to);
                Database.Automatas[id].AddTransition(t);
                return GetNetworkString(id);
            }
        }

        [HttpGet("start/{id}/{state}")]
        public string SetStartState(int id, string state)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                Database.Automatas[id].DefineAsStartState(state);
                return GetNetworkString(id);
            }
            else
            {
                return GetErrorString(404, "Given key not found");
            }
        }

        [HttpGet("end/{id}/{state}")]
        public string SetEndState(int id, string state)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                Database.Automatas[id].DefineAsFinalState(state);
                return GetNetworkString(id);
            }
            else
            {
                return GetErrorString(404, "Given key not found");
            }
        }

        [HttpGet("todfa/{id}")]
        public string ToDfa(int id)
        {
            if(id == -999)
            {
                return GetErrorString(404, "First load the Ndfa before trying to convert");
            }
            else
            {
                if (Database.Automatas.ContainsKey(id))
                {
                    Database.transStates = new List<NewState>();
                    Database.nameIndex = 0;
                    int index = AutomataHelper.TranslateToDfa(Database.Automatas[id]);
                    return GetNetworkString(index);
                }
                else
                {
                    return GetErrorString(404, "Given key not found");
                }
            }
            
        }

        [HttpGet("minimise/{id}")]
        public string MinimiseDfa(int id)
        {
            if (id == -999)
            {
                return GetErrorString(404, "First load the Dfa before trying to minimise");
            }
            else
            {
                if (Database.Automatas.ContainsKey(id))
                {
                    int index = AutomataHelper.MinimiseDfa(Database.Automatas[id]);
                    return GetNetworkString(index);
                }
                else
                {
                    return GetErrorString(404, "Given key not found");
                }
            }
        }

        [HttpGet("togrammar/{id}")]
        public string GetGrammar(int id)
        {
            if (id == -999)
            {
                return GetErrorString(404, "First load the Dfa before trying to load the grammar");
            }
            else
            {
                if (Database.Automatas.ContainsKey(id))
                {
                    Grammar grammar = new Grammar();
                    grammar.GenerateGrammar(Database.Automatas[id]);
                    return JsonConvert.SerializeObject(grammar);
                }
                else
                {
                    return GetErrorString(404, "Given key not found");
                }
            }
        }

        [HttpGet("reset")]
        public string ResetApi()
        {
            Database.ResetDatabase();
            return "Database succesfully reset";
        }

        [HttpGet("construct/{id}/{start}/{contain}/{end}")]
        public string ConstructNetwork(int id, string start, string contain, string end)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                return GetErrorString(406, "Given key already exists");
            }
            AutomataHelper.ConstructNetwork(id, start, contain, end);
            return GetNetworkString(id);
        }

        [HttpGet("check/{id}/{word}")]
        public string CheckWord(int id, string word)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                Automata automata = Database.Automatas[id];
                if(!(word.Length == 0))
                {
                    if (automata.startStates.Count != 0)
                    {
                        bool result = AutomataHelper.CheckWord(id, word);
                        return getWordString(word, result);
                    }
                    else
                    {
                        return GetWordErrorString("DFA/NDFA has no start state, provide at least one", word);
                    }
                }
                else
                {
                    return GetWordErrorString("Cannot check an empty string", word);
                }
                
            }
            else
            {
                return GetWordErrorString("Given key was not found", word);
            }
        }

        [HttpGet("concat/{id1}/{id2}")]
        public string ConcatDfa(int id1, int id2)
        {
            Database.GetAutomatas();
            if(id1 == -999 || id2 == -999)
            {
                return GetErrorString(400, "First load the DFA's before performing the operation");
            }
            else
            {
                int id = AutomataHelper.Concatenation(Database.Automatas[id1], Database.Automatas[id2]);
                if(id == -9090)
                {
                    Database.newStates = new List<NewState>();
                    Database.nextId = 0;
                    return GetErrorString(400, "DFA's not suitable for concatenation");
                }
                return GetNetworkString(id);
            }
        }

        [HttpGet("union/{id1}/{id2}")]
        public string UnionDfa(int id1, int id2)
        {
            Database.GetAutomatas();
            if (id1 == -999 || id2 == -999)
            {
                return GetErrorString(400, "First load the DFA's before performing the operation");
            }
            else
            {
                int id = AutomataHelper.Union(Database.Automatas[id1], Database.Automatas[id2]);
                if (id == -9090)
                {
                    Database.newStates = new List<NewState>();
                    Database.nextId = 0;
                    return GetErrorString(400, "DFA's not suitable for union");
                }
                return GetNetworkString(id);
            }
        }

        [HttpGet("comp/{id}")]
        public string ComplementDfa(int id)
        {
            Database.GetAutomatas();
            if (id == -999)
            {
                return GetErrorString(400, "First load the DFA before performing the operation");
            }
            if (Database.Automatas.ContainsKey(id))
            {
                Automata a = AutomataHelper.Complement(Database.Automatas[id]);
                int index = Database.nextCompId;
                Database.Automatas.Add(index, a);
                Database.nextCompId++;
                return GetNetworkString(index);
            }
            else
            {
                return GetErrorString(404, "Given key was not found");
            }
        }

        [HttpGet("thompson/{input}")]
        public string Thompson(string input)
        {
            Database.GetAutomatas();
            if (input == "-")
            {
                return GetErrorString(404, "False data");
            }
            else
            {
                input = input.Replace('@', '+');
                RegularExpression exp = RegularExpressionHelper.Generate(input);
                Automata a = new Automata(RegularExpression.GetCharAlphabet(input).ToArray());
                Tuple<Automata, string> tuple = RegularExpression.GetNdfa(exp, a, null);
                tuple.Item1.DefineAsStartState(RegularExpression.GetLowest(tuple.Item1.states.ToList()));
                tuple.Item1.DefineAsFinalState(RegularExpression.GetHighest(tuple.Item1.states.ToList()));
                Database.Automatas.Add(Database.nextId, tuple.Item1);
                int index = Database.nextId;
                Database.nextId++;
                Database.nextName = 0;
                return GetNetworkString(index);

            }

        } 

        [HttpGet("dfalanguage/{id}")]
        public string DFALanguage(int id)
        {
            Database.GetAutomatas();
            if (Database.Automatas.ContainsKey(id))
            {
                RegExpData data = new RegExpData();
                data.language = AutomataHelper.GetLanguage(Database.Automatas[id]);
                return JsonConvert.SerializeObject(data);
            }
            else
            {
                return GetErrorString(404, "Given key was not found");
            }
        }

        public string GetNetworkString(int id, int status = 200, string errorMessage = "")
        {
            Network network = AutomataHelper.GetVisNodes(id, Database.Automatas[id]);
            network.status = status;
            network.errorMessage = errorMessage;
            return JsonConvert.SerializeObject(network);
        }

        public string GetErrorString(int status, string error)
        {
            Network network = new Network(0, null, null, null, null, status, error, Database.GetIds(), false);
            return JsonConvert.SerializeObject(network);
        }

        public string getWordString(string word, bool inLanguage)
        {
            return JsonConvert.SerializeObject(new WordFormat(inLanguage, "", word));
        }

        public string GetWordErrorString(string error, string word)
        {
            return JsonConvert.SerializeObject(new WordFormat(false, error, word));
        }
    }
}
