using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathLibrary
{
    /// <summary>
    /// Предоставляет возможность вычислять выражения
    /// с бинарными и унарными математическими операциями
    /// в различных системах счисления
    /// </summary>
    /// <seealso cref="Источник: https://github.com/vegxer/MaxLib"/>
    public static class Calculator
    {
        delegate T BinaryOperationDelegate<T>(T x, T y);
        delegate T UnaryOperationDelegate<T>(T x);

        static Dictionary<string, BinaryOperationDelegate<double>> DoubleOperation =
            new Dictionary<string, BinaryOperationDelegate<double>>
        {
            { "+", (x, y) => x + y },
            { "*", (x, y) => x * y },
            { "^", (x, y) => Math.Pow(x, y) },
            { "/", (x, y) => x / y },
            { "div", (x, y) => (int)x / (int)y },
            { "mod", (x, y) => x < 0 ? (Math.Abs(y) - (Math.Abs((int)x) % Math.Abs((int)y))) : ((int)x % Math.Abs((int)y)) },
        };

        static Dictionary<string, UnaryOperationDelegate<double>> UnaryOperation =
            new Dictionary<string, UnaryOperationDelegate<double>>
        {
            { "sin", x => Math.Sin(x) },
            { "cos", x => Math.Cos(x) },
            { "tg", x => Math.Tan(x) },
            { "ctg", x => 1d / Math.Tan(x)},
            { "ln", x => Math.Log(x) },
            { "√", x => Math.Sqrt(x) },
            { "|", x => Math.Abs(x)},
            { "!", x => Factorial((long)x) },
            { "±", x => x *= -1},
        };

        static Dictionary<string, string> BinaryOperation = new Dictionary<string, string>
        {
            { "and", "&" },
            { "or", "|" },
            { "xor", "^" }
        };

        /// <summary>
        /// Вычисляет математическое выражение
        /// </summary>
        /// <returns>
        /// Результат вычисления выражения в виде строки
        /// </returns>
        /// <param name="expression">Выражение (только с одной операцией)</param>
        /// <param name="numSystem">Система счисления чисел в выражении (от 2 до 16)</param>
        /// <example>
        /// <code>
        /// string sum = Calc.DoOperation("2+2", 10); //sum = 4
        /// string cos = Calc.DoOperation("cos(0)", 10); //cos = 1
        /// </code>
        /// </example>
        public static string DoOperation(string expression, int numSystem)
        {
            expression = FormatString(expression, numSystem);
            if (IsDoubleNumber(expression, numSystem))
            {
                if (CheckNumberLength(expression, numSystem))
                    return expression.TrimStart('0');
                else
                    throw new ArgumentException("Слишком большое число");
            }
            else
            {
                string[] operands = GetOperands(expression, numSystem);
                if (!CheckOperandsLength(operands, numSystem))
                    throw new ArgumentException("Слишком большое число");
                ConvertOperandsToDecimal(operands, numSystem);
                string operation = GetOperation(expression, numSystem);
                if ((operation == "/" || operation == "div" || operation == "mod") && double.Parse(operands[1]) == 0)
                    throw new DivideByZeroException("Попытка деления на ноль");
                string answer;

                try
                {
                    if (operation == "or" || operation == "and" || operation == "xor")
                        answer = BinCalcOperation(operands, operation);
                    else if (operands.Length == 1)
                        answer = Math.Round(UnaryOperation[operation](double.Parse(operands[0])), 15).ToString("F15");
                    else
                        answer = Math.Round(DoubleOperation[operation](double.Parse(operands[0]), double.Parse(operands[1])), 15).ToString("F15");
                }
                catch
                {
                    throw new ArgumentException("Неверно введены операции");
                }

                if (IsDoubleNumber(answer, 10))
                {
                    if (answer.IndexOf('E') != -1)
                    {
                        answer = answer.Replace("E", "e");
                        return ConvertToBase(answer.Substring(0, answer.IndexOf('e')), 10, numSystem) +
                            "e" + ConvertToBase(answer.Substring(answer.IndexOf('e') + 1), 10, numSystem);
                    }
                    return RemovePointlessZeros(ConvertToBase(RemovePointlessZeros(answer), 10, numSystem));
                }
                else
                    return "Ошибка";
            }
        }

        /// <summary>
        /// Переводит число из одной системы счисления в другую
        /// </summary>
        /// <returns>
        /// Число в виде строки в требуемой системе счисления
        /// </returns>
        /// <param name="number">Переводимое число</param>
        /// <param name="from">Из какой системы счисления переводим (от 2 до 16)</param>
        /// <param name="to">В какую систему счисления переводим (от 2 до 16)</param>
        /// <example>
        /// <code>
        /// string hexNum = Calc.DoOperation("10011101", 2, 16); //hexNum = 9D
        /// string binNum = Calc.DoOperation("5", 10, 2); //binNum = 101
        /// </code>
        /// </example>
        public static string ConvertToBase(string number, int from, int to)
        {
            string decimalPart = "", intPart = "";
            if (!CheckNumberLength(number, from))
                throw new ArgumentException("Слишком большое число");
            if (!IsDoubleNumber(number, from))
                throw new ArgumentException("Некорректное число");
            number = CutMinuses(number.Replace(" ", ""));
            string minus = number.IndexOf('-') != -1 ? "-" : "";
            number = number.Replace("-", "");
            try
            {
                if (from > 1 && from < 17 && to > 1 && to < 17)
                {
                    if (number.IndexOf(',') != -1)
                    {
                        decimalPart = ConvertFractionalPartToBase(number.Substring(number.IndexOf(',') + 1), from, to);
                        number = number.Substring(0, number.IndexOf(','));
                    }

                    intPart = ConvertIntegerPartToBase(number, from, to);
                }
                else
                    throw new Exception();
            }
            catch
            {
                throw new ArgumentException("Некорретная система счисления");
            }

            return minus + intPart.ToUpper() + decimalPart;
        }

        /// <summary>
        /// Проверяет, можно ли привести строку к числу типа uint
        /// </summary>
        /// <returns>
        /// Можно привести к uint (true) или нельзя (false)
        /// </returns>
        /// <param name="str">Проверяемая строка</param>
        /// <param name="numSystem">Система счисления (от 2 до 16)</param>
        /// <example>
        /// <code>
        /// bool IsDouble = Calc.DoOperation("F1,6A", 16); //true
        /// bool IsDouble = Calc.DoOperation("4a6", 10); //false
        /// </code>
        /// </example>
        public static bool IsDoubleNumber(string str, int numSystem)
        {
            str = str.Replace(" ", "");
            if (numSystem > 10 && numSystem < 17)
                return str == "e" || str == "π" ||
                    (str.Count(chr => chr == ',') <= 1 && str.Replace(",", "").Replace("-", "") != ""
                    && str.TrimStart('-').Count(chr => chr == '-') == 0 &&
                    str.All(chr => char.IsDigit(chr) || (chr >= 'A' && chr <= DecToHEX(numSystem)) || chr == ',' || chr == '-'));
            else
            {
                double num;
                return (double.TryParse(CutMinuses(str), out num) && !double.IsNaN(num) && !double.IsInfinity(num))
                    || str == "e" || str == "π";
            }
        }

        /// <summary>
        /// Проверяет, можно ли привести строку к числу типа uint
        /// </summary>
        /// <returns>
        /// Можно привести к uint (true) или нельзя (false)
        /// </returns>
        /// <param name="str">Проверяемая строка</param>
        /// <param name="numSystem">Система счисления (от 2 до 16)</param>
        /// <example>
        /// <code>
        /// bool IsInt = Calc.DoOperation("4", 10); //true
        /// bool IsInt = Calc.DoOperation("4,4", 10); //false
        /// </code>
        /// </example>
        public static bool IsIntNumber(string str, int numSystem)
        {
            str = str.Replace(" ", "");
            if (numSystem == 16)
                return str.TrimStart('-').Count(chr => chr == '-') == 0 && str.Replace(",", "").Replace("-", "") != ""
                    && str.All(chr => char.IsDigit(chr) || (chr >= 'A' && chr <= DecToHEX(numSystem)) || chr == '-');
            else
            {
                int num;
                return int.TryParse(CutMinuses(str), out num) || str == "e" || str == "π";
            }
        }

        /// <summary>
        /// Проверяет, можно ли привести строку к числу типа uint
        /// </summary>
        /// <returns>
        /// Можно привести к uint (true) или нельзя (false)
        /// </returns>
        /// <param name="str">Проверяемая строка</param>
        /// <param name="numSystem">Система счисления (от 2 до 16)</param>
        /// <example>
        /// <code>
        /// bool IsUInt = Calc.DoOperation("4", 10); //true
        /// bool IsUInt = Calc.DoOperation("-4", 10); //false
        /// </code>
        /// </example>
        public static bool IsUIntNumber(string str, int numSystem)
        {
            str = str.Replace(" ", "");
            if (numSystem == 16)
                return str != "" &&
                    str.All(chr => char.IsDigit(chr) || (chr >= 'A' && chr <= DecToHEX(numSystem)));
            else
            {
                uint num;
                return uint.TryParse(CutMinuses(str), out num) || str == "e" || str == "π";
            }
        }

        /// <summary>
        /// Удаляет незначащие нули в числе после десятичной запятой
        /// </summary>
        /// <returns>
        /// Число в виде строки без незначащих нулей
        /// </returns>
        /// <param name="number">Входное число</param>
        /// <example>
        /// <code>
        /// string number = Calc.DoOperation("15,6000"); //number = 15,6
        /// string number = Calc.DoOperation("15,0000"); //number = 15
        /// </code>
        /// </example>
        public static string RemovePointlessZeros(string number)
        {
            if (number.IndexOf(',') != -1)
            {
                int i;
                for (i = number.Length - 1; number[i] == '0'; --i)
                    number = number.Remove(i, 1);

                if (number[i] == ',')
                    number = number.Remove(i, 1);
            }

            return number;
        }

        static int HEXToDec(char digit)
        {
            if (digit < 'A' || digit > 'F')
                throw new ArgumentException();

            return digit - 55;
        }

        static char DecToHEX(int digit)
        {
            if (digit < 10 || digit > 16)
                throw new ArgumentException();

            return (char)(digit + 55);
        }

        static string ConvertFractionalPartToBase(string number, int from, int to)
        {
            double decNumber = ConvertFractionalPartToDecimal(number, from);

            string decimalPart = "";
            for (double decPart = decNumber; decimalPart.Length < 16 && decPart != 0; decPart -= (int)decPart)
            {
                decPart *= to;
                if ((int)decPart > 9)
                    decimalPart += DecToHEX((int)decPart);
                else
                    decimalPart += ((int)decPart).ToString();
            }

            return "," + decimalPart;
        }

        static double ConvertFractionalPartToDecimal(string number, int from)
        {
            double result = 0;

            for (int i = 0; i < number.Length; ++i)
            {
                if (!char.IsDigit(number[i]))
                    result += HEXToDec(number[i]) * Math.Pow(from, -i - 1);
                else
                    result += int.Parse(number[i].ToString()) * Math.Pow(from, -i - 1);
            }

            return Math.Round(result, 15);
        }

        static string ConvertIntegerPartToBase(string number, int from, int to)
        {
            long intNum = Convert.ToInt64(number, from);
            string intPart = Convert.ToString(intNum, to);

            return intPart;
        }

        static void ConvertOperandsToDecimal(string[] operands, int numSystem)
        {
            for (int i = 0; i < operands.Length; ++i)
                operands[i] = ConvertToBase(operands[i], numSystem, 10);
        }

        static long Factorial(long n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("Число должно быть целым неотрицательным");
            long factorial = 1;

            for (long i = 2; i <= n; ++i)
            {
                factorial *= i;

                if (factorial <= 0)
                    throw new OverflowException("Слишком большое число");
            }

            return factorial;
        }

        static bool CheckOperandsLength(string[] operands, int numSystem)
        {
            foreach (string operand in operands)
                if (!CheckNumberLength(operand, numSystem))
                    return false;

            return true;
        }

        static bool CheckNumberLength(string number, int numSystem)
        {
            int correctLength;
            switch (numSystem)
            {
                case 10:
                    correctLength = 15;
                    break;
                case 8:
                    correctLength = 18;
                    break;
                case 2:
                    correctLength = 32;
                    break;
                default:
                    correctLength = 9;
                    break;
            }

            number += ",";
            return number.Substring(0, number.IndexOf(',')).ToString().Length <= correctLength;
        }

        static string BinCalcOperation(string[] operands, string operation)
        {
            Assembly SampleAssembly = Assembly.LoadFrom("BinCalc.dll");
            Type type = SampleAssembly.GetTypes()[0];
            MethodInfo method = type.GetMethods()[0];
            object obj = Activator.CreateInstance(type);
            return (string)method.Invoke(obj, new object[] { operands[0] + BinaryOperation[operation] + operands[1] }) + ",0";
        }

        static string[] GetOperands(string str, int numSystem)
        {
            MatchCollection mc = new Regex(@"[^\d,\.ABCDEF-]").Matches(str);

            List<string> separators = new List<string>();
            foreach (Match m in mc)
                separators.Add(m.Value);

            List<string> numbers = str.Split(separators.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            SelectNumbers(numbers);
            CutMinuses(numbers);
            if (numbers.Count > 0 && numbers.Count <= 2)
            {
                if (numbers.All(x => IsDoubleNumber(x, numSystem)))
                    return numbers.ToArray();
                else
                    throw new ArgumentException("Некорректный ввод чисел");
            }
            else
                throw new ArgumentException("Некорректное количество чисел");
        }

        static string GetOperation(string str, int numSystem)
        {
            MatchCollection mc = new Regex(@"[^\d,\.ABCDEF-]").Matches(str);

            List<string> separators = new List<string>();
            foreach (Match m in mc)
                separators.Add(m.Value);

            List<string> numbers = str.Split(separators.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            SelectNumbers(numbers);
            string[] operations = str.Split(numbers.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            string operation = operations.Length == 0 ? "" : operations[0].Replace("(", "");
            operation = operation.Replace(" ", "");
            if (CheckOperations(operations) && operations.Length <= 2
                && CheckOperationCorrectness(operation, numbers, operations, str, numSystem))
            {
                if (!operation.Any())
                    return "+";
                return operation[0] + operation.Replace(operation[0].ToString(), "");
            }
            else
                throw new ArithmeticException("Неверно введены операции");
        }

        static bool CheckOperations(string[] operations)
        {
            if (operations.Length == 1)
                return true;
            else if (operations.Length == 2)
                return operations[1] == ")" || operations[1] == "|";
            else
                return false;
        }

        static string FormatString(string str, int numSystem)
        {
            if (str.IndexOf('π') != -1)
                str = str.Replace("π", ConvertToBase(Math.PI.ToString(), 10, numSystem));
            if (str.IndexOf('e') != -1)
                str = str.Replace("e", ConvertToBase(Math.E.ToString(), 10, numSystem));
            str = str.Replace(" ", "");
            for (int i = 0; i < str.Length - 1; ++i)
                if (char.IsDigit(str[i]) && str[i + 1] == '-')
                    return str.Insert(i + 1, " ");
            return str;
        }

        static void SelectNumbers(List<string> numbers)
        {
            for (int i = 0; i < numbers.Count; ++i)
            {
                if (numbers[i].All(x => x == '-'))
                    numbers.RemoveAt(i--);
            }
        }

        static void CutMinuses(List<string> numbers)
        {
            for (int i = 0; i < numbers.Count; ++i)
                numbers[i] = CutMinuses(numbers[i]);
        }

        static bool CheckOperationCorrectness(string operation, List<string> numbers, string[] operations, string str, int numSystem)
        {
            if (!operation.Any())
                return true;
            if ((operation[0] == '-' || operation[0] == '+')
                && !operation.All(x => x == operation[0]))
                return false;
            if ((operation[0] == '*' || operation[0] == '/') && (operation.Length > 1 || operations.Length > 1))
                return false;
            if (operation.Length == 0 && numbers[1][0] != '-')
                return false;
            if ((operation == "sin" || operation == "cos" || operation == "tg" || operation == "ctg"
                || operation == "ln")
                && (operations[0][operations[0].Length - 1] != '(' || operations[1] != ")"
                || !IsDoubleNumber(numbers[0].ToString(), numSystem) || numbers.Count > 1))
                return false;
            if (operation == "√" && (numbers.Count > 1
                || !IsDoubleNumber(numbers[0].ToString(), numSystem)))
                return false;
            if (operation == "|" && (numbers.Count > 1 || operations[0][operations[0].Length - 1] != '|' || operations[1] != "|"
                || !IsDoubleNumber(numbers[0].ToString(), numSystem)))
                return false;
            if ((operation == "div" || operation == "mod")
                && (!IsIntNumber(numbers[0].ToString(), numSystem) || !IsIntNumber(numbers[1].ToString(), numSystem)))
                return false;
            if ((operation == "or" || operation == "and" || operation == "xor")
                && (!IsUIntNumber(numbers[0].ToString(), numSystem) || !IsUIntNumber(numbers[1].ToString(), numSystem)))
                return false;
            if (operation == "!" && (str.IndexOf('!') == -1 || !IsUIntNumber(str.Substring(0, str.IndexOf('!')), numSystem)
                || numbers.Count > 1 || !IsUIntNumber(numbers[0].ToString(), numSystem)))
                return false;
            if (operation == "±" && (!IsDoubleNumber(numbers[0], numSystem) || numbers.Count > 1))
                return false;
            return true;
        }

        static string CutMinuses(string number)
        {
            int minusesCount = number.Length - number.TrimStart('-').Length;
            number = number.TrimStart('-');

            if (minusesCount % 2 == 1)
                number = "-" + number;

            return number;
        }
    }
}
