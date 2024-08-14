using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    internal class Algorithms
    {
        public static TableauModel PrimalSimplex(TableauModel tableau)
        {
            int numberOfConstraints = tableau.NumberOfConstraints;
            int numberOfVariables = tableau.NumberOfVariables;
            double[,] tableauC = tableau.CanonicalForm();

            while (true)
            {
                //step 1 - check for optimality
                int pivotColumn = -1;

                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)//go through each column (number of variables + number of constraints to include slack variables)
                {
                    if (tableauC[0, j] < 0)
                    {
                        pivotColumn = j;
                        break;
                    }
                }

                if (pivotColumn == -1) break;//Optimal

                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)//go through each column (number of variables + number of constraints to include slack variables)
                {
                    if (tableauC[0, j] < tableauC[0, pivotColumn])
                    {
                        pivotColumn = j;
                    }
                }

                //step 2 - find pivot row
                int pivotRow = -1;
                double minRatio = double.MaxValue; //constant that represents the largest possible value for a double

                for (int i = 1; i < numberOfConstraints + 1; i++)//start at one since z row can't be a pivot row
                {
                    if (tableauC[i, pivotColumn] > 0)//only want positive values since anything divided by a negative is negative and we want smallest positive value of the ratio
                    {
                        double ratio = tableauC[i, numberOfVariables + numberOfConstraints] / tableauC[i, pivotColumn];//RHS value / pivot column value
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1) throw new Exception("Unbounded"); //Unbounded

                //step 3 - pivot
                double pivotValue = tableauC[pivotRow, pivotColumn];

                for (int j = 0; j < numberOfVariables + numberOfConstraints + 1; j++)
                {
                    tableauC[pivotRow, j] = tableauC[pivotRow, j] / pivotValue;
                }

                for (int i = 0; i < numberOfConstraints + 1; i++)
                {
                    if (i != pivotRow)
                    {
                        double ratio = tableauC[i, pivotColumn];
                        for (int j = 0; j < numberOfVariables + numberOfConstraints + 1; j++)
                        {
                            tableauC[i, j] -= ratio * tableauC[pivotRow, j];
                        }
                    }
                }
            }

            TableauModel tableauModel = new TableauModel(tableauC, numberOfVariables, numberOfConstraints);

            return tableauModel;

        }

        public static void RevisedPrimalSimplex()
        {
            //Caitlin
        }

        void BranchBoundSimplex()
        {
            //Caitlin
        }

        void BranchBoundKnapsack()
        {
            //Johannes
        }

        /// <summary>
        /// This function will link x-variables with their respective RHS values and constraint rows
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numVariables"></param>
        /// <param name="solution"></param>
        /// <returns>matrix with x-variable, row number and variable value</returns>
        public static double[,] GetXValues(TableauModel tableau, int numVariables, double[,] solution)
        {
            int rows = solution.GetLength(0);
            int columns = solution.GetLength(1);
            int rhsColumn = columns - 1;

            double[,] result = new double[numVariables, 3]; //column 1: x-variable; column 2: RHS value; column 3: constraint row

            for (int j = 0; j < numVariables; j++) 
            {
                int oneCount = 0;
                double valueInLastColumn = 0;
                int rowIndex = 0;

                //checking if BV
                for (int i = 0; i < rows; i++) 
                {
                    if (solution[i, j] == 1) 
                    {
                        oneCount++;
                        rowIndex = i;
                        valueInLastColumn = solution[i, rhsColumn];
                    }
                }

                //if BV then updates result matrix with RHS value in the same row
                if (oneCount == 1) 
                {
                    result[j, 0] = j + 1;
                    result[j, 1] = valueInLastColumn;
                    result[j, 2] = rowIndex + 1;
                } 
                else 
                { //otherwise, updates result matrix with 0 as variable value
                    result[j, 0] = j + 1;
                    result[j, 1] = 0;
                    result[j, 2] = rowIndex + 1;
                }
            }

            return result;
        }

        ///<summary>
        /// This function will check if all x-variables already have integer answers
        /// </summary>
        /// <param name="RHS"></param>
        /// <returns>boolean value which indicates if all answers are already integers or not</returns>
        public static bool AllIntegers(double[,] RHS)
        {
            bool result = true;
            for (int i = 0; i < RHS.GetLength(0); i++)
            {
                if (RHS[i, 1] != Math.Floor(RHS[i, 1]))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        ///<summary>
        /// This function will select the constraint row with the value closest to ,5
        /// </summary>
        /// <param name="answers"></param>
        /// <returns>constraint row used to make new constraint</returns>
        public static double SelectConstraint(double[,] answers)
        {
            double closestToHalf = double.MaxValue;
            double constraintRow = 0;

            for (int i = 0; i < answers.GetLength(0); i++)
            {
                double decimalPart = answers[i, 1] - Math.Floor(answers[i, 1]);
                double difference = Math.Abs(decimalPart - 0.5);

                if (difference < closestToHalf)
                {
                    closestToHalf = difference;
                    constraintRow = answers[i, 2] - 1;
                }
            }

            return constraintRow;
        }

        ///<summary>
        /// This function will perform the cutting plane simplex algorithm
        /// </summary>
        /// <param name="constraintMatrix"></param>
        /// <param name="RHS"></param>
        /// <param name="stCoefficients"></param>
        /// <returns></returns>
        public static void CuttingPlane(double[,] constraintMatrix, double[] RHS, double[] stCoefficients)
        {
            Console.WriteLine("You've selected to solve with the Cutting Plane Simplex Algorithm...");
            Console.WriteLine("First, we must find the optimal values with the Primal Simplex Algorithm:");

            TableauModel model = new TableauModel(constraintMatrix, RHS, stCoefficients);
            model.ToConsole();
            model = Algorithms.PrimalSimplex(model);
            model.ToConsole("Primal Simplex Optimal Solution:");

            double[,] primalOptimal = model.CanonicalForm();

            //linking variables with values primal simplex optimal values
            double[,] variableAnswers = GetXValues(model, stCoefficients.Length, primalOptimal);

            //checking if primal simplex solution already has x-values that are all integers
            bool allIntegers = AllIntegers(variableAnswers);
            if (allIntegers)
            {
                Console.WriteLine("The current x-variable optimal values are all integers, therefore there is no need to continue with the Cutting Plane Simplex Algorithm.");
            }
            else
            {
                int selectedConstraintRow = (int)SelectConstraint(variableAnswers);
                Console.WriteLine();

                //testing
                //Console.WriteLine("We will select constraint " + (double)selectedConstraintRow);

                double[] constraintValues = new double[primalOptimal.GetLength(1)];
                for (int j = 0; j < primalOptimal.GetLength(1); j++)
                {
                    constraintValues[j] = primalOptimal[selectedConstraintRow, j];                  
                }

                //testing
                //double[] coefficents = { 0.0, 1.4, -1.25, 0, 1, 1.4 };
                //double[] newConstraint = CreateNewConstraint(coefficents, (stCoefficients.Length + constraintMatrix.GetLength(1) + stCoefficients.Length - 1), stCoefficients.Length);

                double[] newConstraint = CreateNewConstraint(constraintValues, (stCoefficients.Length + constraintMatrix.GetLength(1) + stCoefficients.Length - 1), stCoefficients.Length);
                //double[,] newConstraintMatrix = new double[constraintMatrix.GetLength(0) + 1, constraintMatrix.GetLength(1)];

                //testing
                for (int i = 0; i < newConstraint.Length; i++)
                {
                    Console.WriteLine(newConstraint[i]);
                }

                double[,] newConstraintMatrix = UpdateConstraintMatrix(constraintMatrix, newConstraint);
                //double[,] newCoefficientMatrix = UpdateCoefficientMatrix(stCoefficients, constraintMatrix.GetLength(1), newConstraint);
                //double[] newRHSMatrix = UpdateRHSMatrix(RHS, stCoefficients.Length + constraintMatrix.GetLength(1), newConstraint, );

                for (int i = 0; i <constraintMatrix.GetLength(0); i++)
                {
                    for (int j = 0;j < newConstraintMatrix.GetLength(1);j++)
                        Console.WriteLine(constraintMatrix[i,j]);
                }


                //model = new TableauModel()

                //double[] coefficents = { 0.0, 1.4, -1.25, 0, 1, 1.4 };  
                //CreateNewConstraint(coefficents, (stCoefficients.Length + constraintMatrix.GetLength(1) + stCoefficients.Length - 1), stCoefficients.Length);
            }
        }

        public static double[] ExtractDecimalPart(double[] coefficients)
        {
            double[] result = new double[coefficients.Length];

            //testing
            //Console.WriteLine("Hello");
            //for (int i = 0; i < coefficients.Length; i++)
            //{
            //    Console.WriteLine(coefficients[i]);
            //}
            //Console.WriteLine("Hello");

            for (int i = 0; i < coefficients.Length; i++)
            {
                double decimalPart = coefficients[i] - Math.Truncate(coefficients[i]);

                // Check if the value is a whole number or 0
                if (decimalPart == 0)
                {
                    result[i] = 0;
                }
                else
                {
                    // Handle positive values
                    if (coefficients[i] > 0)
                    {
                        result[i] = Math.Round(decimalPart * -1, 3);
                    }
                    // Handle negative values
                    else
                    {
                        result[i] = Math.Round((1 - Math.Abs(decimalPart)) * -1, 3);
                    }
                }
            }

            //testing
            //Console.WriteLine("Bye");
            //for (int i = 0; i < result.Length; i++)
            //{
            //    Console.WriteLine(result[i]);
            //}
            //Console.WriteLine("Bye");

            return result;
        }

        public static double[] CreateNewConstraint(double[] rowValues, int tableauSize, int numCoefficients)
        {
            double[] result = new double[rowValues.Length + 1];
            double[] decimalConstraint = ExtractDecimalPart(rowValues);

            for (int i = 0; i < decimalConstraint.Length; i++)
            {
                result[i] = decimalConstraint[i];
            }

            result[result.Length - 2] = 1.0;

            // add new slack variable
            result[result.Length - 1] = decimalConstraint[decimalConstraint.Length - 1];

            //testing
            //for (int i = 0; i < result.Length; i++)
            //{
            //    Console.WriteLine(result[i]);
            //}

            return result;
        }

        public static double[,] UpdateConstraintMatrix(double[,] originalMatrix, double[] constraintToAdd)
        {
            double[,] result = new double[originalMatrix.GetLength(0) + 1, originalMatrix.GetLength(1)];

            for (int i = 0; i < originalMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < originalMatrix.GetLength(1); j++)
                {
                    result[i, j] = originalMatrix[i, j];
                }
            }

            // Add the new constraint to the last row of the new constraint matrix
            int lastRow = result.GetLength(0) - 1;
            for (int j = 0; j < originalMatrix.GetLength(1); j++)
            {
                result[result.GetLength(0) - 1, j] = constraintToAdd[j];

            }

            // Testing
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write(result[i, j] + " ");
                }
                Console.WriteLine();
            }

            return result;
        }

        public static double[,] UpdateCoefficientMatrix(double[,] originalMatrix, int numConstraints, double[] constraintToAdd)
        {
            double[,] result = new double[originalMatrix.GetLength(0) + 1, originalMatrix.GetLength(1)];

            for (int i = 0; i < originalMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < originalMatrix.GetLength(1); j++)
                {
                    result[i, j] = originalMatrix[i, j];
                }
            }

            // Add the new constraint to the last row of the new constraint matrix
            int lastRow = result.GetLength(0) - 1;
            for (int j = numConstraints; j < numConstraints + originalMatrix.GetLength(1); j++)
            {
                result[result.GetLength(0) - 1, j] = constraintToAdd[j];

            }

            // Testing
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write(result[i, j] + " ");
                }
                Console.WriteLine();
            }

            return result;
        }

        void SensitivityAnalysis()
        {
            //Everyone
        }
    }
}
