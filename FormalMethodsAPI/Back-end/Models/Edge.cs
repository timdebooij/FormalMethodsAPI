using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Edge
    {
        public int from;
        public int to;
        public string label;
        public string arrows;

        public Edge(int from, int to, string label)
        {
            this.from = from;
            this.to = to;
            this.label = label;
            this.arrows = "to";
        }
    }
}
