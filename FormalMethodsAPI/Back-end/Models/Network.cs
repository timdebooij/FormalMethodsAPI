using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Data Class for network. It is a combination of all information that the frontend needs to generate a visnetwork
    /// </summary>
    public class Network
    {
        public int id { get; set; }
        public List<Node> nodes { get; set; }
        public List<Edge> edges { get; set; }
        public char[] alphabet { get; set; }
        public List<string> states { get; set; }
        public int status { get; set; }
        public string errorMessage { get; set; }
        public List<int> ids { get; set; }
        public bool isDfa { get; set; }

        public Network(int id, List<Node> nodes, List<Edge> edges, char[] alphabet, List<string> states, int status, string errorMessage, List<int> ids, bool isDfa)
        {
            this.id = id;
            this.nodes = nodes;
            this.edges = edges;
            this.alphabet = alphabet;
            this.states = states;
            this.status = status;
            this.errorMessage = errorMessage;
            this.ids = ids;
            this.isDfa = isDfa;
        }
    }
}
