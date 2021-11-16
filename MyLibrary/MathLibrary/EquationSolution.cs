using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public static class EquationSolution
    {
        public static List<Pair<double>> SeparateRoots(double start, double end, double h, Function f)
        {
            List<Pair<double>> roots = new List<Pair<double>>();
            double x1 = start, x2 = x1 + h;

            while (x2 <= end)
            {
                if (f.Value(x1) * f.Value(x2) < 0)
                    roots.Add(new Pair<double>(x1, x2));
                x1 = x2;
                x2 = x1 + h;
            }

            return roots;
        }

        public static double HalfDevisionMethod(double x1, double x2, double h, double eps, Function f)
        {
            double delta = h;

            while (x2 - x1 > eps)
            {
                double mid = (x1 + x2) / 2;

                if (x2 - x1 <= delta)
                    delta *= 0.1;

                if (f.Value(x1) * f.Value(mid) < 0)
                    x2 = mid;
                else
                    x1 = mid;
            }

            return (x1 + x2) / 2;
        }

        /// <summary>
        /// f(x) = 0 => 
        /// Выразить их этой функции x => 
        /// x = phi(x)
        /// </summary>
        public static double SimpleIteration(double a, double b, double e, Function phi, Function absPhi)
        {
            double M1 = absPhi.MaxValue(a, b, Math.Pow(e, 0.25));

            if (0 < M1 && M1 < 1)
            {
                double y = phi.Value((a + b) / 2), p = Math.Abs((a + b) / 2 - y), x = y;
                if (M1 > 0.5)
                    e = e * (1 - M1) / M1;

                while (p > e)
                {
                    y = phi.Value(x);
                    p = Math.Abs(x - y);
                    x = y;
                }

                return x;
            }
            else
                throw new ArgumentException("Функция не является сжимающей на отрезке");
        }

        public static double TangentMethod(double a, double b, double e, Function f, Function AbsDerivative1st, Function AbsDerivative2nd)
        {
            double m1 = AbsDerivative1st.MinValue(a, b, Math.Pow(e, 0.25));
            double M2 = AbsDerivative2nd.MaxValue(a, b, Math.Pow(e, 0.25));
            double x = f.Value(a) * AbsDerivative2nd.Value(a) > 0 ? a : b;
            if (m1 != 0 && M2 != 0)
                e = Math.Sqrt(2 * m1 * e / M2);

            double y = x;
            do
            {
                x = y;
                y = x - f.Value(x) / AbsDerivative1st.Value(x);
            }
            while (Math.Abs(y - x) >= e);

            return y;
        }

        public static double ChordMethod(double a, double b, double e, Function f, Function AbsDerivative1st, Function AbsDerivative2nd)
        {
            double m1 = AbsDerivative1st.MinValue(a, b, Math.Pow(e, 0.25));
            double M1 = AbsDerivative1st.MaxValue(a, b, Math.Pow(e, 0.25));
            double N = f.Value(a) * AbsDerivative2nd.Value(a) > 0 ? a : b;
            double x0 = N == a ? b : a;
            if (m1 != 0 && M1 - m1 != 0)
                e = m1 * e / (M1 - m1);

            double x1 = x0;
            do
            {
                x0 = x1;
                x1 = x0 - f.Value(x0) * (N - x0) / (f.Value(N) - f.Value(x0));
            }
            while (Math.Abs(x0 - x1) >= e);

            return x1;
        }
    }
}
