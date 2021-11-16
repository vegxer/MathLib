using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class Function
    {
        public delegate double Func(double x);
        Func function;

        public Function(Func function)
        {
            this.function = new Func(function);
        }

        public double Value(double x) => function(x);

        public double MaxValue(double a, double b, double h)
        {
            double max = function(a);

            for (double x = a + h; x <= b; x += h)
            {
                double value = function(x);
                if (value > max)
                    max = value;
            }

            return max;
        }

        public double MinValue(double a, double b, double h)
        {
            double min = function(a);

            for (double x = a + h; x <= b; x += h)
            {
                double value = function(x);
                if (value < min)
                    min = value;
            }

            return min;
        }
    }
}
