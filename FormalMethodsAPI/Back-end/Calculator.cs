using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormalMethodsAPI.Back_end
{
    public class Calculator
    {

        public string getString()
        {
            string r = "henlo, the number is: ";
            return r + getNumbers(3, 8);
        }

        public int getNumbers(int n1, int n2)
        {
            return n1 * n2;
        }
    }
}
