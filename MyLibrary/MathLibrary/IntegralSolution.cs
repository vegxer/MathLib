using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public static class IntegralSolution
    {
        public static double ParabolaMethod(double a, double b, double h, Function f)
        {
            double integral = 0;
            int i = 1;
            for (double x = a + h; x < b; x += h, ++i)
            {
                if (i % 2 == 0)
                    integral += 2 * f.Value(x);
                else
                    integral += 4 * f.Value(x);
            }
            integral += f.Value(a) + f.Value(b);
            return integral * h / 3;
        }

        public static double TrapezoidMethod(double a, double b, double h, Function f)
        {
            double integral = 0;
            for (double x = a + h; x < b; x += h)
                integral += f.Value(x);
            integral *= 2;
            integral += f.Value(a) + f.Value(b);
            return h * integral / 2;
        }
    }
}
