using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class LinearEquationsSystem
    {
        protected int equationsNumber, variablesNumber = -1, roundValue;
        protected double determinant = 1;
        protected List<LinearEquation> LES;

        public LinearEquationsSystem(int variablesNumber, int roundValue)
        {
            if (variablesNumber > 0)
            {
                this.roundValue = roundValue + 2;
                LES = new List<LinearEquation>();
                this.variablesNumber = variablesNumber;
            }
            else
                throw new ArgumentOutOfRangeException("Количество переменных должно быть больше нуля");
        }

        public LinearEquationsSystem(List<LinearEquation> linearEquations, int roundValue)
        {
            if (IsSameNumOfVariables(linearEquations))
            {
                this.roundValue = roundValue + 2;
                equationsNumber = 0;
                variablesNumber = linearEquations[0].Length;
                CloneList(linearEquations);
            }
            else
                throw new ArgumentException("Количество переменных во всех уравнениях должно быть одинаково");
        }

        public LinearEquationsSystem(int roundValue)
        {
            equationsNumber = 0;
            this.roundValue = roundValue + 2;
            LES = new List<LinearEquation>();
        }

        private void CloneList(List<LinearEquation> linearEquations)
        {
            LES = new List<LinearEquation>();

            foreach (LinearEquation equation in linearEquations)
                Add((LinearEquation)equation.Clone());
        }

        public void ToDiagonal()
        {
            if (variablesNumber > equationsNumber)
                throw new Exception("Система имеет бесконечное количество решений");
            //прямой ход
            for (int x = 0, y = 0; x < variablesNumber && y < equationsNumber - 1; ++x, ++y)
            {
                determinant = Math.Round(determinant * LES[y][x], roundValue);
                if (LES[y][x] == 0 && !AddEquation(x, y))
                    continue;
                for (int i = y + 1; i < equationsNumber; ++i)
                {
                    double value = LES[i][x] / LES[y][x];
                    for (int j = x; j < variablesNumber; ++j)
                        LES[i][j] = Math.Round(LES[i][j] - value * LES[y][j], roundValue);
                    LES[i].FreeCoeff = Math.Round(LES[i].FreeCoeff - value * LES[y].FreeCoeff, roundValue);
                    if (LES[i].IsEmpty())
                        Remove(i--);
                    if (!LinearEquation.IsCoeffsCorrect(LES[i]))
                        throw new ArgumentException("Система линейных уравнений не имеет решений");
                }
                LES[y].FreeCoeff = Math.Round(LES[y].FreeCoeff / LES[y][x], roundValue);
                for (int i = LES[y].Length - 1; i >= x; --i)
                    LES[y][i] = Math.Round(LES[y][i] / LES[y][x], roundValue);
            }
            determinant = Math.Round(determinant * LES[equationsNumber - 1][variablesNumber - 1], roundValue);
            LES[equationsNumber - 1].FreeCoeff = Math.Round(LES[equationsNumber - 1].FreeCoeff / LES[equationsNumber - 1][variablesNumber - 1], roundValue);
            LES[equationsNumber - 1][variablesNumber - 1] = 1;
            

            //обратный ход
            for (int x = variablesNumber - 1, y = equationsNumber - 2; x >= 0 && y >= 0; --x, --y)
            {
                for (int i = y; i >= 0; --i)
                {
                    LES[i].FreeCoeff = Math.Round(LES[i].FreeCoeff - LES[y + 1].FreeCoeff * LES[i][x], roundValue);
                    LES[i][x] = 0;
                }
            }
        }

        public double[] Solve()
        {
            ToDiagonal();
            
            for (int i = 0; i < LES.Count; ++i)
                if (!LinearEquation.IsCoeffsCorrect(LES[i]))
                    throw new ArgumentException("Система линейных уравнений не имеет решений");
            if (variablesNumber > equationsNumber)
                throw new Exception("Система имеет бесконечное количество решений");

            double[] solution = new double[equationsNumber];
            for (int i = 0; i < equationsNumber; ++i)
                solution[i] = Math.Round(LES[i].FreeCoeff, roundValue - 3);
            return solution;
        }

        public virtual LinearEquationsSystem Clone()
        {
            LinearEquationsSystem copy = new LinearEquationsSystem(variablesNumber, roundValue - 2);

            foreach (LinearEquation equation in LES)
                copy.Add((LinearEquation)equation.Clone());

            return copy;
        }

        private LinearEquationsSystem ToReduced()
        {
            LinearEquationsSystem res = new LinearEquationsSystem(VariablesNumber, roundValue - 2);

            for (int i = 0; i < EquationsNumber; ++i)
            {
                res.Add(new LinearEquation(VariablesNumber));
                for (int j = 0; j < VariablesNumber; ++j)
                    res[i][j] = i == j ? 0 : Math.Round(-LES[i][j] / LES[i][i], roundValue);
                res[i].FreeCoeff = Math.Round(LES[i].FreeCoeff / LES[i][i], roundValue);
            }

            return res;
        }

        public double[] IterativeMethodRo1(double eps)
        {
            LinearEquationsSystem A = ToReduced();
            double alpha = A.GetAlphaRo1();
            LinearEquation B = A.GetFreeCoeffs();
            LinearEquation X0 = B;
            LinearEquation X1 = A * X0 + B;
            while (A.Ro1(X1, X0) >= eps * (1 - alpha) / alpha)
            {
                X0 = X1;
                X1 = A * X0 + B;
            }

            return X1;
        }

        public double[] IterativeMethodRo2(double eps)
        {
            LinearEquationsSystem A = ToReduced();
            double alpha = A.GetAlphaRo2();
            LinearEquation B = A.GetFreeCoeffs();
            LinearEquation X0 = B;
            LinearEquation X1 = A * X0 + B;
            while (A.Ro2(X1, X0) >= eps * (1 - alpha) / alpha)
            {
                X0 = X1;
                X1 = A * X0 + B;
            }

            return X1;
        }

        public double[] IterativeMethodRo3(double eps)
        {
            LinearEquationsSystem A = ToReduced();
            double alpha = A.GetAlphaRo3();
            LinearEquation B = A.GetFreeCoeffs();
            LinearEquation X0 = B;
            LinearEquation X1 = A * X0 + B;
            while (A.Ro3(X1, X0) >= eps * (1 - alpha) / alpha)
            {
                X0 = X1;
                X1 = A * X0 + B;
            }

            return X1;
        }

        private double GetAlphaRo1()
        {
            double max = double.MinValue;
            for (int i = 0; i < EquationsNumber; ++i)
            {
                double sum = 0;
                for (int j = 0; j < VariablesNumber; ++j)
                    sum += Math.Abs(this[i][j]);
                if (sum > max)
                    max = sum;
            }
            return max;
        }

        private double GetAlphaRo2()
        {
            double max = double.MinValue;
            for (int j = 0; j < EquationsNumber; ++j)
            {
                double sum = 0;
                for (int i = 0; i < VariablesNumber; ++i)
                    sum += Math.Abs(this[i][j]);
                if (sum > max)
                    max = sum;
            }
            return max;
        }

        private double GetAlphaRo3()
        {
            double sum = 0;
            for (int i = 0; i < EquationsNumber; ++i)
                for (int j = 0; j < VariablesNumber; ++j)
                    sum += this[i][j] * this[i][j];
            return Math.Sqrt(sum);
        }

        private double Ro1(LinearEquation equation1, LinearEquation equation2)
        {
            if (equation1.Length == equation2.Length)
            {
                double max = Math.Abs(equation1[0] - equation2[0]);
                for (int i = 1; i < equation1.Length; ++i)
                    if (Math.Abs(equation1[i] - equation2[i]) > max)
                        max = Math.Abs(equation1[i] - equation2[i]);
                return max;
            }
            else
                throw new ArgumentException("Векторы должны быть одинакового размера");
        }

        private double Ro2(LinearEquation equation1, LinearEquation equation2)
        {
            if (equation1.Length == equation2.Length)
            {
                double sum = 0;
                for (int i = 0; i < equation1.Length; ++i)
                    sum += Math.Abs(equation1[i] - equation2[i]);
                return sum;
            }
            else
                throw new ArgumentException("Векторы должны быть одинакового размера");
        }

        private double Ro3(LinearEquation equation1, LinearEquation equation2)
        {
            if (equation1.Length == equation2.Length)
            {
                double sum = 0;
                for (int i = 0; i < equation1.Length; ++i)
                    sum += (equation1[i] - equation2[i]) * (equation1[i] - equation2[i]);
                return Math.Sqrt(sum);
            }
            else
                throw new ArgumentException("Векторы должны быть одинакового размера");
        }

        private LinearEquation GetFreeCoeffs()
        {
            LinearEquation B = new LinearEquation(EquationsNumber - 1);
            for (int i = 0; i < EquationsNumber - 1; ++i)
                B[i] = this[i].FreeCoeff;
            B.FreeCoeff = this[EquationsNumber - 1].FreeCoeff;
            return B;
        }

        public static LinearEquationsSystem operator +(LinearEquationsSystem system1, LinearEquationsSystem system2)
        {
            if (system1.EquationsNumber == system2.EquationsNumber && system1.VariablesNumber == system2.VariablesNumber)
            {
                for (int i = 0; i < system1.EquationsNumber; ++i)
                {
                    system1[i] += system2[i];
                    system1[i].FreeCoeff += system2[i].FreeCoeff;
                }

                return system1;
            }
            else
                throw new ArgumentException("Количество переменных и количество уравнений должно быть одинаково");
        }

        public static LinearEquationsSystem operator -(LinearEquationsSystem system1, LinearEquationsSystem system2)
        {
            if (system1.EquationsNumber == system2.EquationsNumber && system1.VariablesNumber == system2.VariablesNumber)
            {
                for (int i = 0; i < system1.EquationsNumber; ++i)
                {
                    system1[i] -= system2[i];
                    system1[i].FreeCoeff -= system2[i].FreeCoeff;
                }

                return system1;
            }
            else
                throw new ArgumentException("Количество переменных и количество уравнений должно быть одинаково");
        }

        public static LinearEquation operator *(LinearEquationsSystem system, LinearEquation equation)
        {
            LinearEquation result = new LinearEquation(equation.Length);
            for (int i = 0; i < system.EquationsNumber - 1; ++i)
            {
                for (int j = 0; j < equation.Length; ++j)
                    result[i] += Math.Round(system[i][j] * equation[j], system.RoundValue);
                result[i] += Math.Round(system[i][system.VariablesNumber - 1] * equation.FreeCoeff, system.RoundValue);
            }
            for (int i = 0; i < equation.Length; ++i)
                result.FreeCoeff += Math.Round(system[system.EquationsNumber - 1][i] * equation[i], system.RoundValue);
            result.FreeCoeff += Math.Round(system[system.EquationsNumber - 1][system.VariablesNumber - 1] * equation.FreeCoeff, system.RoundValue);
            return result;
        }

        public static LinearEquation operator *(LinearEquation equation, LinearEquationsSystem system)
        {
            LinearEquation result = new LinearEquation(equation.Length);
            for (int i = 0; i < system.EquationsNumber - 1; ++i)
            {
                for (int j = 0; j < equation.Length; ++j)
                    result[i] += Math.Round(system[i][j] * equation[j], system.RoundValue);
                result[i] += Math.Round(system[i][system.VariablesNumber - 1] * equation.FreeCoeff, system.RoundValue);
            }
            for (int i = 0; i < equation.Length; ++i)
                result.FreeCoeff += Math.Round(system[system.EquationsNumber - 1][i] * equation[i], system.RoundValue);
            result.FreeCoeff += Math.Round(system[system.EquationsNumber - 1][system.VariablesNumber - 1] * equation.FreeCoeff, system.RoundValue);
            return result;
        }

        public int RoundValue
        {
            get
            {
                return roundValue;
            }
            set
            {
                roundValue = value;
            }
        }

        public void Discrepacies(List<double> solution)
        {
            for (int i = 0; i < LES.Count; ++i)
            {
                double lineValue = 0;
                Console.Write("b{0}' = ", i + 1);
                for (int j = 0; j < LES[i].Length; ++j)
                {
                    Console.Write(CutTheNum(LES[i][j], roundValue) + "*" + CutTheNum(solution[j], roundValue));
                    if (j != variablesNumber - 1)
                        Console.Write(" + ");
                    lineValue = Math.Round(lineValue + LES[i][j] * solution[j], roundValue);
                }
                Console.WriteLine(" = {0}", CutTheNum(lineValue, roundValue));
                Console.WriteLine("b{0} - b{1}' = {2}\n", i + 1, i + 1, CutTheNum(Math.Round(LES[i].FreeCoeff - lineValue, roundValue), roundValue));
            }
        }

        public double[,] GetInverseMatrix()
        {
            double[,] inverseMatrix = new double[LES.Count, variablesNumber];
            for (int i = 0; i < LES.Count; ++i)
            {
                LinearEquationsSystem system = Clone();
                ReplaceFreeCoeffs(system, i);
                double[] solution = system.Solve();
                for (int j = 0; j < solution.Length; ++j)
                    inverseMatrix[j, i] = solution[j];
            }
            return inverseMatrix;
        }

        private void ReplaceFreeCoeffs(LinearEquationsSystem LES, int index)
        {
            for (int i = 0; i < LES.EquationsNumber; ++i)
            {
                if (i == index)
                    LES[i].FreeCoeff = 1;
                else
                    LES[i].FreeCoeff = 0;
            }
        }

        public bool AddEquation(int x, int y)
        {
            for (int i = y + 1; i < equationsNumber; ++i)
                if (LES[i][x] != 0)
                {
                    LES[y] += LES[i];
                    for (int j = 0; j < LES[y].Length; ++j)
                        LES[y][j] = Math.Round(LES[y][j], roundValue);
                    return true;
                }
            return false;
        }

        public void Add(LinearEquation linearEquation)
        {
            if (linearEquation.Length == variablesNumber)
            {
                LES.Add(linearEquation);
                ++equationsNumber;
            }
            else
                throw new ArgumentException("Количество переменных во всех уравнениях должно быть одинаково");
        }

        public void Remove(int index)
        {
            if (index >= 0 && index < equationsNumber)
            {
                LES.RemoveAt(index);
                --equationsNumber;
            }
            else
                throw new IndexOutOfRangeException();
        }

        public string SolveString()
        {
            double[] solution = Solve();
            string strSolution = "(";

            foreach (double x in solution)
                strSolution += string.Format("{0}; ", x);

            return strSolution.Remove(strSolution.Length - 2) + ")";
        }

        private bool IsSameNumOfVariables(List<LinearEquation> linearEquations)
        {
            int n = linearEquations[0].Length;
            for (int i = 1; i < linearEquations.Count; ++i)
                if (linearEquations[i].Length != n)
                    return false;
            return true;
        }

        public LinearEquation this[int index]
        {
            get
            {
                if (index >= 0 && index < equationsNumber)
                    return LES[index];
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < equationsNumber)
                {
                    if (value.Length == variablesNumber)
                        LES[index] = value;
                    else
                        throw new ArgumentException("Количество переменных во всех уравнениях должно быть одинаково");
                }
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public double Determinant
        {
            get
            {
                ToDiagonal();
                return determinant;
            }
        }

        public override string ToString()
        {
            string system = "";

            for (int i = 0; i < LES.Count; ++i)
            {
                if (i == 0)
                    system += " ╔ ";
                else if (i == LES.Count - 1)
                    system += " ╚ ";
                else
                    system += " ║ ";

                system += LES[i].ToString() + "\n";
            }

            return system;
        }

        private string CutTheNum(double x, int decDigits)
        {
            string X = x.ToString();
            if ((int)x == x)
                X += ',';
            string res = ((int)x).ToString() + ',';
            if (x < 0 && res[0] != '-')
                res = res.Insert(0, "-");
            for (int i = X.IndexOf(',') + 1; i < X.IndexOf(',') + 1 + decDigits; ++i)
            {
                if (i < X.Length)
                    res += X[i];
                else
                    res += '0';
            }
            return res;
        }

        public static implicit operator List<LinearEquation>(LinearEquationsSystem system)
        {
            List<LinearEquation> equations = new List<LinearEquation>();

            for (int i = 0; i < system.EquationsNumber; ++i)
                equations.Add((LinearEquation)system[i].Clone());

            return equations;
        }

        public int EquationsNumber
        {
            get
            {
                return equationsNumber;
            }
        }

        public int VariablesNumber
        {
            get
            {
                return variablesNumber;
            }
        }
    }
}
