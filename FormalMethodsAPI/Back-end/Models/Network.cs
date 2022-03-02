using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Network
    {
        public List<Node> nodes { get; set; }
        public List<Edge> edges { get; set; }

        public Network(List<Node> nodes, List<Edge> edges)
        {
            this.nodes = nodes;
            this.edges = edges;
        }
    }
}
