using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathLibrary.Sign;

namespace MathLibrary
{
    public class LinearInequality : LinearEquation
    {
        public InequalitySign Sign { get; set; }

        public LinearInequality(double[] coeffs, string sign) : base(coeffs)
        {
            Sign = InequalitySign.Create(sign);
        }

        public LinearInequality(string coeffs, string sign) : base(coeffs)
        {
            Sign = InequalitySign.Create(sign);
        }

        public LinearInequality(int n, string sign) : base(n)
        {
            Sign = InequalitySign.Create(sign);
        }
        
        public bool IsCorrect(double[] variables)
        {
            double g = FreeCoeff;
            return Sign.Compare(Value(variables), FreeCoeff);
        }

        public override LinearFunction Clone()
        {
            return new LinearInequality((LinearEquation)base.Clone(), Sign.ToString());
        }

        public override string ToString()
        {
            return base.ToString().Replace("=", Sign.ToString());
        }

        public static LinearInequality operator -(LinearInequality a)
        {
            return new LinearInequality(-new LinearEquation(a), a.Sign.InverseSign().ToString());
        }

        public static bool operator ==(LinearInequality a, LinearInequality b)
        {
            return a == b && a.Sign.ToString() == b.Sign.ToString();
        }

        public static bool operator !=(LinearInequality a, LinearInequality b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (LinearInequality)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Sign.ToString().GetHashCode();
        }

    }
}
