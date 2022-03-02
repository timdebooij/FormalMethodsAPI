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

        public Node(int id, string label)
        {
            this.id = id;
            this.label = label;
        }
    }
}
