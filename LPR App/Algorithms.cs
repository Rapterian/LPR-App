using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    internal class Algorithms
    {
        public static double[,] PrimalSimplex(double[,] constraintMatrix, double[] RHS, double[] stCoefficients)
        {
            int numberOfConstraints = RHS.Length;
            int numberOfVariables = stCoefficients.Length;

            //Initialize tableau
            double[,] tableau = new double[numberOfConstraints + 1, numberOfVariables + numberOfConstraints + 1];//2D array with number of constraints rows and number of variables + number of constraints(slack variables) columns

            //Objective Function Row
            for (int j = 0; j < numberOfVariables; j++)
            {
                tableau[0, j] = -stCoefficients[j];//make the z row variables negative
            }

            //Constraint Rows
            for (int i = 0; i < numberOfConstraints; i++)
            {
                for (int j = 0; j < numberOfVariables; j++)
                {
                    tableau[i + 1, j] = constraintMatrix[i, j];//put the constraint matrix values in the tableau row by row
                }
                tableau[i + 1, numberOfVariables + i] = 1; //Slack Variables
                tableau[i + 1, numberOfVariables + numberOfConstraints] = RHS[i];//RHS values
            }

            while (true)
            {
                //step 1 - check for optimality
                int pivotColumn = -1;

                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)//go through each column (number of variables + number of constraints to include slack variables)
                {
                    if (tableau[0, j] < 0)
                    {
                        pivotColumn = j;
                        break;
                    }
                }

                if (pivotColumn == -1) break;//Optimal

                for (int j = 0; j < numberOfVariables + numberOfConstraints; j++)//go through each column (number of variables + number of constraints to include slack variables)
                {
                    if (tableau[0, j] < tableau[0, pivotColumn])
                    {
                        pivotColumn = j;
                    }
                }

                //step 2 - find pivot row
                int pivotRow = -1;
                double minRatio = double.MaxValue; //constant that represents the largest possible value for a double

                for (int i = 1; i < numberOfConstraints + 1; i++)//start at one since z row can't be a pivot row
                {
                    if (tableau[i, pivotColumn] > 0)//only want positive values since anything divided by a negative is negative and we want smallest positive value of the ratio
                    {
                        double ratio = tableau[i, numberOfVariables + numberOfConstraints] / tableau[i, pivotColumn];//RHS value / pivot column value
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1) throw new Exception("Unbounded"); //Unbounded

                //step 3 - pivot
                double pivotValue = tableau[pivotRow, pivotColumn];

                for (int j = 0; j < numberOfVariables + numberOfConstraints + 1; j++)
                {
                    tableau[pivotRow, j] = tableau[pivotRow, j] / pivotValue;
                }

                for (int i = 0; i < numberOfConstraints + 1; i++)
                {
                    if (i != pivotRow)
                    {
                        double ratio = tableau[i, pivotColumn];
                        for (int j = 0; j < numberOfVariables + numberOfConstraints + 1; j++)
                        {
                            tableau[i, j] -= ratio * tableau[pivotRow, j];
                        }
                    }
                }
            }

           
            return tableau;

        }

        public static void RevisedPrimalSimplex(string[] objFunc, string[] constraints, string[] restrictions, double[] basicVariables)
        {
            Console.WriteLine("Canonical Form:");

            foreach (string coefficent in objFunc)
            {
                Console.WriteLine("Original ->" + coefficent);
                if (coefficent.StartsWith("+"))
                {
                    Console.WriteLine("Replaced with ->" + coefficent.Replace('+', '-'));
                }

                else if (coefficent.StartsWith("-"))
                {
                    Console.Write(coefficent.Replace('-', '+'));
                }

            }

            Console.WriteLine();






            /*  BV (row)
             *  Xbv (coloumn)
             *  NBV (row)
             *  Xnbv (column)
             *  Co-efficents of BV (row)
             *  Co-efficents of NBV (row)
             *  Matrix of columns for Basic variables
             *  Matrix of columns for Non-basic variables
             *  RHS values
             *  
             *  6 formulas
           */
        }

        void BranchBoundSimplex()
        {
            //Caitlin
        }

        void BranchBoundKnapsack()
        {
            //Johannes
        }

        void CuttingPlane()
        {
            //Caitlin
        }

        void SensitivityAnalysis()
        {
            //Everyone
        }
    }
}
