using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    public class FileHandling
    {
        public static void WriteToFile(string filePath, TableauModel model)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write the objective function
                writer.WriteLine(string.Join(" ", model.ObjectiveFunction));

                // Write the constraints
                for (int i = 0; i < model.NumberOfMaxConstraints; i++)
                {
                    for (int j = 0; j < model.NumberOfVariables; j++)
                    {
                        writer.Write(model.MaxConstraintMatrix[i, j] + " ");
                    }
                    writer.WriteLine(model.RightHandSide[i + 1]);
                }
            }
        }

        public static TableauModel ReadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            string[] objFuncTokens = lines[0].Split(' ');

            double[] objectiveFunction = Array.ConvertAll(objFuncTokens, double.Parse);
            int numberOfVariables = objectiveFunction.Length;
            int numberOfConstraints = lines.Length - 1;

            double[,] constraintMatrix = new double[numberOfConstraints, numberOfVariables];
            double[] rightHandSide = new double[numberOfConstraints + 1];

            for (int i = 1; i <= numberOfConstraints; i++)
            {
                string[] constraintTokens = lines[i].Split(' ');
                for (int j = 0; j < numberOfVariables; j++)
                {
                    constraintMatrix[i - 1, j] = double.Parse(constraintTokens[j]);
                }
                rightHandSide[i] = double.Parse(constraintTokens[numberOfVariables]);
            }

            return new TableauModel(constraintMatrix, rightHandSide, objectiveFunction);
        }
    }
}
