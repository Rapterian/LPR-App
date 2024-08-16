using System;
using LPR_App;
using MathNet.Numerics.Optimization;

namespace LPR_App
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Linear Programming Solver!");

                // Display menu options
                Console.WriteLine("1. Solve using Primal Simplex Algorithm");
                Console.WriteLine("2. Solve using Revised Primal Simplex Algorithm");
                Console.WriteLine("3. Solve using Branch & Bound Simplex Algorithm");
                Console.WriteLine("4. Solve using Cutting Plane Algorithm");
                Console.WriteLine("5. Solve using Branch & Bound Knapsack Algorithm");
                Console.WriteLine("6. Read Problem from File (Please see commented out code)");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                // default problem
                double[,] A = { { 1, 1 }, { 2, 1 }, { 1, 3 } };
                double[] b = { 0, 4, 5, 6 };
                double[] c = { 3, 2 };
                
                TableauModel model = null;

                switch (choice)
                {
                    case 1: //primal simplex
                        //string problemType;
                        //double[] objectiveFunction;
                        //double[,] constraintMatrix;
                        //double[] rightHandSide;
                        //string[] constraintRelations;
                        //string[] signRestrictions;

                        //FileHandling.ReadFromFile(out problemType, out objectiveFunction, out constraintMatrix, out rightHandSide, out constraintRelations, out signRestrictions);

                        //// adding problem to TableauModel
                        //TableauModel rubricProblem = new TableauModel(constraintMatrix, rightHandSide, objectiveFunction);

                        model = GetSampleModel();
                        model = Algorithms.PrimalSimplex(model);
                        model.ToConsole("Optimal Solution:", false);
                        break;

                    case 2: //revised primal simplex
                        //Still to be implemented
                        break;

                    case 3: //branch & bound simplex
                        Algorithms.BranchBoundSimplex(A, b, c);
                        break;

                    case 4: //cutting plane
                        Algorithms.CuttingPlane(A, b, c);
                        break;

                    case 5: //branch & bound knapsack
                        // Sample data for Branch & Bound Knapsack
                        double[] weight = { 12, 2, 1, 1, 4 };
                        double[] value = { 4, 2, 2, 1, 10 };
                        double weightLimit = 15;
                        Algorithms.BranchBoundKnapsack(value, weight, weightLimit);
                        break;

                    case 6: // read file
                        //FileHandling.ReadFromFile(out problemType, out objectiveFunction, out constraintMatrix, out rightHandSide, out constraintRelations, out signRestrictions);

                        //rubricProblem = new TableauModel(constraintMatrix, rightHandSide, objectiveFunction);
                        //rubricProblem.ToConsole("Loaded Problem:", true);
                        break;

                    case 7: //exit
                        Console.WriteLine("Exiting the program.");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine("Would you like to perform a sensitivity analysis?");
                Console.WriteLine("Choose an option: ");
                Console.WriteLine("1. Yes");
                Console.WriteLine("2. No");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        // Placeholder for sensitivity analysis
                        Console.WriteLine("Sensitivity Analysis not implemented yet.");
                        break;

                    case 2:
                        Console.WriteLine("Exiting the program.");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.ReadLine();
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
            double[] c = { -3, -2 };

            //double[,] solution = Algorithms.PrimalSimplex(A, b, c);
            //Algorithms.displayTableau(solution, c.Length, b.Length-1, "Optimal Solution:");

            //Primal Simplex
            TableauModel model = new TableauModel(A, b, c);
            model = Algorithms.PrimalSimplex(model);
            model.ToConsole("Optimal Solution:", false);

            //Primal simplex revised
            double[,] D = {
                { 8, 6, 1 },
                { 4, 2, 1.5 },
                { 2, 1.5, 0.5 }
              };

            double[] e = { 0, 48, 20, 8 };
            double[] f = { 60, 30, 20 };
            TableauModel anotherModel = new TableauModel(D, e, f);
            anotherModel = Algorithms.PrimalSimplexRevised(anotherModel);
            anotherModel.ToConsole("Optimal Solution:", false);

            //Cutting Plane Simplex
            Algorithms.CuttingPlane(A, b, c);


            double[] weight = { 12, 2, 1, 1, 4 };
            double[] value = { 4, 2, 2, 1, 10 };
            double weightLimit = 15;

            Algorithms.BranchBoundKnapsack(value, weight, weightLimit);

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
                Console.Write(item + "\t");
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
                    Console.Write(B[i, j] + "\t");
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
            Console.WriteLine();
            Console.WriteLine("===========");
            Console.WriteLine("RHS ranges:");
            Console.WriteLine("===========");
            double[] rhsRanges;
            for (int i = 0; i < model.NumberOfConstraints(); i++)
            {
                // Calculate RHS ranges for constraint i
                rhsRanges = SensitivityAnalysis.rangeRHS(initialModel, model, i);

                // Display the RHS range for the current constraint
                Console.WriteLine($"RHS {i}:\nUpper Range: {rhsRanges[0]}\nLower Range: {rhsRanges[1]}\n");
            }

            Console.WriteLine("=============================");
            Console.WriteLine("Objective Coefficient Ranges:");
            Console.WriteLine("=============================");
            for (int i = 0; i < model.NumberOfVariables; i++)
            {
                double[] objectiveRanges;
                if (model.BasicVariablePos().Contains(i))
                {
                    objectiveRanges = SensitivityAnalysis.rangeObjectiveCoefficient(initialModel, model, i, false);
                }
                else
                {
                    objectiveRanges = SensitivityAnalysis.rangeObjectiveCoefficient(initialModel, model, i, true);
                }

                Console.WriteLine($"Objective Coefficient {i}:\nUpper Range: {objectiveRanges[0]}\nLower Range: {objectiveRanges[1]}\n");
            }

            Console.WriteLine("==============================");
            Console.WriteLine("Constraint Coefficient Ranges:");
            Console.WriteLine("==============================");
            for (int i = 0; i < model.NumberOfConstraints(); i++)
            {
                for (int j = 0; j < model.NumberOfVariables; j++)
                {
                    double[] constraintRanges = SensitivityAnalysis.rangeConstraintCoefficient(initialModel, model, i, j);
                    Console.WriteLine($"Constraint {i}, Variable {j}:\nUpper Range: {constraintRanges[0]}\nLower Range: {constraintRanges[1]}\n");
                }
            }

            Console.ReadLine();
        }
    }
}

