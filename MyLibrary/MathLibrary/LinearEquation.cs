using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class LinearEquation : LinearFunction
    {
        public double FreeCoeff { get; set; }

        public LinearEquation(double[] coeffs) : base(coeffs.Take(coeffs.Length - 1).ToArray())
        {
            FreeCoeff = coeffs[coeffs.Length - 1];
        }

        public LinearEquation(string coeffs) : base(coeffs)
        {
            FreeCoeff = base.coeffs[base.coeffs.Length - 1];
            base.coeffs = base.coeffs.Take(base.coeffs.Length - 1).ToArray();
        }

        public LinearEquation(int n) : base(n)
        {
            FreeCoeff = 0;
        }

        public override string ToString()
        {
            return base.ToString() + " = " + FreeCoeff.ToString();
        }

        public override void RandomInit(double leftBorder, double rightBorder)
        {
            base.RandomInit(leftBorder, rightBorder);
            FreeCoeff = new Random().NextDouble() * (rightBorder - leftBorder) + leftBorder;
        }

        public override void InitByNumber(double number)
        {
            base.InitByNumber(number);
            FreeCoeff = number;
        }

        public override LinearFunction Clone()
        {
            LinearEquation clone = new LinearEquation(coeffs.Length);
            clone.coeffs = base.Clone();
            clone.FreeCoeff = FreeCoeff;

            return clone;
        }

        public static bool IsCoeffsCorrect(LinearEquation a)
        {
            if (double.IsNaN(a.FreeCoeff) || double.IsInfinity(a.FreeCoeff))
                return false;

            if (a.FreeCoeff != 0)
            {
                for (int i = 0; i < a.Length; ++i)
                    if (a[i] != 0)
                        return true;
                return false;
            }
            return true;
        }

        public static LinearEquation operator +(LinearEquation a, LinearEquation b)
        {
            return new LinearEquation(new LinearFunction(a) + new LinearFunction(b));
        }

        public static LinearEquation operator *(LinearEquation a, double r)
        {
            return new LinearEquation(new LinearFunction(a) * r);
        }

        public static LinearEquation operator -(LinearEquation a)
        {
            return new LinearEquation(-new LinearFunction(a));
        }

        public static LinearEquation operator -(LinearEquation a, LinearEquation b)
        {
            return a + (-b);
        }

        public static bool operator ==(LinearEquation a, LinearEquation b)
        {
            return new LinearFunction(a) == new LinearFunction(b);
        }

        public static bool operator !=(LinearEquation a, LinearEquation b)
        {
            return !(a == b);
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && FreeCoeff == 0;
        }

        public static implicit operator double[](LinearEquation a)
        {
            double[] coeffs = new double[a.Length + 1];
            ((double[])(LinearFunction)a).CopyTo(coeffs, 0);
            coeffs[a.Length] = a.FreeCoeff;
            return coeffs;
        }

        public override bool Equals(object obj)
        {
            return this == (LinearEquation)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ (int)FreeCoeff;
        }
    }
}
