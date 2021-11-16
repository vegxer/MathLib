using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class Fraction
    {
        int numerator, denomerator;

        public Fraction(int numerator, int denomerator)
        {
            this.numerator = numerator;
            if (denomerator > 0)
                this.denomerator = denomerator;
            else
                throw new ArgumentException("Знаменатель должен быть натуральным числом");
            ReduceFraction();
        }

        public Fraction(int numerator)
        {
            this.numerator = numerator;
            this.denomerator = 1;
            ReduceFraction();
        }

        public Fraction(string fract)
        {
            char[] separators = { ' ', '/' };
            string[] input = fract.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (int.TryParse(input[0], out numerator) && int.TryParse(input[1], out denomerator))
                ReduceFraction();
            else
                throw new ArgumentException();
        }

        public Fraction(double decFract)
        {
            if ((int)decFract == decFract)
            {
                numerator = (int)decFract;
                denomerator = 1;
            }
            else
            {
                string strDecFract = decFract.ToString();
                int decimalPartLength = strDecFract.Substring(strDecFract.IndexOf(',') + 1).Length;
                numerator = (int)Math.Pow(10, decimalPartLength) * int.Parse(strDecFract.Substring(0, strDecFract.IndexOf(',')));
                if (numerator < 0)
                    numerator -= int.Parse(strDecFract.Substring(strDecFract.IndexOf(',') + 1));
                else
                    numerator += int.Parse(strDecFract.Substring(strDecFract.IndexOf(',') + 1));
                denomerator = (int)Math.Pow(10, decimalPartLength);
                ReduceFraction();
            }
        }

        private void ReduceFraction()
        {
            if (numerator != 0)
            {
                int gcf = GCF(Math.Abs(numerator), denomerator);
                numerator /= gcf;
                denomerator /= gcf;
            }
        }

        static private int GCF(int a, int b)
        {
            if (a < b)
                return GCF(a, b - a);
            else if (b < a)
                return GCF(a - b, b);
            else
                return a;
        }

        public static Fraction operator -(Fraction fraction) => new Fraction(-fraction.numerator, fraction.denomerator);

        public static Fraction operator +(Fraction fraction1, Fraction fraction2)
        {
            int denomerator = fraction1.denomerator * fraction2.denomerator / GCF(fraction1.denomerator, fraction2.denomerator);
            int numerator = denomerator / fraction1.denomerator * fraction1.numerator + denomerator / fraction2.denomerator * fraction2.numerator;
            return new Fraction(numerator, denomerator);
        }

        public static Fraction operator -(Fraction fraction1, Fraction fraction2)
        {
            int denomerator = fraction1.denomerator * fraction2.denomerator / GCF(fraction1.denomerator, fraction2.denomerator);
            int numerator = denomerator / fraction1.denomerator * fraction1.numerator - denomerator / fraction2.denomerator * fraction2.numerator;
            return new Fraction(numerator, denomerator);
        }

        public static Fraction operator *(Fraction fraction1, Fraction fraction2) => new Fraction(fraction1.numerator * fraction2.numerator, fraction1.denomerator * fraction2.denomerator);

        public static Fraction operator /(Fraction fraction1, Fraction fraction2)
        {
            if (fraction2.numerator != 0)
                return new Fraction(fraction1.numerator * fraction2.denomerator, fraction1.denomerator * fraction2.numerator);
            else
                throw new DivideByZeroException();
        }

        public static bool operator ==(Fraction fraction1, Fraction fraction2)
        {
            return fraction1.numerator == fraction2.numerator && fraction1.denomerator == fraction2.denomerator;
        }

        public static bool operator !=(Fraction fraction1, Fraction fraction2)
        {
            return fraction1.numerator != fraction2.numerator || fraction1.denomerator != fraction2.denomerator;
        }

        public static bool operator <(Fraction fraction1, Fraction fraction2)
        {
            int denomerator = fraction1.denomerator * fraction2.denomerator / GCF(fraction1.denomerator, fraction2.denomerator);
            return denomerator / fraction1.denomerator * fraction1.numerator < denomerator / fraction2.denomerator * fraction2.numerator;
        }

        public static bool operator >(Fraction fraction1, Fraction fraction2)
        {
            int denomerator = fraction1.denomerator * fraction2.denomerator / GCF(fraction1.denomerator, fraction2.denomerator);
            return denomerator / fraction1.denomerator * fraction1.numerator > denomerator / fraction2.denomerator * fraction2.numerator;
        }

        public static bool operator <=(Fraction fraction1, Fraction fraction2)
        {
            int denomerator = fraction1.denomerator * fraction2.denomerator / GCF(fraction1.denomerator, fraction2.denomerator);
            return denomerator / fraction1.denomerator * fraction1.numerator <= denomerator / fraction2.denomerator * fraction2.numerator;
        }

        public static bool operator >=(Fraction fraction1, Fraction fraction2)
        {
            int denomerator = fraction1.denomerator * fraction2.denomerator / GCF(fraction1.denomerator, fraction2.denomerator);
            return denomerator / fraction1.denomerator * fraction1.numerator >= denomerator / fraction2.denomerator * fraction2.numerator;
        }


        public static implicit operator double(Fraction fraction) => (double)fraction.numerator / (double)fraction.denomerator;


        public static explicit operator int(Fraction fraction) => (int)((double)fraction.numerator / (double)fraction.denomerator);

        public override string ToString() => numerator.ToString() + '/' + denomerator.ToString();

        public override bool Equals(object obj)
        {
            if (obj is Fraction)
                return numerator == (obj as Fraction).numerator && denomerator == (obj as Fraction).denomerator;
            else
                throw new ArgumentException();
        }

        public override int GetHashCode() => numerator ^ denomerator;
    }
}
