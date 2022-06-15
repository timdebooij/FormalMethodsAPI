using FormalMethodsAPI.Back_end;
using FormalMethodsAPI.Back_end.Models;
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
        //public Dictionary<int, Automata> Automatas = new Dictionary<int, Automata>();

        [HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        public string Get(string input)
        {
            Automata m = Automata.getExampleSlide14Lesson2();
            Network network = Automata.getVisNodes(0, m);
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
            Database.getAutomatas();
            Network network = new Network(0, null, null, null, null, 300, "", Database.getIds(), false);
            return JsonConvert.SerializeObject(network);
        }

        [HttpGet("dfaId")]
        public string GetDfaIds()
        {
            Database.getAutomatas();
            Network network = new Network(0, null, null, null, null, 300, "", Database.getDfaIds(), false);
            return JsonConvert.SerializeObject(network);
        }

        [HttpGet("ndfaId")]
        public string GetNDfaIds()
        {
            Database.getAutomatas();
            Network network = new Network(0, null, null, null, null, 300, "", Database.getNdfaIds(), false);
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
                Network network = new Network(0, null, null, null, null, 406, "Fill in the alphabet for the new network", Database.getIds(), false);
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
                Database.Automatas[id].addState(state);
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
                Database.Automatas[id].addTransition(t);
                return GetNetworkString(id);
            }
        }

        [HttpGet("start/{id}/{state}")]
        public string SetStartState(int id, string state)
        {
            if (Database.Automatas.ContainsKey(id))
            {
                Database.Automatas[id].defineAsStartState(state);
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
                Database.Automatas[id].defineAsFinalState(state);
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
                    int index = Automata.TranslateToDfa(Database.Automatas[id]);
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
                    int index = Automata.MinimiseDfa(Database.Automatas[id]);
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
                    grammar.generateGrammar(Database.Automatas[id]);
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
            int stateIteration = 0;
            List<char> symbols = new List<char>();
            foreach (char c in start)
            {
                if (!(c == ' ' || c == '-' || symbols.Contains(c)))
                {
                    symbols.Add(c);
                }
            }
            foreach (char c in contain)
            {
                if (!(c == ' ' || c == '-' || symbols.Contains(c)))
                {
                    symbols.Add(c);
                }
            }
            foreach (char c in end)
            {
                if (!(c == ' ' || c == '-' || symbols.Contains(c)))
                {
                    symbols.Add(c);
                }
            }
            Automata network = new Automata(symbols.ToArray());
            string currentState = GetNextState(stateIteration);
            if (!(start == "-"))
            {
                network.defineAsStartState(GetNextState(stateIteration));
                for (int i = 0; i < start.Length; i++)
                {
                    stateIteration++;
                    network.addState(GetNextState(stateIteration));
                    network.addTransition(new Transition(currentState, start[i].ToString(), GetNextState(stateIteration)));
                    currentState = GetNextState(stateIteration);
                }
                stateIteration++;
            }
            if(!(contain == "-"))
            {
                for (int i = 0; i < contain.Length; i++)
                {

                    network.addState(GetNextState(stateIteration));
                    network.addTransition(new Transition(currentState, contain[i].ToString(), GetNextState(stateIteration)));
                    currentState = GetNextState(stateIteration);
                    stateIteration++;
                }
            }
            
            if(!(end == "-"))
            {
                for (int i = 0; i < end.Length; i++)
                {
                    network.addState(GetNextState(stateIteration));
                    network.addTransition(new Transition(currentState, end[i].ToString(), GetNextState(stateIteration)));
                    currentState = GetNextState(stateIteration);
                    stateIteration++;
                }
                network.defineAsFinalState(currentState);
            }
            
            
            Database.getAutomatas();
            Database.Automatas.Add(id, network);
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
                        List<string> nextStates = new List<string>();
                        List<string> tempStates = new List<string>();
                        foreach (string state in automata.startStates)
                        {
                            nextStates = nextStates.Concat(automata.getNextStates(word[0].ToString(), state)).ToList();
                        }
                        if(nextStates.Count == 0)
                        {
                            return getWordString(word, false);
                        }
                        for(int i = 1; i < word.Length; i++)
                        {
                            foreach(string state in nextStates)
                            {
                                tempStates = tempStates.Concat(automata.getNextStates(word[i].ToString(), state)).ToList();
                            }
                            if (tempStates.Count == 0)
                            {
                                return getWordString(word, false);
                            }
                            else
                            {
                                nextStates = tempStates;
                                tempStates = new List<string>();
                                if(i == word.Length - 1)
                                {
                                    if(automata.finalStates.Count != 0)
                                    {
                                        foreach (string state in automata.finalStates)
                                        {
                                            if (nextStates.Contains(state))
                                            {
                                                return getWordString(word, true);
                                            }
                                            else
                                            {
                                                return getWordString(word, false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return getWordString(word, true);
                                    }
                                    
                                }
                            }
                        }
                        return getWordString(word, true);
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
            Database.getAutomatas();
            if(id1 == -999 || id2 == -999)
            {
                return GetErrorString(400, "First load the DFA's before performing the operation");
            }
            else
            {
                int id = Automata.Concatenation(Database.Automatas[id1], Database.Automatas[id2]);
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
            Database.getAutomatas();
            if (id1 == -999 || id2 == -999)
            {
                return GetErrorString(400, "First load the DFA's before performing the operation");
            }
            else
            {
                int id = Automata.Union(Database.Automatas[id1], Database.Automatas[id2]);
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
            Database.getAutomatas();
            if (id == -999)
            {
                return GetErrorString(400, "First load the DFA before performing the operation");
            }
            if (Database.Automatas.ContainsKey(id))
            {
                Automata a = Automata.Complement(Database.Automatas[id]);
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
            Database.getAutomatas();
            if (input == "-")
            {
                return GetErrorString(404, "False data");
            }
            else
            {
                input = input.Replace('@', '+');
                RegularExpression exp = RegularExpression.generate(input);
                Automata a = new Automata(RegularExpression.getCharAlphabet(input).ToArray());
                Tuple<Automata, string> tuple = RegularExpression.GetNdfa(exp, a, null);
                tuple.Item1.defineAsStartState(RegularExpression.GetLowest(tuple.Item1.states.ToList()));
                tuple.Item1.defineAsFinalState(RegularExpression.GetHighest(tuple.Item1.states.ToList()));
                Database.Automatas.Add(Database.nextId, tuple.Item1);
                int index = Database.nextId;
                Database.nextId++;
                Database.nextName = 0;
                return GetNetworkString(index);

            }

        }

        public static string GetNextState(int iteration)
        {
            List<string> states = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H" };
            return states[iteration];
        }

        public string GetNetworkString(int id, int status = 200, string errorMessage = "")
        {
            Network network = Automata.getVisNodes(id, Database.Automatas[id]);
            network.status = status;
            network.errorMessage = errorMessage;
            return JsonConvert.SerializeObject(network);
        }

        public string GetErrorString(int status, string error)
        {
            Network network = new Network(0, null, null, null, null, status, error, Database.getIds(), false);
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
