using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MathLibrary
{
    public class LinearInequalitiesSystem : LinearEquationsSystem
    {
        public LinearInequalitiesSystem(int variablesNumber, int roundValue) : base(variablesNumber, roundValue) { }

        public LinearInequalitiesSystem(List<LinearInequality> linearInequalities, int roundValue)
            : base(linearInequalities.ToList<LinearEquation>(), roundValue) { }

        public LinearInequalitiesSystem(string @filePath, int roundValue) : base(roundValue)
        {
            using (StreamReader reader = new StreamReader(@filePath))
            {
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    List<string> inequality = reader.ReadLine().Split(new string[] { " " } ,
                        StringSplitOptions.RemoveEmptyEntries).ToList();
                    string sign = inequality[inequality.Count - 2];
                    inequality.RemoveAt(inequality.Count - 2);
                    double[] coeffs = inequality.Select(double.Parse).ToArray();

                    if (variablesNumber == -1)
                        variablesNumber = coeffs.Length - 1;
                    Add(new LinearInequality(coeffs, sign));
                }
            }
        }

        public override LinearEquationsSystem Clone()
        {
            LinearInequalitiesSystem clone = new LinearInequalitiesSystem(VariablesNumber, roundValue - 2);
            LinearEquationsSystem baseClone = base.Clone();

            foreach (LinearEquation equation in (List<LinearEquation>)baseClone)
                clone.Add((LinearInequality)equation);

            return clone;
        }
    }
}
