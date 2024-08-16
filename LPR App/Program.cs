using Sytem;
﻿using LPR_App;

namespace LPR_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Linear Programming Solver!");

            while (true)
            {
                // Display menu options
                Console.WriteLine("1. Solve using Primal Simplex Algorithm");
                Console.WriteLine("2. Solve using Branch & Bound Simplex Algorithm");
                Console.WriteLine("3. Solve using Cutting Plane Algorithm");
                Console.WriteLine("4. Perform Sensitivity Analysis");
                Console.WriteLine("5. Read Problem from File");
                Console.WriteLine("6. Write Problem to File");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                TableauModel model = null;

                switch (choice)
                {
                    case 1:
                        model = GetSampleModel();
                        model = Algorithms.PrimalSimplex(model);
                        model.ToConsole("Optimal Solution:", false);
                        break;
                    case 2:
                        // Sample data for Branch & Bound Knapsack
                        double[] weight = { 12, 2, 1, 1, 4 };
                        double[] value = { 4, 2, 2, 1, 10 };
                        double weightLimit = 15;
                        Algorithms.BranchBoundKnapsack(value, weight, weightLimit);
                        break;
                    case 3:
                        // Sample data for Cutting Plane
                        double[,] A = { { 1, 1 }, { 2, 1 }, { 1, 3 } };
                        double[] b = { 0, 4, 5, 6 };
                        double[] c = { 3, 2 };
                        Algorithms.CuttingPlane(A, b, c);
                        break;
                    case 4:
                        // Placeholder for sensitivity analysis
                        Console.WriteLine("Sensitivity Analysis not implemented yet.");
                        break;
                    case 5:
                        Console.Write("Enter the file path to read from: ");
                        string readPath = Console.ReadLine();
                        model = FileHandling.ReadFromFile(readPath);
                        model.ToConsole("Loaded Problem:", true);
                        break;
                    case 6:
                        Console.Write("Enter the file path to save to: ");
                        string writePath = Console.ReadLine();
                        if (model != null)
                        {
                            FileHandling.WriteToFile(writePath, model);
                            Console.WriteLine("Problem saved successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No problem loaded or solved to save.");
                        }
                        break;
                    case 7:
                        Console.WriteLine("Exiting the program.");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static TableauModel GetSampleModel()
        {
            double[,] A = {
                            { 1, 1 },
                            { 2, 1 },
                            { 1, 3 }
                          };

            double[] b = { 0, 4, 5, 6 };
            double[] c = { 3, 2 };

            return new TableauModel(A, b, c);
        }
    }
}

