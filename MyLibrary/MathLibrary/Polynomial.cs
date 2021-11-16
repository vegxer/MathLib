using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class Polynomial
    {
        protected double[] coeffs;
        protected int degree;

        public Polynomial(int degree)
        {
            if (degree >= 0)
            {
                this.degree = degree;
                coeffs = new double[degree + 1];
            }
            else
                throw new ArgumentException("Степень многочлена должна быть целой неотрицательной");
        }

        public Polynomial(double[] coeffs)
        {
            degree = coeffs.Length - 1;
            this.coeffs = new double[degree + 1];
            for (int i = 0; i <= degree; ++i)
                this.coeffs[i] = coeffs[degree - i];
        }

        public Polynomial(string coeffs)
        {
            if (!string.IsNullOrWhiteSpace(coeffs))
            {
                char[] separators = { ' ' };
                string[] input = coeffs.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                degree = input.Length - 1;
                this.coeffs = new double[degree + 1];
                for (int i = 0; i <= degree; ++i)
                    this.coeffs[i] = double.Parse(input[degree - i]);
            }
            else
                throw new ArgumentNullException();
        }

        public Polynomial FindDerivative()
        {
            Polynomial derivative = new Polynomial(degree - 1);

            for (int i = 0; i < degree; ++i)
                derivative[i] = (i + 1) * coeffs[i + 1];

            return derivative;
        }

        public double Value(double argument)
        {
            double value = 0;
            for (int i = 0; i < coeffs.Length; ++i)
                value += coeffs[i] * Math.Pow(argument, i);
            return value;
        }

        public override string ToString()
        {
            string polynom = "";
            bool polyn = false;
            for (int i = 0; i < degree; ++i)
            {
                if (coeffs[degree - i] != 0)
                {
                    if (Math.Abs(coeffs[degree - i]) != 1)
                        polynom += Math.Abs(coeffs[degree - i]).ToString() + "*x" + ((degree - i > 1) ? ("^" + (degree - i).ToString()) : "");
                    else
                        polynom += "x" + ((degree - i > 1) ? ("^" + (degree - i).ToString()) : "");
                    polyn = true;
                }
                if (coeffs[degree - i - 1] < 0)
                    polynom += " - ";
                else if (coeffs[degree - i - 1] > 0 && coeffs[degree - i] != 0)
                    polynom += " + ";
            }
            if (coeffs[0] != 0)
                polynom += Math.Abs(coeffs[0]).ToString();
            else if (!polyn)
                polynom = "0";
            return polynom;
        }

        public int Degree
        {
            get
            {
                return degree;
            }
        }

        public double this[int degree]
        {
            get
            {
                if (degree > this.degree || degree < 0)
                    throw new IndexOutOfRangeException("Такой степени многочлена нет");
                else
                    return coeffs[degree];
            }
            set
            {
                if (degree > this.degree || degree < 0)
                    throw new IndexOutOfRangeException("Такой степени многочлена нет");
                else
                    coeffs[degree] = value;
            }
        }

        public static Polynomial operator -(Polynomial polynom)
        {
            for (int i = 0; i <= polynom.Degree; ++i)
                polynom[i] *= -1;
            return polynom;
        }

        public static Polynomial operator +(Polynomial polynom1, Polynomial polynom2)
        {
            Polynomial resPolynom = new Polynomial(Math.Max(polynom1.Degree, polynom2.Degree));
            int i;
            for (i = 0; i <= Math.Min(polynom1.Degree, polynom2.Degree); ++i)
                resPolynom[i] = polynom1[i] + polynom2[i];
            for (; i <= Math.Max(polynom1.Degree, polynom2.Degree); ++i)
                resPolynom[i] = polynom1.Degree > polynom2.Degree ? polynom1[i] : polynom2[i];
            return resPolynom;
        }

        public static Polynomial operator -(Polynomial polynom1, Polynomial polynom2)
        {
            Polynomial resPolynom = new Polynomial(Math.Max(polynom1.Degree, polynom2.Degree));
            int i;
            for (i = 0; i <= Math.Min(polynom1.Degree, polynom2.Degree); ++i)
                resPolynom[i] = polynom1[i] - polynom2[i];
            for (; i <= Math.Max(polynom1.Degree, polynom2.Degree); ++i)
                resPolynom[i] = polynom1.Degree > polynom2.Degree ? polynom1[i] : polynom2[i];
            return resPolynom;
        }

        public static Polynomial operator *(double number, Polynomial polynom)
        {
            for (int i = 0; i <= polynom.Degree; ++i)
                polynom[i] *= number;
            return polynom;
        }

        public static Polynomial operator *(Polynomial polynom, double number)
        {
            for (int i = 0; i <= polynom.Degree; ++i)
                polynom[i] *= number;
            return polynom;
        }

        public static Polynomial operator /(Polynomial polynom, double number)
        {
            for (int i = 0; i <= polynom.Degree; ++i)
                polynom[i] = polynom[i] / number;
            return polynom;
        }

        public static bool operator ==(Polynomial polynom1, Polynomial polynom2)
        {
            return polynom1.Degree < polynom2.Degree ? CheckEquality(polynom1, polynom2) : CheckEquality(polynom2, polynom1);
        }

        public static bool operator !=(Polynomial polynom1, Polynomial polynom2)
        {
            return polynom1.Degree < polynom2.Degree ? !CheckEquality(polynom1, polynom2) : !CheckEquality(polynom2, polynom1);
        }

        private static bool CheckEquality(Polynomial smallerPolynom, Polynomial biggerPolynom)
        {
            for (int i = smallerPolynom.Degree + 1; i <= biggerPolynom.Degree; ++i)
                if (biggerPolynom[i] != 0)
                    return false;
            for (int i = 0; i <= smallerPolynom.Degree; ++i)
                if (biggerPolynom[i] != smallerPolynom[i])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Polynomial)
                return Degree < (obj as Polynomial).Degree ? CheckEquality(this, obj as Polynomial) : CheckEquality(obj as Polynomial, this);
            else
                throw new ArgumentException();
        }

        public override int GetHashCode()
        {
            int hash = (int)coeffs[0];
            for (int i = 0; i < coeffs.Length; ++i)
                hash ^= (int)coeffs[i];
            return hash;
        }

    }
}
