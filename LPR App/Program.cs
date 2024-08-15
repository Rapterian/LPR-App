using Sytem;
﻿using LPR_App;

namespace LPR_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Linear Programming Solver!");

            // Display menu options
            Console.WriteLine("1. Solve using Primal Simplex Algorithm");
            Console.WriteLine("2. Solve using Branch & Bound Simplex Algorithm");
            Console.WriteLine("3. Solve using Cutting Plane Algorithm");
            Console.WriteLine("4. Perform Sensitivity Analysis");
            Console.WriteLine("5. Exit");

            // Example data for testing the algorithms
            double[,] A = {
                { 1, 1 },
                { 2, 1 },
                { 1, 3 }
              };

            double[] b = { 0, 4, 5, 6 };
            double[] c = { 3, 2 };

            //double[,] solution = Algorithms.PrimalSimplex(A, b, c);
            //Algorithms.displayTableau(solution, c.Length, b.Length-1, "Optimal Solution:");

            //Primal Simplex
            TableauModel model = new TableauModel(A,b,c);
            model = Algorithms.PrimalSimplex(model);
            model.ToConsole("Optimal Solution:", false);

            //Cutting Plane Simplex
            Algorithms.CuttingPlane(A, b, c);


            double[] weight = { 12, 2, 1, 1, 4 };
            double[] value = { 4, 2, 2, 1, 10 };
            double weightLimit = 15;

            Algorithms.BranchBoundKnapsack(value,weight , weightLimit);

            double[,] tableau = model.MaxConstraintMatrix;

            double[] result = Algorithms.GetNonBasicVariableRange(tableau, model.NumberOfVariables, model.NumberOfMaxConstraints, 0);

            Console.WriteLine($"Allowable Increase: {result[0]}");
            Console.WriteLine($"Allowable Decrease: {result[1]}");

            Console.ReadLine();

        }
    }
}

