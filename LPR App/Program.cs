using System;
using LPR_App;

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

            TableauModel initialModel = new TableauModel(A, b, c);
            initialModel.ToConsole("Initial Tableau:", true);

            model.ToConsole("Optimal Solution:", false);
            Console.WriteLine();

            double[] cbv = SensitivityAnalysis.GetCBV(initialModel, model);
            Console.WriteLine("====");
            Console.WriteLine("CBV:");
            Console.WriteLine("====");
            foreach (var item in cbv)
            {
                Console.Write(item+"\t");
            }
            Console.WriteLine();
            double[,] B = SensitivityAnalysis.GetB(initialModel, model);
            Console.WriteLine("==");
            Console.WriteLine("B:");
            Console.WriteLine("==");
            for (int i = 0; i < B.GetLength(0); i++)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    Console.Write(B[i,j] + "\t");
                }
                Console.WriteLine();
            }
            double[,] BInverse = SensitivityAnalysis.GetBInverse(initialModel, model);
            Console.WriteLine("==========");
            Console.WriteLine("B Inverse:");
            Console.WriteLine("==========");
            for (int i = 0; i < BInverse.GetLength(0); i++)
            {
                for (int j = 0; j < BInverse.GetLength(1); j++)
                {
                    Console.Write(BInverse[i, j] + "\t");
                }
                Console.WriteLine();
            }
            

            double[] cbvDotB = SensitivityAnalysis.GetDotProduct(initialModel, model);
            Console.WriteLine("================");
            Console.WriteLine("CBV * B Inverse:");
            Console.WriteLine("================");
            foreach (var item in cbvDotB)
            {
                Console.Write(item + "\t");
            }
            Console.WriteLine();


            Console.ReadLine();

        }
        
    }
}

