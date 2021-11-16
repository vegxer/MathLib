using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public static class HeatConductionEquationSolution
    {
        public static double[,] Solve(double h, double a, double E, double L, Function zeroTimeStartCondition, Function zeroXStartCondition, Function endXStartCondition)
        {
            double l = Math.Round(h * h / (2 * a * a), 10);
            double[,] net = new double[(int)(E / l) + 1, (int)Math.Round(L / h, 10) + 1];
            int n = net.GetLength(0);
            StartCondition(net, h, l, zeroTimeStartCondition, zeroXStartCondition, endXStartCondition);

            for (double t = l; t <= E + 1e-10; t += l)
                for (double x = h; x < L; x += h)
                    net[n - (int)Math.Round(t / l, 10) - 1, (int)Math.Round(x / h, 10)]
                        = 0.5 * (net[n - (int)Math.Round(t / l, 10), (int)Math.Round(x / h, 10) + 1] + net[n - (int)Math.Round(t / l, 10), (int)Math.Round(x / h, 10) - 1]);

            return net;
        }

        private static void StartCondition(double[,] net, double h, double l, Function f, Function phi, Function psi)
        {
            int n = net.GetLength(0), m = net.GetLength(1);

            for (int j = 0; j < m; ++j)
                net[n - 1, j] = f.Value(j * h);

            for (int i = 0; i < n; ++i)
                net[n - i - 1, 0] = phi.Value(i * l);

            for (int i = 0; i < n; ++i)
                net[n - i - 1, m - 1] = psi.Value(i * l);
        }

        public static void PrintSolution(double[,] array, int roundValue, double h, double a)
        {
            double l = Math.Round(h * h / (2 * a * a), 10);
            int n = array.GetLength(0), m = array.GetLength(1);
            for (int i = 0; i < n; ++i)
            {
                string tab = "";
                if (i == 0 || i == n - 1)
                    tab = "\t";
                Console.Write("t = {0}{1}\t|", (n - i - 1) * l, tab);
                for (int j = 0; j < m; ++j)
                    Console.Write("{0,10}", Math.Round(array[i, j], roundValue));
                Console.WriteLine("\n");
            }

            for (int j = 0; j < m * 13; ++j)
            {
                Console.Write("-");
            }

            Console.WriteLine();
            Console.Write("\t\tx");

            for (int j = 0; j < m; ++j)
            {
                Console.Write("{0,10}", j * h);
            }
        }
    }
}
