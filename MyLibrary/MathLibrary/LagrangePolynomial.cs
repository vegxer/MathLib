using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class LagrangePolynomial
    {
        Polynomial LagrangePolynom;
        int roundValue;
        List<Pair<double>> nodes;

        public LagrangePolynomial(int roundValue)
        {
            nodes = new List<Pair<double>>();
            this.roundValue = roundValue + 2;
        }

        public LagrangePolynomial(int roundValue, List<Pair<double>> nodes) : this(roundValue)
        {
            for (int i = 0; i < nodes.Count; ++i)
                this.nodes.Add(new Pair<double>(nodes[i][0], nodes[i][1]));

            LagrangePolynom = CalculateLagrangePolynomial();
        }

        public void AddNode(double x, double y)
        {
            Pair<double> addNode = new Pair<double>(x, y);
            if (!nodes.Contains(addNode))
            {
                nodes.Add(addNode);
                LagrangePolynom = CalculateLagrangePolynomial();
            }  
        }

        private Polynomial Li(int j)
        {
            Polynomial polynom = new Polynomial(nodes.Count - 1);
            polynom[polynom.Degree] = 1;
            for (int i = 1; i <= polynom.Degree; ++i)
            {
                polynom[polynom.Degree - i] = GetCoeff(polynom.Degree, i, j);
                if (i % 2 == 1)
                    polynom[polynom.Degree - i] *= -1;
            }

            double denomerator = 1;
            for (int i = 0; i <= polynom.Degree; ++i)
                if (i != j)
                    denomerator = Math.Round(denomerator * (nodes[j][0] - nodes[i][0]), roundValue);

            return polynom / denomerator;
        }

        private Polynomial CalculateLagrangePolynomial()
        {
            Polynomial PLagrange = new Polynomial(nodes.Count - 1);
            for (int i = 0; i < nodes.Count; ++i)
                PLagrange += nodes[i][1] * Li(i);
            return PLagrange;
        }

        private double GetCoeff(int n, int k, int p)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i <= n; ++i)
                indexes.Add(i);
            indexes.RemoveAt(p);
            double coeff = 0;
            int[] comb = new int[k + 1]; //сочетание

            for (int i = 0; i < k; ++i) //заполнение массива от 0 до k - 1
                comb[i] = i;
            comb[k] = n + 2; //барьер

            int j = 0;
            while (comb[j] < n) //пока элемент сочетания не больше максимального элемента в исходном множестве
            {
                double value = 1;
                for (int i = 0; i < k; ++i)
                    value = Math.Round(value * nodes[indexes[comb[i]]][0], roundValue);
                coeff = Math.Round(coeff + value, roundValue);

                for (j = 0; comb[j] + 1 == comb[j + 1]; ++j) //если условие всё время выполняется, то цикл упирается в барьер и завершается
                    comb[j] = j;
                ++comb[j];
            }
            return coeff;
        }

        public double L(double x)
        {
            return LagrangePolynom.Value(x);
        }

        public override string ToString()
        {
            return LagrangePolynom.ToString();
        }

        public Polynomial FindDerivative()
        {
            return LagrangePolynom.FindDerivative();
        }
    }
}
