using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Text;
using LPR_App;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;

namespace LPR_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Linear Programming Solver!");
            while (true)
            {
                Console.WriteLine("====");
                Console.WriteLine("MENU");
                Console.WriteLine("====");
                // Display menu options
                Console.WriteLine("1. Solve using Primal Simplex Algorithm");
                Console.WriteLine("2. Solve using Revised Primal Simplex Algorithm");
                Console.WriteLine("3. Solve using Branch & Bound Simplex Algorithm");
                Console.WriteLine("4. Solve using Cutting Plane Algorithm");
                Console.WriteLine("5. Solve using Branch & Bound Knapsack Algorithm");
                Console.WriteLine("6. Read Problem from File (Please see commented out code)");
                Console.WriteLine("7. Exit");
                Console.WriteLine("*******************");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());
                Console.WriteLine("*******************");

                //max +2 +3 +3 +5 +2 +4
                //+11 + 8 + 6 + 14 + 10 + 10 <= 40


                // default problem
                double[,] A = { { 1, 1 }, { 2, 1 }, { 1, 3 } };
                double[] b = { 0, 4, 5, 6 };
                double[] c = { 3, 2 };



                TableauModel initialModel = new TableauModel(A, b, c);
                TableauModel optimalModel = new TableauModel(A, b, c);

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


                        optimalModel = Algorithms.PrimalSimplex(optimalModel, true);
                        optimalModel.ToConsole("Optimal Solution:", false);
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
                while (choice!=2)
                {
                    Console.WriteLine("#################################################\n");
                    Console.WriteLine("Would you like to perform a sensitivity analysis?\n");
                    Console.WriteLine("#################################################");
                    Console.WriteLine("1. Yes");
                    Console.WriteLine("2. No");
                    Console.WriteLine("*******************");
                    Console.Write("Choose an option: ");
                    choice = int.Parse(Console.ReadLine());
                    Console.WriteLine("*******************");

                    switch (choice)
                    {
                        case 1:
                            // Placeholder for sensitivity analysis
                            sensitivityAnalysisMenu(initialModel, optimalModel);
                            break;

                        case 2:
                            Console.WriteLine();
                            break;

                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    } 
                }
            }
        }

        

        public static void sensitivityAnalysisMenu(TableauModel initualModel, TableauModel optimalModel)
        {

            Console.WriteLine("--------------------");
            Console.WriteLine("Sensitivity Analysis");
            Console.WriteLine("--------------------");
            // Display menu options
            Console.WriteLine("1. Display the range of a selected Non - Basic Variable.");
            Console.WriteLine("2. Apply and display a change of a selected Non - Basic Variable.");
            Console.WriteLine("3. Display the range of a selected Basic Variable.");
            Console.WriteLine("4. Apply and display a change of a selected Basic Variable.");
            Console.WriteLine("5. Display the range of a selected constraint right-hand - side value.");
            Console.WriteLine("6. Apply and display a change of a selected constraint right-hand - side value.");
            Console.WriteLine("7. Display the range of a selected variable in a Non-Basic Variable column.");
            Console.WriteLine("8. Apply and display a change of a selected variable in a Non-Basic Variable column.");
            Console.WriteLine("9. Add a new activity to an optimal solution.");
            Console.WriteLine("10. Add a new constraint to an optimal solution.");
            Console.WriteLine("11. Display the shadow prices.");
            Console.WriteLine("12. Duality");
            Console.WriteLine("13. Cancel");
            Console.WriteLine("*******************");
            Console.Write("Choose an option: ");
            int choice = int.Parse(Console.ReadLine());
            Console.WriteLine("*******************");

            switch (choice)
            {
                case 1:
                    int variablePos = chooseVariable(optimalModel, false);
                    double[] range = SensitivityAnalysis.rangeObjectiveCoefficient(initualModel, optimalModel, variablePos, false);
                    Console.WriteLine(range[0] + " <= DELTA <= " + range[1]);
                    break;

                case 2:
                    variablePos = chooseVariable(optimalModel, false);
                    Console.WriteLine("*******************");
                    Console.Write("Choose a new value: ");
                    double newValue = checkValue(Console.ReadLine());
                    Console.WriteLine("*******************");
                    double[,] newMatrix= SensitivityAnalysis.changeObjectiveCoefficient(initualModel, optimalModel, variablePos, newValue, false);
                    TableauModel tableauModel = new TableauModel(newMatrix,optimalModel.NumberOfVariables,optimalModel.NumberOfMaxConstraints,optimalModel.NumberOfMinConstraints);
                    tableauModel.ToConsole("Optimal Solution:", false);
                    break;

                case 3:
                    variablePos = chooseVariable(optimalModel, true);
                    range = SensitivityAnalysis.rangeObjectiveCoefficient(initualModel, optimalModel, variablePos, true);
                    Console.WriteLine(range[0] + " <= DELTA <= " + range[1]);
                    break;

                case 4:
                    variablePos = chooseVariable(optimalModel, true);
                    Console.WriteLine("*******************");
                    Console.Write("Choose a new value: ");
                    newValue = checkValue(Console.ReadLine());
                    Console.WriteLine("*******************");
                    newMatrix = SensitivityAnalysis.changeObjectiveCoefficient(initualModel, optimalModel, variablePos, newValue, true);
                    tableauModel = new TableauModel(newMatrix, optimalModel.NumberOfVariables, optimalModel.NumberOfMaxConstraints, optimalModel.NumberOfMinConstraints);
                    tableauModel.ToConsole("Optimal Solution:", false);
                    break;

                case 5:
                    int constraintPos = chooseConstraint(optimalModel);
                    range = SensitivityAnalysis.rangeRHS(initualModel, optimalModel, constraintPos);
                    Console.WriteLine(range[0] + " <= DELTA <= " + range[1]);
                    break;

                case 6:
                    constraintPos = chooseConstraint(optimalModel);
                    Console.WriteLine("*******************");
                    Console.Write("Choose a new value: ");
                    newValue = checkValue(Console.ReadLine());
                    Console.WriteLine("*******************");
                    newMatrix = SensitivityAnalysis.changeRHSvalues(initualModel, optimalModel, constraintPos, newValue);
                    tableauModel = new TableauModel(newMatrix, optimalModel.NumberOfVariables, optimalModel.NumberOfMaxConstraints, optimalModel.NumberOfMinConstraints);
                    tableauModel.ToConsole("Optimal Solution:", false);
                    break;

                case 7:
                    variablePos = chooseVariable(optimalModel, false);
                    range = SensitivityAnalysis.rangeConstraintCoefficient(initualModel, optimalModel, 0, 0);
                    Console.WriteLine(range[0] + " <= DELTA <= " + range[1]);
                    break;

                case 8:
                    constraintPos = chooseConstraint(optimalModel);
                    variablePos = chooseVariable(optimalModel, true);
                    Console.WriteLine("*******************");
                    Console.Write("Choose a new value: ");
                    newValue = checkValue(Console.ReadLine());
                    Console.WriteLine("*******************");
                    newMatrix = SensitivityAnalysis.changeConstraintCoefficient(initualModel, optimalModel, constraintPos, variablePos, newValue);
                    tableauModel = new TableauModel(newMatrix, optimalModel.NumberOfVariables, optimalModel.NumberOfMaxConstraints, optimalModel.NumberOfMinConstraints);
                    tableauModel.ToConsole("Optimal Solution:", false);
                    break;

                case 9:
                    Console.WriteLine("*******************");
                    double[] constraits = getConstraints(optimalModel);
                    Console.Write("New variable coeficient value: ");
                    double variable = checkValue(Console.ReadLine());
                    Console.WriteLine("*******************");
                    newMatrix = SensitivityAnalysis.addActivity(initualModel, optimalModel, variable, constraits);
                    tableauModel = new TableauModel(newMatrix, optimalModel.NumberOfVariables, optimalModel.NumberOfMaxConstraints, optimalModel.NumberOfMinConstraints);
                    tableauModel.ToConsole("Optimal Solution:", false);
                    break;

                case 10:
                    Console.WriteLine("*******************");
                    double[] variables = getVariables(optimalModel);
                    Console.Write("New constraint right hand side value: ");
                    double rhs = checkValue(Console.ReadLine());
                    Console.WriteLine("*******************");
                    newMatrix = SensitivityAnalysis.addConstraint(initualModel, optimalModel, variables, rhs);
                    tableauModel = new TableauModel(newMatrix, optimalModel.NumberOfVariables, optimalModel.NumberOfMaxConstraints, optimalModel.NumberOfMinConstraints);
                    tableauModel.ToConsole("Optimal Solution:", false);
                    break;

                case 11:
                    var BInverse = Matrix<double>.Build.DenseOfArray(SensitivityAnalysis.GetBInverse(initualModel, optimalModel));
                    var CBV = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(SensitivityAnalysis.GetCBV(initualModel, optimalModel));
                    double[] shadowprices = (BInverse * CBV).AsArray();
                    Console.WriteLine("Shadow Prices:");
                    for (int i = 0; i < shadowprices.Length; i++)
                    {
                        Console.WriteLine($"Shadow Price {i + 1}: {shadowprices[i]}");
                    }
                    
                    break;

                case 12:
                    Console.WriteLine("Duality:"+SensitivityAnalysis.CheckDuality(initualModel,optimalModel));
                    return;

                case 13:
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    sensitivityAnalysisMenu(initualModel, optimalModel);
                    break;
            }
        }

        public static int chooseVariable(TableauModel model, bool basic)
        {
            int result = 0;
            var variables = new List<int>();
            if (basic)
            {
                variables = model.BasicVariablePos();

            }
            else
            {
                variables = model.nonBasicVariablePos();
            }

            for (int index = 0; index < variables.Count; index++)
            {
                int item = variables[index];

                if (item < model.NumberOfVariables)
                {
                    Console.WriteLine($"{index + 1} X{item + 1}");
                }
                else if (item < model.NumberOfMaxConstraints + model.NumberOfVariables)
                {
                    Console.WriteLine($"{index + 1} S{item - model.NumberOfVariables + 1}");
                }
                else
                {
                    Console.WriteLine($"{index + 1} E{item - model.NumberOfVariables - model.NumberOfMaxConstraints + 1}");
                }
            }
            Console.WriteLine("*******************");
            Console.Write("Choose an variable: ");
            string choice = Console.ReadLine();
            Console.WriteLine("*******************");
            if (int.TryParse(choice, out int choiceInt))
            {
                if (choiceInt > 0 && choiceInt <= variables.Count)
                {
                    result = variables[choiceInt - 1];
                    return result;

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                    chooseVariable(model, basic);

                }
            }
            else
            {

                Console.WriteLine("Invalid input. Please enter a valid integer.");
                chooseVariable(model, basic);

            }
            return result;
        }
        public static int checkChoise(string choice, int highest)
        {
            int result = -1;
            if (int.TryParse(choice, out int choiceInt))
            {
                if (choiceInt > 0 && choiceInt <= highest)
                {
                    result = choiceInt;
                    return result;

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                    checkChoise(choice, highest);

                }
            }
            else
            {

                Console.WriteLine("Invalid input. Please enter a valid integer.");
                checkChoise(choice, highest);

            }
            return result;
        }
        public static double checkValue(string choice)
        {
            double result = -1;
            if (double.TryParse(choice, out double choiceInt))
            { 
                result = choiceInt;
                return result;
            }
            else
            {

                Console.WriteLine("Invalid input. Please enter a valid double.");
                checkValue(choice);

            }
            return result;
        }

        public static int chooseConstraint(TableauModel model)
        {
            int result = 0;
            int numberOfConstraints = model.NumberOfConstraints();

            for (int index = 0; index < numberOfConstraints; index++)
            {
                Console.WriteLine($"{index + 1}");
            }
            Console.WriteLine("***********************");
            Console.Write("Choose an constraint: ");
            string choice = Console.ReadLine();
            Console.WriteLine("***********************");
            if (int.TryParse(choice, out int choiceInt))
            {
                if (choiceInt > 0 && choiceInt <= numberOfConstraints)
                {
                    result = choiceInt - 1;
                    return result;

                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid integer.");
                    chooseConstraint(model);

                }
            }
            else
            {

                Console.WriteLine("Invalid input. Please enter a valid integer.");
                chooseConstraint(model);

            }
            return result;
        }
        static double[] getConstraints(TableauModel tableauModel)
        {
            double[] newValues = new double[tableauModel.NumberOfConstraints()];
            for (int i = 0; i < tableauModel.NumberOfConstraints(); i++)
            {
                Console.Write($"Constraint {i+1}:");
                double newValue = checkValue(Console.ReadLine());
                newValues[i] = newValue;
            }
            return newValues;
        }
        static double[] getVariables(TableauModel tableauModel)
        {
            double[] newValues = new double[tableauModel.NumberOfVariables];
            for (int i = 0; i < tableauModel.NumberOfVariables; i++)
            {
                Console.Write($"Constraint {i + 1}:");
                double newValue = checkValue(Console.ReadLine());
                newValues[i] = newValue;
            }
            return newValues;
        }
        static void GetSampleModel()
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
            model = Algorithms.PrimalSimplex(model, true);
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

