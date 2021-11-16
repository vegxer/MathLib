using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MathLibrary
{
    public class ExtremumSearch
    {
        private delegate bool Compare(double num1, double num2);

        LinearFunction function;
        LinearInequalitiesSystem constraints;

        public ExtremumSearch(LinearFunction function, LinearInequalitiesSystem constraints)
        {
            CheckInputParameters(function, constraints);
            this.function = function.Clone();
            this.constraints = (LinearInequalitiesSystem)constraints.Clone();
        }

        private void CheckInputParameters(LinearFunction function, LinearInequalitiesSystem constraints)
        {
            if (function.Length != constraints.VariablesNumber)
                throw new ArgumentException("Количество переменных в целевой функции" +
                    " и системе ограничений должно совпадать");
            if (constraints.VariablesNumber > constraints.EquationsNumber)
                throw new ArgumentException("Количество переменных в системе ограничений" +
                    " должно быть не больше количества уравнений");
        }

        private double[] FindExtremum(Compare comp)
        {
            double[] extremumVertice;
            List<double[]> vertices = GetVertices();
            double extremum = function.Value(vertices[0]);
            extremumVertice = vertices[0];

            foreach (double[] vertice in vertices)
                if (comp(function.Value(vertice), extremum))
                {
                    extremum = function.Value(vertice);
                    extremumVertice = vertice;
                }

            return extremumVertice;
        }

        public double[] FindMaxVertice()
        {
            double[] extremumVertice = FindExtremum((x1, x2) => x1 > x2);
            double[] infVertice = TryFindInfinityExtremum(TryFindInfinityExtremum(extremumVertice, double.MaxValue), double.MinValue);
            if (double.IsNegativeInfinity(function.Value(infVertice)))
                return extremumVertice;
            else
                return infVertice;
        }

        public double[] FindMinVertice()
        {
            double[] extremumVertice = FindExtremum((x1, x2) => x1 < x2);
            double[] infVertice = TryFindInfinityExtremum(TryFindInfinityExtremum(extremumVertice, double.MaxValue), double.MinValue);
            if (double.IsPositiveInfinity(function.Value(infVertice)))
                return extremumVertice;
            else
                return infVertice;
        }

        public List<double[]> FindExtremums()
        {
            return new List<double[]> { FindMinVertice(), FindMaxVertice() };
        }

        private double[] TryFindInfinityExtremum(double[] extremumVertice, double infinity)
        {
            double[] infinityVertice = Enumerable.Repeat(infinity, extremumVertice.Length).ToArray();
            if (FitsConstraints(infinityVertice))
                return Enumerable.Repeat(infinity == double.MaxValue ? double.PositiveInfinity : double.NegativeInfinity,
                    extremumVertice.Length).ToArray();
            else
                return extremumVertice;
        }

        public List<double[]> GetVertices()
        {
            List<double[]> vertices = new List<double[]>();

            int n = constraints.EquationsNumber, k = constraints.VariablesNumber;
            int[] comb = new int[k + 1];

            for (int i = 0; i < k; ++i) 
                comb[i] = i;

            comb[k] = n + 2;
            int j = 0;

            int verticeNum = 0;
            while (comb[j] < n)
            {
                Console.WriteLine("Найдём {0}-ую вершину", ++verticeNum);
                LinearEquationsSystem combSystem = CreateSystem(comb);
                Console.WriteLine(combSystem.ToString());

                try
                {
                    double[] solution = combSystem.Solve();
                    string strSolution = combSystem.SolveString();
                    Console.WriteLine("Решение системы: " + strSolution);
                    Console.WriteLine("F{0} = {1}", strSolution, function.Value(solution));

                    if (FitsConstraints(solution))
                        vertices.Add(solution);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message.ToString() + "\n\n");
                }

                for (j = 0; comb[j] + 1 == comb[j + 1]; ++j)
                    comb[j] = j;
                ++comb[j];
            }

            return vertices;
        }

        private bool FitsConstraints(double[] solution)
        {
            for (int i = 0; i < constraints.EquationsNumber; ++i)
                if (!((LinearInequality)constraints[i]).IsCorrect(solution))
                {
                    if (solution[0] != double.MaxValue && solution[0] != double.MinValue)
                        Console.WriteLine("Решение системы не соответствует {0}-му ограничению\n\n", i + 1);
                    return false;
                }

            if (solution[0] != double.MaxValue && solution[0] != double.MinValue)
                Console.WriteLine("Решение системы подходит под все ограничения\n\n");
            else
                Console.WriteLine("Система ограничений не образует замкнутую область,\n" +
                    "поэтому экстремум стремится к бесконечности");
            return true;
        }

        private LinearEquationsSystem CreateSystem(int[] comb)
        {
            LinearEquationsSystem system = new LinearEquationsSystem(constraints.VariablesNumber, constraints.RoundValue - 2);

            for (int i = 0; i < comb.Length - 1; ++i)
                system.Add(new LinearEquation((LinearEquation)constraints[comb[i]].Clone()));

            return system;
        }

        public static string ExtremumVerticeToString(double[] array)
        {
            string strArray = "(";

            foreach (double x in array)
            {
                string number = x.ToString();
                if (double.IsNegativeInfinity(x))
                    number = "-inf";
                else if (double.IsPositiveInfinity(x))
                    number = "+inf";

                strArray += string.Format("{0}; ", number);
            }

            return strArray.Remove(strArray.Length - 2) + ")";
        }
    }
}
