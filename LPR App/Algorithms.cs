using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    public class Algorithms
    {
        /// <summary>
        /// This function will display the tableau in the console whithout the name
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numberOfVariables"></param>
        /// <param name="numberOfConstraints"></param>
        public static void displayTableau(double[,] tableau, int numberOfVariables, int numberOfConstraints)
        {
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
        /// This function will display the tableau in the console with the name
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numberOfVariables"></param>
        /// <param name="numberOfConstraints"></param>
        /// <param name="name"></param>
        public static void displayTableau(double[,] tableau, int numberOfVariables, int numberOfConstraints, String name)
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
            int numberOfConstraints = RHS.Length - 1;
            int numberOfVariables = stCoefficients.Length;

            //Initialize tableau
            double[,] tableau = canonicalForm(constraintMatrix, RHS, stCoefficients);

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
            int numberOfConstraints = RHS.Length - 1;
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

        public static void BranchBoundKnapsack(double[] value, double[] weight, double weightLimit)
        {

            //Johannes
            Dictionary<List<BranchAndBoundItemModel>, bool> branches = new Dictionary<List<BranchAndBoundItemModel>, bool>();

            List<BranchAndBoundItemModel> items = new List<BranchAndBoundItemModel>();

            for (int i = 0; i < value.Length; i++)
            {
                items.Add(new BranchAndBoundItemModel { Value = value[i], Weight = weight[i], Ratio = value[i] / weight[i], Name = $"X {i + 1}" });
            }

            branches.Add(items, false);
           
            while (true)
            {
                int unsolvedCounters = 0;
                foreach(var branch in branches)
                {
                    if(branch.Value == false)
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
                if (weightLimit - item.Weight > 0)
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

            result.AddRange(locked);

            if (!hasSubProblem)
            {
                double totalValue = 0;
                foreach (BranchAndBoundItemModel item in result)
                {
                    totalValue += item.IsSelected * item.Value;
                }
                Console.WriteLine($"Candidate solution:" + totalValue);
            }

            
            return result;
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
