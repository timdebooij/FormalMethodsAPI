using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    public class Node
    {
        public int id;
        public string label;
        public Color color;

        public Node(int id, string label, Color color)
        {
            this.id = id;
            this.label = label;
            this.color = color;
        }
    }
}
