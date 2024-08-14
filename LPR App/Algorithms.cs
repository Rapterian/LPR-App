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
            int numberOfConstraints = tableau.NumberOfMaxConstraints;
            int numberOfVariables = tableau.NumberOfVariables;
            double[,] tableauC = tableau.CanonicalForm(true);
            int iteration = 0;
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
                TableauModel tableauIteration = new TableauModel(tableauC, numberOfVariables, numberOfConstraints);
                tableauIteration.ToConsole($"Iteration {iteration}", false);

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

                iteration++;
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

        public static void BranchBoundKnapsack(double[] value, double[] weight, double weightLimit)
        {

            //Johannes
            Dictionary<List<BranchAndBoundItemModel>, bool> branches = new Dictionary<List<BranchAndBoundItemModel>, bool>();

            List<BranchAndBoundItemModel> items = new List<BranchAndBoundItemModel>();

            for (int i = 0; i < value.Length; i++)
            {
                items.Add(new BranchAndBoundItemModel { Value = value[i], Weight = weight[i], Ratio = value[i] / weight[i], Name = $"X{i + 1}" });
            }

            branches.Add(items, false);

            while (true)
            {
                int unsolvedCounters = 0;
                foreach (var branch in branches)
                {
                    if (branch.Value == false)
                    {
                        unsolvedCounters++;
                        break;
                    }
                }
                if (unsolvedCounters == 0)
                {
                    break;
                }

                List<List<BranchAndBoundItemModel>> keys = new List<List<BranchAndBoundItemModel>>(branches.Keys);

                Dictionary<List<BranchAndBoundItemModel>, bool> branchesWithSubProblems = branches;

                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    if (branches[key] == false)
                    {
                        List<BranchAndBoundItemModel> parentBranch = solveBranch(key, weightLimit);



                        foreach (var item in parentBranch)
                        {
                            if (item.IsSelected != 0 && item.IsSelected != 1)
                            {
                                branchesWithSubProblems = addSubBracnhes(branchesWithSubProblems, parentBranch);
                                break;
                            }
                        }
                        branches[key] = true;
                    }
                }

                branches = branchesWithSubProblems;
            }

            


        }

        private static List<BranchAndBoundItemModel> solveBranch(List<BranchAndBoundItemModel> items, double weightLimit)
        {
            List<BranchAndBoundItemModel> locked = new List<BranchAndBoundItemModel>();
            List<BranchAndBoundItemModel> result = new List<BranchAndBoundItemModel>();
            foreach (BranchAndBoundItemModel item in items)
            {
                result.Add(new BranchAndBoundItemModel { Value = item.Value, Weight = item.Weight, Ratio = item.Ratio, Name = item.Name, IsSelected = item.IsSelected, Locked = item.Locked });
            }

            bool hasSubProblem = false;
            foreach (BranchAndBoundItemModel i in result)
            {
                if (i.Locked)
                {
                    locked.Add(i);
                    weightLimit -= i.Weight * i.IsSelected;
                }
            }
            foreach (BranchAndBoundItemModel i in locked)
            {
                result.Remove(i);
            }

            result = result.OrderByDescending(x => x.Ratio).ToList();

            foreach (BranchAndBoundItemModel item in result)
            {
                item.IsSelected = 0;
            }

            foreach (BranchAndBoundItemModel item in result)
            {
                if (weightLimit>0)
                {
                    if (weightLimit - item.Weight >= 0)
                    {
                        weightLimit -= item.Weight;
                        item.IsSelected = 1;
                    }
                    else
                    {
                        item.IsSelected = weightLimit / item.Weight;
                        hasSubProblem = true;
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Infeasible solution found");
                }
            }

            result.AddRange(locked);

            if (!hasSubProblem && weightLimit>=0)
            {
                Console.WriteLine("");
                double totalValue = 0;
                foreach (BranchAndBoundItemModel item in result)
                {
                    totalValue += item.IsSelected * item.Value;
                }
                Console.WriteLine($"Candidate solution:" + totalValue);
                Console.WriteLine("Remaining weight:"+weightLimit);
                displayKnapsackTable(result);
            }


            return result;
        }

        private static void displayKnapsackTable(List<BranchAndBoundItemModel> items)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Name\t|Selected|Weight|Value|Locked");
            Console.WriteLine("======================================");
            foreach (BranchAndBoundItemModel item in items)
            {
                Console.WriteLine(item.ToString());
                Console.WriteLine("--------------------------------------");
            }
        }

        private static Dictionary<List<BranchAndBoundItemModel>, bool> addSubBracnhes(Dictionary<List<BranchAndBoundItemModel>, bool> branches, List<BranchAndBoundItemModel> parentBracnh)
        {
            List<BranchAndBoundItemModel> subProblem1 = new List<BranchAndBoundItemModel>();
            List<BranchAndBoundItemModel> subProblem2 = new List<BranchAndBoundItemModel>();

            foreach (BranchAndBoundItemModel item in parentBracnh)
            {
                subProblem1.Add(new BranchAndBoundItemModel { Value = item.Value, Weight = item.Weight, Ratio = item.Ratio, Name = item.Name, IsSelected = item.IsSelected, Locked = item.Locked });
                subProblem2.Add(new BranchAndBoundItemModel { Value = item.Value, Weight = item.Weight, Ratio = item.Ratio, Name = item.Name, IsSelected = item.IsSelected, Locked = item.Locked });
            }

            foreach (BranchAndBoundItemModel item in subProblem1)
            {
                if (item.IsSelected != 0 && item.IsSelected != 1)
                {
                    item.IsSelected = 0;
                    item.Locked = true;
                }
            }

            foreach (BranchAndBoundItemModel item in subProblem2)
            {
                if (item.IsSelected != 0 && item.IsSelected != 1)
                {
                    item.IsSelected = 1;
                    item.Locked = true;
                }
            }

            if (!branches.ContainsKey(subProblem1))
            {
                branches.Add(subProblem1, false);
            }
            if (!branches.ContainsKey(subProblem2))
            {
                branches.Add(subProblem2, false);
            }

            branches[parentBracnh] = true;

            return branches;
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
            model.ToConsole("Initial Tableau",true);
            model = Algorithms.PrimalSimplex(model);
            model.ToConsole("Primal Simplex Optimal Solution:",false);

            double[,] primalOptimal = model.CanonicalForm(false);

            //linking variables with values primal simplex optimal values
            double[,] variableAnswers = GetXValues(model, stCoefficients.Length, primalOptimal);

            //checking if primal simplex solution already has x-values that are all integers
            bool allIntegers = AllIntegers(variableAnswers);
            if (allIntegers)
            {
                Console.WriteLine("The Primal Simplex optimal values are all integers, therefore there is no need to continue with the Cutting Plane Simplex Algorithm.");
            }
            else
            {
                double selectedConstraint = SelectConstraint(variableAnswers);
                Console.WriteLine();
                Console.WriteLine("We will select constraint " + selectedConstraint);
            }
        }

        void SensitivityAnalysis()
        {
            //Everyone
        }
    }
}
