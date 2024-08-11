﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    internal class Algorithms
    {
        /// <summary>
        /// This function will display the tableau in the console whithout the name
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numberOfVariables"></param>
        /// <param name="numberOfConstraints"></param>
        public static void displayTableau(double[,] tableau, int numberOfVariables, int numberOfConstraints)
        {
            for (int i = 0; i < numberOfConstraints-1; i++)
            {
                Console.Write($"x{i + 1} \t");
            }
            int s = 1;
            for (int i = numberOfConstraints; i < numberOfConstraints + numberOfVariables+1; i++)
            {
                Console.Write($"s{s} \t");
                s++;
            }
            Console.WriteLine("RHS");
            for (int i = 0; i < numberOfVariables + 2; i++)
            {
                for (int j = 0; j < numberOfConstraints + numberOfVariables + 1; j++)
                {
                    Console.Write(tableau[i, j] + " \t");
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// This function will display the tableau in the console with the name
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numberOfVariables"></param>
        /// <param name="numberOfConstraints"></param>
        /// <param name="name"></param>
        public static void displayTableau(double[,] tableau, int numberOfVariables, int numberOfConstraints,String name)
        {
            for(int i = 0; i < name.Length; i++)
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
            for (int i = 0; i < numberOfConstraints - 1; i++)
            {
                Console.Write($"x{i + 1} \t");
            }
            int s = 1;
            for (int i = numberOfConstraints; i < numberOfConstraints + numberOfVariables + 1; i++)
            {
                Console.Write($"s{s} \t");
                s++;
            }
            Console.WriteLine("RHS");
            for (int i = 0; i < numberOfVariables + 2; i++)
            {
                for (int j = 0; j < numberOfConstraints + numberOfVariables + 1; j++)
                {
                    Console.Write(tableau[i, j] + " \t");
                }
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// This function will perform the primal simplex algorithm on the given tableau
        /// </summary>
        /// <param name="constraintMatrix"></param>
        /// <param name="RHS"></param>
        /// <param name="stCoefficients"></param>
        /// <returns>Solved Tablue</returns>
        /// <exception cref="Exception"></exception>
        public static double[,] PrimalSimplex(double[,] constraintMatrix, double[] RHS, double[] stCoefficients)
        {
            int numberOfConstraints = RHS.Length-1;
            int numberOfVariables = stCoefficients.Length;

            //Initialize tableau
            double[,] tableau = canonicalForm(constraintMatrix,RHS,stCoefficients);

            displayTableau(tableau, numberOfVariables, numberOfConstraints, "Canonical Form:");

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

        /// <summary>
        /// This function will convert the given input into canonical form
        /// </summary>
        /// <param name="constraintMatrix"></param>
        /// <param name="RHS"></param>
        /// <param name="stCoefficients"></param>
        /// <returns>canonical form</returns>
        private static double[,] canonicalForm(double[,] constraintMatrix, double[] RHS, double[] stCoefficients)
        {
            int numberOfConstraints = RHS.Length-1;
            int numberOfVariables = stCoefficients.Length;

            //Initialize tableau
            double[,] tableau = new double[numberOfConstraints + 1, numberOfVariables + numberOfConstraints + 1];//2D array with number of constraints rows and number of variables + number of constraints(slack variables) columns

            //Objective Function Row
            for (int j = 0; j < numberOfVariables; j++)
            {
                tableau[0, j] = -stCoefficients[j];//make the z row variables negative
            }

            tableau[0, numberOfVariables + numberOfConstraints] = RHS[0];//Objective Function Row RHS

            //Constraint Rows
            for (int i = 0; i < numberOfConstraints; i++)
            {
                for (int j = 0; j < numberOfVariables; j++)
                {
                    tableau[i + 1, j] = constraintMatrix[i, j];//put the constraint matrix values in the tableau row by row
                }
                tableau[i + 1, numberOfVariables + i] = 1; //Slack Variables
                tableau[i + 1, numberOfVariables + numberOfConstraints] = RHS[i + 1];//RHS values
            }

            return tableau;
        }

        public static void RevisedPrimalSimplex(string[] objFunc, string[] constraints, string[] restrictions, double[] basicVariables)
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

        public static double[,] getXValues(double[,] tableau, int numVariables)
        { //returns a matrix with column number (correlates to x-subscript) and variable value)
            int rows = tableau.GetLength(0);
            int columns = tableau.GetLength(1);
            int rhsColumn = columns - 1;

            double[,] result = new double[numVariables, 2];

            for (int j = 0; j < numVariables; j++) {
                int oneCount = 0;
                double valueInLastColumn = 0;

                //checking if BV
                for (int i = 0; i < rows; i++) {
                    if (tableau[i, j] == 1) {
                        oneCount++;
                        valueInLastColumn = tableau[i, rhsColumn];
                    }
                }

                //if BV then updates result matrix with RHS value in the same row
                if (oneCount == 1) {
                    result[j, 0] = j + 1;
                    result[j, 1] = valueInLastColumn; 
                } else { //otherwise, updates result matrix with 0 as variable value
                    result[j, 0] = 0;
                    result[j, 1] = 0;
                }
            }

            return result;
        }

        public static void CuttingPlane(double[,] constraintMatrix, double[] RHS, double[] stCoefficients, int numberOfVariables, int numberOfConstraints, String name)
        { //performs cutting plane simplex algorithm
            double[,] primalSolution = Algorithms.PrimalSimplex(constraintMatrix, RHS, stCoefficients);
            Console.WriteLine();
            Algorithms.displayTableau(primalSolution, stCoefficients.Length, RHS.Length, "Primal Simplex Optimal Solution:");

            //linking variables with values primal simplex optimal values
            double[,] variableAnswers = getXValues(primalSolution, numberOfVariables);

            //checking if primal simplex solution already has x-values that are all integers
            bool allIntegers = true;
            for (int i = 0; i < variableAnswers.GetLength(0); i++) {
                if (variableAnswers[i, 1] != Math.Floor(variableAnswers[i, 1])) {
                    allIntegers = false;
                    break;
                }
            }

            if (allIntegers) {
                Console.WriteLine("The Primal Simplex optimal values are all integers, therefore there is no need to continue with the Cutting Plane Simplex Algorithm.");
            } else {
                double closestToHalf = double.MaxValue;
                double closestValue = 0;

                for (int i = 0; i < variableAnswers.GetLength(0); i++) {
                    double decimalPart = variableAnswers[i, 1] - Math.Floor(variableAnswers[i, 1]);
                    double difference = Math.Abs(decimalPart - 0.5);

                    if (difference < closestToHalf) {
                        closestToHalf = difference;
                        closestValue = variableAnswers[i, 1];
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Selected value to force as an integer is " + closestValue);
            }
        }

        void SensitivityAnalysis()
        {
            //Everyone
        }
    }
}
