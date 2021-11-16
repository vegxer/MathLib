using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MathLibrary
{
    public class LinearFunction
    {
        protected double[] coeffs;

        public LinearFunction(double[] coeffs)
        {
            this.coeffs = new double[coeffs.Length];
            coeffs.CopyTo(this.coeffs, 0);
        }

        public LinearFunction(string coeffs)
        {
            this.coeffs = coeffs.Split(new string[] { " " }, 
                StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();
        }

        public static LinearFunction FromFile(string @filePath)
        {
            using (StreamReader reader = new StreamReader(@filePath))
            {
                string function = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(function))
                    throw new ArgumentException("Функция должна быть записана в первой строке файла");

                return new LinearFunction(function);
            }
        }

        public LinearFunction(int n)
        {
            if (n > 1)
                coeffs = new double[n];
            else
                throw new ArgumentOutOfRangeException("Количество коэффициентов должно быть больше одного");
        }

        public virtual LinearFunction Clone()
        {
            return new LinearFunction(coeffs);
        }

        public double Value(double[] variables)
        {
            if (coeffs.Length != variables.Length)
                throw new ArgumentException("Некорректное количество переменных");

            if (double.IsInfinity(variables[0]))
            {
                if (Value(Enumerable.Repeat(1d, variables.Length).ToArray()) < 0)
                    return double.NegativeInfinity;
                else
                    return double.PositiveInfinity;
            }

            double value = 0;

            for (int i = 0; i < coeffs.Length; ++i)
                value += coeffs[i] * variables[i];

            return value;
        }

        public string ValueStr(double[] variables)
        {
            double value = Value(variables);

            if (double.IsNegativeInfinity(value))
                return "-inf";
            else if (double.IsPositiveInfinity(value))
                return "+inf";
            else
                return value.ToString();
        }

        public double this[int index]
        {
            get
            {
                if (index >= 0 && index < coeffs.Length)
                    return coeffs[index];
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < coeffs.Length)
                    coeffs[index] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public int Length => coeffs.Length;

        public override string ToString()
        {
            string func = "";

            for (int i = 0; i < coeffs.Length; ++i)
            {
                if (i != 0 && coeffs[i] != 0 && func != "")
                    func += (coeffs[i] < 0 ? " - " : " + ");

                if (coeffs[i] != 0)
                {
                    if (coeffs[i] != 1)
                        func += Math.Abs(coeffs[i]).ToString() + "*";

                    func += "x" + (i + 1).ToString();
                }
            }

            if (func == "")
                func = "0";

            return func;
        }

        public virtual void RandomInit(double leftBorder, double rightBorder)
        {
            if (leftBorder < rightBorder)
            {
                Random Rand = new Random();
                for (int i = 0; i < coeffs.Length; ++i)
                    coeffs[i] = Rand.NextDouble() * (rightBorder - leftBorder) + leftBorder;
            }
            else
                throw new ArgumentException("Левая граница должна быть меньше правой");
        }

        public virtual void InitByNumber(double number)
        {
            for (int i = 0; i < coeffs.Length; ++i)
                coeffs[i] = number;
        }

        public static LinearFunction operator +(LinearFunction a, LinearFunction b)
        {
            return a.Length > b.Length ? Sum(b, a) : Sum(a, b);
        }

        private static LinearFunction Sum(LinearFunction a, LinearFunction b)
        {
            LinearFunction sum = new LinearFunction(b.Length);
            int i;

            for (i = 0; i < a.Length; ++i)
                sum[i] = a[i] + b[i];

            for (; i < b.Length; ++i)
                sum[i] = b[i];
            
            return sum;
        }

        public static LinearFunction operator -(LinearFunction a, LinearFunction b)
        {
            return a + (-b);
        }

        public static LinearFunction operator *(LinearFunction a, double r)
        {
            for (int i = 0; i < a.Length; ++i)
                a[i] *= r;
            
            return a;
        }

        public static LinearFunction operator *(double r, LinearFunction a)
        {
            return a * r;
        }

        public static LinearFunction operator -(LinearFunction a)
        {
            for (int i = 0; i < a.Length; ++i)
                a[i] *= -1;
            
            return a;
        }

        public static bool operator ==(LinearFunction a, LinearFunction b)
        {
            return a.Length > b.Length ? IsEqual(b, a) : IsEqual(a, b);
        }

        public static bool operator !=(LinearFunction a, LinearFunction b)
        {
            return !(a == b);
        }

        private static bool IsEqual(LinearFunction a, LinearFunction b)
        {
            int i;
            for (i = 0; i < a.Length; ++i)
                if (a[i] != b[i])
                    return false;
            for (; i < b.Length; ++i)
                if (b[i] != 0)
                    return false;

            return true;
        }

        public virtual bool IsEmpty()
        {
            for (int i = 0; i < Length; ++i)
                if (coeffs[i] != 0)
                    return false;

            return true;
        }

        public static implicit operator double[](LinearFunction a)
        {
            double[] coeffs = new double[a.Length];

            for (int i = 0; i < a.Length; ++i)
                coeffs[i] = a[i];

            return coeffs;
        }

        public override bool Equals(object obj)
        {
            return this == (LinearFunction)obj;
        }

        public override int GetHashCode()
        {
            int hash = (int)coeffs[0];

            for (int i = 1; i < Length; ++i)
                hash ^= (int)coeffs[i];

            return hash;
        }
    }
}
