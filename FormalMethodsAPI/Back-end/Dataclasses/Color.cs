using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end.Models
{
    /// <summary>
    /// Data Class for storing the color of a visnode
    /// </summary>
    public class Color
    {
        public string border;
        public string background;

        public Color(string border, string background)
        {
            this.border = border;
            this.background = background;
        }
    }
}
