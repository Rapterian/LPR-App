using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    public class FileHandling
    {
        public static void WriteToFile(string filePath, List<string> content)
        {
            // Capture the original console output
            TextWriter originalConsole = Console.Out;

            // Use a StringBuilder to capture the console output
            StringBuilder consoleOutput = new StringBuilder();
            using (StringWriter writer = new StringWriter(consoleOutput))
            {
                // Write the captured output to a file
                File.AppendAllLines(filePath, content);
            }

            // Restore the original console output
            Console.SetOut(originalConsole);
        }

        public static void ReadFromFile(out string problemType, out double[] objectiveFunction,
                                out double[,] constraintMatrix, out double[] rightHandSide,
                                out string[] constraintRelations, out string[] signRestrictions)
        {
            {
                string filePath = "Problem.txt";
                string[] lines = File.ReadAllLines(filePath);

                // Parse the objective function
                string[] objFuncTokens = lines[0].Split(' ');
                problemType = objFuncTokens[0]; // max or min
                int numberOfVariables = (objFuncTokens.Length - 1) / 2;
                objectiveFunction = new double[numberOfVariables];

                for (int j = 0; j < numberOfVariables; j++)
                {
                    string sign = objFuncTokens[2 * j + 1];
                    double coefficient = double.Parse(objFuncTokens[2 * j + 2]);
                    objectiveFunction[j] = sign == "-" ? -coefficient : coefficient;
                }

                // Parse the constraints
                int numberOfConstraints = lines.Length - 2; // Constraints are all lines except the first and last
                constraintMatrix = new double[numberOfConstraints, numberOfVariables];
                rightHandSide = new double[numberOfConstraints];
                constraintRelations = new string[numberOfConstraints];

                for (int i = 1; i <= numberOfConstraints; i++)
                {
                    string[] constraintTokens = lines[i].Split(' ');

                    for (int j = 0; j < numberOfVariables; j++)
                    {
                        string sign = constraintTokens[2 * j];
                        double coefficient = double.Parse(constraintTokens[2 * j + 1]);
                        constraintMatrix[i - 1, j] = sign == "-" ? -coefficient : coefficient;
                    }

                    constraintRelations[i - 1] = constraintTokens[2 *numberOfVariables];
                    rightHandSide[i - 1] = double.Parse(constraintTokens[2 *numberOfVariables + 1]);
                }

                // Parse the sign restrictions
                signRestrictions = lines[lines.Length - 1].Split(' ');
            }
        }
    }
}
