using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary.Sign
{
    public abstract class InequalitySign
    {
        public abstract bool Compare(double num1, double num2);
        public abstract override string ToString();

        public static InequalitySign Create(string inequalitySign)
        {
            switch (inequalitySign)
            {
                case "<":
                    return new SignLess();
                case ">":
                    return new SignMore();
                case "<=":
                    return new SignLessOrEqual();
                case ">=":
                    return new SignMoreOrEqual();
                default:
                    throw new ArgumentException("Неверный ввод знака неравенства");
            }
        }

        public InequalitySign InverseSign()
        {
            return new Dictionary<string, InequalitySign>
            {
                { "<", new SignMore() },
                { ">", new SignLess() },
                { "<=", new SignMoreOrEqual() },
                { ">=", new SignLessOrEqual() }
            }[ToString()];
        }

        public InequalitySign Clone()
        {
            return Create(ToString());
        }
    }
}
