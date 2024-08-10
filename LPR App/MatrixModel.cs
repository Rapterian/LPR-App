using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    public class MatrixModel
    {
        public double[,] ConstraintMatrix { get; set; }
        public double[] RightHandSide { get; set; }
        public double[] ObjectiveFunction { get; set; }

        public MatrixModel(double[,] constraintMatrix, double[] rightHandSide, double[] objectiveFunction)
        {
            ConstraintMatrix = constraintMatrix;
            RightHandSide = rightHandSide;
            ObjectiveFunction = objectiveFunction;
        }

        public int NumberOfVariables => ObjectiveFunction?.Length ?? 0;

        public int NumberOfConstraints => RightHandSide?.Length ?? 0;
    
        public double[,] CanonicalForm()
        {
            //Initialize tableau
            double[,] tableau = new double[NumberOfConstraints + 1, NumberOfVariables + NumberOfConstraints + 1];//2D array with number of constraints rows and number of variables + number of constraints(slack variables) columns

            //Objective Function Row
            for (int j = 0; j < NumberOfVariables; j++)
            {
                tableau[0, j] = -ObjectiveFunction[j];//make the z row variables negative
            }

            //Constraint Rows
            for (int i = 0; i < NumberOfConstraints; i++)
            {
                for (int j = 0; j < NumberOfVariables; j++)
                {
                    tableau[i + 1, j] = ConstraintMatrix[i, j];//put the constraint matrix values in the tableau row by row
                }
                tableau[i + 1, NumberOfVariables + i] = 1; //Slack Variables
                tableau[i + 1, NumberOfVariables + NumberOfConstraints] = RightHandSide[i];//RHS values
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
        public void ToString(String name)
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
            for (int i = 0; i < NumberOfConstraints - 1; i++)
            {
                Console.Write($"x{i + 1} \t");
            }
            int s = 1;
            for (int i = NumberOfConstraints; i < NumberOfConstraints + NumberOfVariables + 1; i++)
            {
                Console.Write($"s{s} \t");
                s++;
            }
            Console.WriteLine("RHS");
            for (int i = 0; i < NumberOfVariables + 2; i++)
            {
                for (int j = 0; j < NumberOfConstraints + NumberOfVariables + 1; j++)
                {
                    Console.Write(CanonicalForm()[i, j] + " \t");
                }
                Console.WriteLine("");
            }
        }

        public void ToString()
        {
            ToString("");
        }
    }
}
