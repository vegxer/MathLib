using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary.Sign
{
    public class SignMoreOrEqual : InequalitySign
    {
        public override bool Compare(double num1, double num2)
        {
            return num1 >= num2;
        }

        public override string ToString()
        {
            return ">=";
        }
    }
}
