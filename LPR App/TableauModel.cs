using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LPR_App
{
    public class TableauModel
    {

        public double[,] MaxConstraintMatrix { get; set; }
        public double[,] MinConstraintMatrix { get; set; }
        public double[] RightHandSide { get; set; }
        public double[] ObjectiveFunction { get; set; }

        public int NumberOfVariables { get; set; }
        public int NumberOfMaxConstraints { get; set; }
        public int NumberOfMinConstraints { get; set; }



        public TableauModel(double[,] constraintMatrix, double[] rightHandSide, double[] objectiveFunction)
        {
            MaxConstraintMatrix = constraintMatrix;
            RightHandSide = rightHandSide;
            ObjectiveFunction = objectiveFunction;
            NumberOfVariables = ObjectiveFunction?.Length ?? 0;
            NumberOfMaxConstraints = RightHandSide?.Length - 1 ?? 0;
        }

        public TableauModel(double[,] maxConstraintMatrix, double[,] minConstraintMatrix, double[] rightHandSide, double[] objectiveFunction)
        {
            MaxConstraintMatrix = maxConstraintMatrix;
            MinConstraintMatrix = minConstraintMatrix;
            RightHandSide = rightHandSide;
            ObjectiveFunction = objectiveFunction;
            NumberOfVariables = ObjectiveFunction?.Length ?? 0;
            NumberOfMaxConstraints = 0;


            if (maxConstraintMatrix != null && maxConstraintMatrix.GetLength(0) > 0)
            {
                NumberOfMaxConstraints = maxConstraintMatrix.GetLength(0);
            }

            NumberOfMinConstraints = 0;


            if (minConstraintMatrix != null && minConstraintMatrix.GetLength(0) > 0)
            {
                NumberOfMinConstraints = minConstraintMatrix.GetLength(0);
            }
        }

        public TableauModel(double[,] matrix, int numberOfVariables, int numberOfMaxConstraints, int numberOfMinConstraints)
        {
            int numberOfConstraints = numberOfMaxConstraints + numberOfMinConstraints;
            double[,] maxConstraintMatrix = new double[numberOfMaxConstraints, numberOfVariables + numberOfConstraints + 1];
            double[,] minConstraintMatrix = new double[numberOfMinConstraints, numberOfVariables + numberOfConstraints + 1];
            double[] objectiveFunction = new double[numberOfConstraints + numberOfVariables];
            double[] rightHandSide = new double[numberOfConstraints + 1];
            for (int i = 0; i < numberOfConstraints + 1; i++)
            {
                rightHandSide[i] = matrix[i, numberOfVariables + numberOfConstraints];
            }
            for (int i = 0; i < numberOfVariables + numberOfConstraints; i++)
            {
                objectiveFunction[i] = matrix[0, i];
            }
            for (int i = 0; i < numberOfMaxConstraints; i++)
            {
                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)
                {
                    maxConstraintMatrix[i, j] = matrix[i + 1, j];
                }
            }
            for (int i = numberOfMaxConstraints; i < numberOfConstraints; i++)
            {
                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)
                {
                    minConstraintMatrix[i, j] = matrix[i + 1, j];
                }
            }

            MaxConstraintMatrix = maxConstraintMatrix;
            MinConstraintMatrix = minConstraintMatrix;
            RightHandSide = rightHandSide;
            ObjectiveFunction = objectiveFunction;
            NumberOfVariables = numberOfVariables;
            NumberOfMaxConstraints = numberOfMaxConstraints;
            NumberOfMinConstraints = numberOfMinConstraints;
        }

        public TableauModel(double[,] matrix, int numberOfVariables, int numberOfConstraints)
        {
            double[,] constraintMatrix = new double[numberOfConstraints + 1, numberOfVariables + numberOfConstraints + 1];
            double[] objectiveFunction = new double[numberOfConstraints + numberOfVariables];
            double[] rightHandSide = new double[numberOfConstraints + 1];
            for (int i = 0; i < numberOfConstraints + 1; i++)
            {
                rightHandSide[i] = matrix[i, numberOfVariables + numberOfConstraints];
            }
            for (int i = 0; i < numberOfVariables + numberOfConstraints; i++)
            {
                objectiveFunction[i] = matrix[0, i];
            }
            for (int i = 0; i < numberOfConstraints; i++)
            {
                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)
                {
                    constraintMatrix[i, j] = matrix[i + 1, j];
                }
            }
            MaxConstraintMatrix = constraintMatrix;
            RightHandSide = rightHandSide;
            ObjectiveFunction = objectiveFunction;
            NumberOfVariables = numberOfVariables;
            NumberOfMaxConstraints = numberOfConstraints;
        }


        public int NumberOfConstraints()
        {
            return NumberOfMaxConstraints + NumberOfMinConstraints;
        }
        public double[,] CanonicalForm(bool initialTableau)
        {
            int numberOfConstraints = NumberOfMaxConstraints + NumberOfMinConstraints;

            //Initialize tableau
            double[,] tableau = new double[numberOfConstraints + 1, NumberOfVariables + NumberOfMaxConstraints + 1];//2D array with number of constraints rows and number of variables + number of constraints(slack variables) columns

            if (initialTableau)
            {
                //Objective Function Row
                for (int j = 0; j < NumberOfVariables; j++)
                {
                    tableau[0, j] = -ObjectiveFunction[j];//make the z row variables negative
                }

                tableau[0, NumberOfVariables + numberOfConstraints] = RightHandSide[0];//Objective Function Row RHS

                //Constraint Rows
                for (int i = 0; i < NumberOfMaxConstraints; i++)
                {
                    for (int j = 0; j < NumberOfVariables; j++)
                    {
                        tableau[i + 1, j] = MaxConstraintMatrix[i, j];//put the constraint matrix values in the tableau row by row
                    }
                    tableau[i + 1, NumberOfVariables + i] = 1; //Slack Variables
                    tableau[i + 1, NumberOfVariables + numberOfConstraints] = RightHandSide[i + 1];//RHS values
                }
                for (int i = NumberOfMaxConstraints; i < numberOfConstraints; i++)
                {
                    for (int j = 0; j < NumberOfVariables; j++)
                    {
                        tableau[i + 1, j] = MinConstraintMatrix[i - NumberOfMaxConstraints, j];//put the constraint matrix values in the tableau row by row
                    }
                    tableau[i + 1, NumberOfVariables + i] = -1; //excess Variables
                    tableau[i + 1, NumberOfVariables + numberOfConstraints] = RightHandSide[i + 1];//RHS values
                }
            }
            else
            {
                for (int j = 0; j < numberOfConstraints + NumberOfVariables; j++)
                {
                    tableau[0, j] = ObjectiveFunction[j];
                }

                for (int i = 1; i < numberOfConstraints + 1; i++)
                {

                    for (int j = 0; j < numberOfConstraints + NumberOfVariables; j++)
                    {
                        tableau[i, j] = MaxConstraintMatrix[i - 1, j];
                    }

                }
                for (int i = 0; i < numberOfConstraints + 1; i++)
                {
                    tableau[i, numberOfConstraints + NumberOfVariables] = RightHandSide[i];
                }
            }

            return tableau;
        }

        /// <summary>
        /// This function will display the tableau in the console with the name
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numberOfVariables"></param>
        /// <param name="numberOfConstraints"></param>
        /// <param name="name"></param>
        public void ToConsole(string name, bool initialTableau)
        {
            int numberOfConstraints = NumberOfMaxConstraints + NumberOfMinConstraints;
            for (int i = 0; i < name.Length; i++)
            {
                Console.Write($"-");
            }
            Console.WriteLine();
            Console.WriteLine(name);
            for (int i = 0; i < name.Length; i++)
            {
                Console.Write($"-");
            }
            Console.WriteLine();
            for (int i = 0; i < NumberOfVariables; i++)
            {
                Console.Write($"x{i + 1} \t");
            }
            int s = 1;
            for (int i = 0; i < NumberOfMaxConstraints; i++)
            {
                Console.Write($"s{s} \t");
                s++;
            }
            for (int i = 0; i < NumberOfMinConstraints; i++)
            {
                Console.Write($"e{s} \t");
                s++;
            }
            Console.WriteLine("RHS");
            Console.WriteLine("----------------------------------------------");
            for (int i = 0; i < NumberOfMaxConstraints + 1; i++)
            {
                for (int j = 0; j < NumberOfMaxConstraints + NumberOfVariables + 1; j++)
                {
                    double value = CanonicalForm(initialTableau)[i, j];

                    if (value != 0)
                    {
                        double roundedValue = Math.Round(value, 3);
                        Console.Write(roundedValue + " \t");
                    }
                    else
                    {
                        
                        Console.Write("0" + " \t");
                    }
                    
                }
                Console.WriteLine("");
            }
        }

        public void ToConsole(bool initialTableau)
        {
            ToConsole("", initialTableau);
        }


        public List<int> nonBasicVariablePos()
        {
            List<int> nonBasicVariablePos = new List<int>();


            int rows = CanonicalForm(false).GetLength(0); // Number of rows
            int columns = CanonicalForm(false).GetLength(1)-1; // Number of columns

            // Loop through each column
            for (int col = 0; col < columns; col++)
            {
                int zeroCount = 0;
                int oneCount = 0;


                // Loop through each row in the current column
                for (int row = 0; row < rows; row++)
                {
                    if (CanonicalForm(false)[row, col] == 0)
                    {
                        zeroCount++;
                    }
                    else if (CanonicalForm(false)[row, col] == 1)
                    {
                        oneCount++;
                      }

                }

                if (zeroCount != rows - 1 || oneCount != 1)
                {
                    nonBasicVariablePos.Add(col);
                }
            }

            return nonBasicVariablePos;
        }
        public List<int> nonBasicVariablePos(bool initialTableau)
        {
            List<int> nonBasicVariablePos = new List<int>();


            int rows = CanonicalForm(initialTableau).GetLength(0); // Number of rows
            int columns = CanonicalForm(initialTableau).GetLength(1) - 1; // Number of columns

            // Loop through each column
            for (int col = 0; col < columns; col++)
            {
                int zeroCount = 0;
                int oneCount = 0;


                // Loop through each row in the current column
                for (int row = 0; row < rows; row++)
                {
                    if (CanonicalForm(initialTableau)[row, col] == 0)
                    {
                        zeroCount++;
                    }
                    else if (CanonicalForm(initialTableau)[row, col] == 1)
                    {
                        oneCount++;
                    }

                }

                if (zeroCount != rows - 1 || oneCount != 1)
                {
                    nonBasicVariablePos.Add(col);
                }
            }

            return nonBasicVariablePos;
        }
        public List<int> BasicVariablePos()
        {
            List<int> basicVariablePos = new List<int>();


            int rows = CanonicalForm(false).GetLength(0); // Number of rows
            int columns = CanonicalForm(false).GetLength(1)-1; // Number of columns

            // Loop through each column
            for (int col = 0; col < columns; col++)
            {
                int zeroCount = 0;
                int oneCount = 0;


                // Loop through each row in the current column
                for (int row = 0; row < rows; row++)
                {
                    if (CanonicalForm(false)[row, col] == 0)
                    {
                        zeroCount++;
                    }
                    else if (CanonicalForm(false)[row, col] == 1)
                    {
                        oneCount++;
                    }
                    
                }

                if (zeroCount == rows - 1 && oneCount == 1)
                {
                    basicVariablePos.Add(col);
                }
            }

            return basicVariablePos;
        }

        public List<int> BasicVariablePos(bool initialTableau)
        {
            List<int> basicVariablePos = new List<int>();


            int rows = CanonicalForm(initialTableau).GetLength(0); // Number of rows
            int columns = CanonicalForm(initialTableau).GetLength(1) - 1; // Number of columns

            // Loop through each column
            for (int col = 0; col < columns; col++)
            {
                int zeroCount = 0;
                int oneCount = 0;


                // Loop through each row in the current column
                for (int row = 0; row < rows; row++)
                {
                    if (CanonicalForm(initialTableau)[row, col] == 0)
                    {
                        zeroCount++;
                    }
                    else if (CanonicalForm(initialTableau)[row, col] == 1)
                    {
                        oneCount++;
                    }

                }

                if (zeroCount == rows - 1 && oneCount == 1)
                {
                    basicVariablePos.Add(col);
                }
            }

            return basicVariablePos;
        }
        public TableauModel GetDualTableau()
        {
            // Number of primal constraints becomes the number of dual variables and vice versa
            int numDualVariables = NumberOfMaxConstraints + NumberOfMinConstraints;
            int numDualConstraints = NumberOfVariables;

            // Transpose the constraint matrix
            double[,] dualMaxConstraintMatrix = new double[numDualConstraints, numDualVariables];
            double[,] dualMinConstraintMatrix = new double[numDualConstraints, numDualVariables];

            for (int i = 0; i < numDualConstraints; i++)
            {
                for (int j = 0; j < NumberOfMaxConstraints; j++)
                {
                    dualMaxConstraintMatrix[i, j] = MaxConstraintMatrix[j, i];
                }
                for (int j = 0; j < NumberOfMinConstraints; j++)
                {
                    dualMinConstraintMatrix[i, j] = MinConstraintMatrix[j, i];
                }
            }

            // The dual's right-hand side becomes the primal's objective function
            double[] dualRightHandSide = new double[numDualConstraints];
            Array.Copy(ObjectiveFunction, dualRightHandSide, numDualConstraints);

            // The dual's objective function coefficients are the right-hand side values of the primal
            double[] dualObjectiveFunction = new double[numDualVariables];
            Array.Copy(RightHandSide, 1, dualObjectiveFunction, 0, numDualVariables); // Skip the first value as it's usually related to the primal objective function.

            // Create and return the dual tableau
            return new TableauModel(dualMaxConstraintMatrix, dualMinConstraintMatrix, dualRightHandSide, dualObjectiveFunction);
        }

    }

}
