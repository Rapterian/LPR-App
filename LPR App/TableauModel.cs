using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            RightHandSide = rightHandSide;
            ObjectiveFunction = objectiveFunction;
            NumberOfVariables = ObjectiveFunction?.Length ?? 0;
            NumberOfMaxConstraints = RightHandSide?.Length - 1 ?? 0;
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



        public double[,] CanonicalForm(bool initialTableau)
        {

            //Initialize tableau
            double[,] tableau = new double[NumberOfMaxConstraints + 1, NumberOfVariables + NumberOfMaxConstraints + 1];//2D array with number of constraints rows and number of variables + number of constraints(slack variables) columns

            if (initialTableau)
            {
                //Objective Function Row
                for (int j = 0; j < NumberOfVariables; j++)
                {
                    tableau[0, j] = -ObjectiveFunction[j];//make the z row variables negative
                }

                tableau[0, NumberOfVariables + NumberOfMaxConstraints] = RightHandSide[0];//Objective Function Row RHS

                //Constraint Rows
                for (int i = 0; i < NumberOfMaxConstraints; i++)
                {
                    for (int j = 0; j < NumberOfVariables; j++)
                    {
                        tableau[i + 1, j] = MaxConstraintMatrix[i, j];//put the constraint matrix values in the tableau row by row
                    }
                    tableau[i + 1, NumberOfVariables + i] = 1; //Slack Variables
                    tableau[i + 1, NumberOfVariables + NumberOfMaxConstraints] = RightHandSide[i + 1];//RHS values
                }
            }
            else
            {
                for (int j = 0; j < NumberOfMaxConstraints + NumberOfVariables; j++)
                {
                    tableau[0, j] = ObjectiveFunction[j];
                }

                for (int i = 1; i < NumberOfMaxConstraints + 1; i++)
                {
                    
                    for (int j = 0; j < NumberOfMaxConstraints + NumberOfVariables ; j++)
                    {
                        tableau[i, j] = MaxConstraintMatrix[i-1, j];
                    }
                    
                }
                for (int i = 0; i < NumberOfMaxConstraints + 1; i++)
                {
                    tableau[i, NumberOfMaxConstraints + NumberOfVariables] = RightHandSide[i];
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
        public void ToConsole(String name, bool initialTableau)
        {
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
            for (int i = NumberOfMaxConstraints; i < NumberOfMaxConstraints + NumberOfVariables + 1; i++)
            {
                Console.Write($"s{s} \t");
                s++;
            }
            Console.WriteLine("RHS");
            for (int i = 0; i < NumberOfMaxConstraints + 1; i++)
            {
                for (int j = 0; j < NumberOfMaxConstraints + NumberOfVariables + 1; j++)
                {
                    Console.Write(CanonicalForm(initialTableau)[i, j] + " \t");
                }
                Console.WriteLine("");
            }
        }

        public void ToConsole(bool initialTableau)
        {
            ToConsole("", initialTableau);
        }
    }
}
