using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Data Class for a node in a visnetwork
    /// </summary>
    public class Node
    {
        public int id;
        public string label;
        public Color color;
        public string shape;

        public Node(int id, string label, Color color, string shape)
        {
            this.id = id;
            this.label = label;
            this.color = color;
            this.shape = shape;
        }
    }
}
