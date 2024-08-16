using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LPR_App
{
    internal class Algorithms
    {
        public static TableauModel PrimalSimplex(TableauModel tableau)
        {
            if (tableau.NumberOfMinConstraints != 0)
            {
                throw new Exception("Primal Simplex only works with maximization problems");
            }
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

        public static TableauModel PrimalSimplexRevised(TableauModel tableau)
        {
            TableauModel tableau2 = new TableauModel(tableau.CanonicalForm(true).Clone() as double[,], tableau.NumberOfVariables, tableau.NumberOfConstraints());
            double[,] tableauC = new double[tableau.CanonicalForm(true).GetLength(0), tableau.CanonicalForm(true).GetLength(1)];
            
            int itteration = 0;
            var BInverse = Matrix<double>.Build.DenseOfArray(SensitivityAnalysis.GetBInverse(tableau, tableau2));
            var B = SensitivityAnalysis.GetB(tableau, tableau2);
            var CBV = SensitivityAnalysis.GetCBV(tableau, tableau2);
            List<double> exitVariablesPositions = new List<double>();

            while (true)
            {
                double[,] tableauTest = tableau.CanonicalForm(true);
                int rows = BInverse.RowCount;
                int columns = BInverse.ColumnCount;

                double[,] BInverseAsArray = new double[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        BInverseAsArray[i, j] = BInverse[i, j];
                    }
                }
                
                var CBVdotB = SensitivityAnalysis.GetDotProduct(tableau, tableau2, CBV, BInverseAsArray);


                for (int j = 0; j < tableau2.BasicVariablePos().Count; j++)
                {
                    int basicVariablePos = tableau2.BasicVariablePos()[j];

                    tableauC[0, basicVariablePos] = CBVdotB[j];

                    for (int i = 0; i < tableau2.NumberOfConstraints(); i++)
                    {
                        tableauC[i + 1, basicVariablePos] = BInverse[i, j];
                    }
                }

                Dictionary<int, double> nonBasicVariableNewValue = new Dictionary<int, double>();


                for (int i = 0; i < tableau2.nonBasicVariablePos().Count; i++)
                {
                    int nonBasicVariablePos = tableau2.nonBasicVariablePos()[i];

                    nonBasicVariableNewValue.Add(nonBasicVariablePos, SensitivityAnalysis.CalculateNewObjectiveCoefficient(tableau, tableau2, nonBasicVariablePos, tableau2.CanonicalForm(itteration == 0)[0, nonBasicVariablePos],CBV,BInverseAsArray));

                    tableauC[0, nonBasicVariablePos] = nonBasicVariableNewValue.ElementAt(i).Value;
                }


                int entryVariablePos = -1;
                double minValue = double.MaxValue;

                foreach (var kvp in nonBasicVariableNewValue)
                {
                    if (kvp.Value < minValue)
                    {
                        minValue = kvp.Value;
                        entryVariablePos = kvp.Key;
                    }
                }

                if (minValue >= 0)
                {
                    break;
                }

                var Ai = Vector<double>.Build.DenseOfArray(SensitivityAnalysis.GetAi(tableau, entryVariablePos));
                var Bi = BInverse * Ai;

                for (int i = 0; i < tableau2.NumberOfConstraints(); i++)
                {
                    tableauC[i + 1, entryVariablePos] = Bi[i];
                }

                var RHS = BInverse * Vector<double>.Build.DenseOfArray(SensitivityAnalysis.GetAi(tableau, tableau2.NumberOfConstraints() + tableau2.NumberOfVariables));

                for (int i = 0; i < tableau2.NumberOfConstraints(); i++)
                {
                    tableauC[i + 1, tableau2.NumberOfVariables + tableau2.NumberOfConstraints()] = RHS[i];
                }

                TableauModel tableauModel = new TableauModel(tableauC, tableau2.NumberOfVariables, tableau2.NumberOfConstraints());
                tableauModel.ToConsole($"Iteration {itteration++}", false);

               
                List<double> ratios = new List<double>();

                for (int i = 0; i < tableau2.NumberOfConstraints(); i++)
                {
                    if (Bi[i] > 0)
                    {
                        ratios.Add(RHS[i] / Bi[i]);
                    }
                    else
                    {
                        ratios.Add(double.MaxValue);
                    }
                }

                int exitVariablePos = ratios.IndexOf(ratios.Min());
                exitVariablesPositions.Add(exitVariablePos);

                foreach (int pos in exitVariablesPositions)
                {
                    BInverse[pos, pos] = 1 / Bi[pos] * BInverse[pos, pos];

                    for (int i = 0; i < tableau2.NumberOfConstraints(); i++)
                    {
                        if (i != pos)
                        {
                            BInverse[i, pos] = BInverse[i, pos] - Bi[i] / Bi[pos];
                        }
                    } 
                }

                int nonBasicVariablePos2 = tableau.nonBasicVariablePos(true)[entryVariablePos];
                CBV[exitVariablePos] = tableau.CanonicalForm(true)[0, nonBasicVariablePos2];


                for (int i = 0; i < tableau2.NumberOfConstraints() + 1; i++)
                {
                    if (i != exitVariablePos)
                    {
                        tableauC[i, entryVariablePos] = 0;
                    }
                    else
                    {
                        tableauC[i, entryVariablePos] = 1;
                    }
                }

                for (int i = 0; i < tableau2.NumberOfConstraints() + 1; i++)
                {
                    if (i != entryVariablePos)
                    {
                        tableauC[exitVariablePos, i] = 0;
                    }
                    else
                    {
                        tableauC[exitVariablePos, i] = 1;
                    }
                }

                for (int i = 1; i < tableau2.NumberOfConstraints() + 1; i++)
                {
                    for (int j = tableau2.NumberOfVariables; j < tableau2.NumberOfConstraints()+ tableau2.NumberOfVariables; j++)
                    {
                        tableauC[i, j] = BInverse[i-1, j - tableau2.NumberOfVariables];

                    }
                }

                tableau2 = new TableauModel(tableauC, tableau2.NumberOfVariables, tableau2.NumberOfConstraints());
            }

            return tableau2;

        }

        // Helper function to compute dot product
        private static double DotProduct(double[] vector1, double[] vector2)
        {
            double result = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                result += vector1[i] * vector2[i];
            }
            return result;
        }

        // Helper function to get a column from the tableau
        private static double[] GetColumn(double[,] tableau, int columnIndex)
        {
            int rows = tableau.GetLength(0);
            double[] column = new double[rows];
            for (int i = 0; i < rows; i++)
            {
                column[i] = tableau[i, columnIndex];
            }
            return column;
        }

        // Helper function to get a row from the tableau
        private static double[] GetRow(double[,] tableau, int rowIndex)
        {
            int columns = tableau.GetLength(1);
            double[] row = new double[columns];
            for (int j = 0; j < columns; j++)
            {
                row[j] = tableau[rowIndex, j];
            }
            return row;
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
                if (weightLimit > 0)
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

            if (!hasSubProblem && weightLimit >= 0)
            {
                Console.WriteLine("");
                double totalValue = 0;
                foreach (BranchAndBoundItemModel item in result)
                {
                    totalValue += item.IsSelected * item.Value;
                }
                Console.WriteLine($"Candidate solution:" + totalValue);
                Console.WriteLine("Remaining weight:" + weightLimit);
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
        public static double[,] GetXValues(int numVariables, double[,] solution)
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
        /// <returns>boolean array which indicates if each answer is already an integer or not</returns>
        public static bool[] AllIntegers(double[,] RHS)
        {
            bool[] result = new bool[RHS.GetLength(0)];
            for (int i = 0; i < RHS.GetLength(0); i++)
            {
                if (RHS[i, 1] != Math.Floor(RHS[i, 1]))
                {
                    result[i] = false;
                }
                else
                {
                    result[i] = true;
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
        /// This function will extract the decimal parts of the values
        /// </summary>
        /// <param name="coefficients"></param>
        /// <returns>array of decimal parts of values</returns>
        public static double[] ExtractDecimalPart(double[] coefficients)
        {
            double[] result = new double[coefficients.Length];

            //testing
            //Console.WriteLine("Hello");
            //for (int i = 0; i < coefficients.Length; i++)
            //{
            //    Console.WriteLine(coefficients[i]);
            //}
            //Console.WriteLine("Hello");

            for (int i = 0; i < coefficients.Length; i++)
            {
                double decimalPart = coefficients[i] - Math.Truncate(coefficients[i]);

                // Check if the value is a whole number or 0
                if (decimalPart == 0)
                {
                    result[i] = 0;
                }
                else
                {
                    // Handle positive values
                    if (coefficients[i] > 0)
                    {
                        result[i] = Math.Round(decimalPart * -1, 3);
                    }
                    // Handle negative values
                    else
                    {
                        result[i] = Math.Round((1 - Math.Abs(decimalPart)) * -1, 3);
                    }
                }
            }

            //testing
            //Console.WriteLine("Bye");
            //for (int i = 0; i < result.Length; i++)
            //{
            //    Console.WriteLine(result[i]);
            //}
            //Console.WriteLine("Bye");

            return result;
        }

        ///<summary>
        /// This function will create an array which contains the values of the new constraint
        /// </summary>
        /// <param name="rowValues"></param>
        /// <param name="tableauSize"></param>
        /// <param name="numCoefficients"></param>
        /// <returns>an array of values that represent the new constraint values to be added</returns>
        public static double[] CreateNewConstraint(double[] rowValues, int tableauSize, int numCoefficients)
        {
            double[] result = new double[rowValues.Length + 1];
            double[] decimalConstraint = ExtractDecimalPart(rowValues);

            for (int i = 0; i < decimalConstraint.Length; i++)
            {
                result[i] = decimalConstraint[i];
            }

            result[result.Length - 2] = 1.0;

            // add new slack variable
            result[result.Length - 1] = decimalConstraint[decimalConstraint.Length - 1];

            //testing
            //for (int i = 0; i < result.Length; i++)
            //{
            //    Console.WriteLine(result[i]);
            //}

            return result;
        }

        ///<summary>
        /// This function will perform the dual simplex algorithm
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="numOfVariables"></param>
        /// <param name="numOfConstraints"></param>
        /// /// <returns>a matrix containing the tableau after dual simplex</returns>
        public static double[,] DualSimplex(double[,] tableau, int numOfVariables, int numOfConstraints)
        {
            int rows = tableau.GetLength(0);
            int columns = tableau.GetLength(1);

            while (true)
            {
                // step 1 - identify the leaving variable (row) - minimum in the RHS (b) column
                int leavingRow = -1;
                double minRHS = double.MaxValue;

                for (int i = 0; i < numOfConstraints + 1; i++)
                {
                    double rhsValue = tableau[i, columns - 1];
                    if (rhsValue < 0)
                    {
                        if (rhsValue < minRHS)
                        {
                            leavingRow = i;
                            minRHS = tableau[i, columns - 1];
                        }
                    }
                }

                // if there's no negative element in the RHS, the current solution is optimal
                if (leavingRow == -1)
                {
                    Console.WriteLine();
                    break;
                }

                // step 2 - identify the entering variable (column) using the dual ratio test
                int enteringColumn = -1;
                double minRatio = double.MaxValue;

                for (int j = 0; j < tableau.GetLength(1) - 1; j++)
                {
                    if (tableau[leavingRow, j] < 0) // only look at negative coefficients in the leaving row
                    {
                        double ratio = Math.Abs(tableau[0, j] / tableau[leavingRow, j]);
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            enteringColumn = j;
                        }
                    }
                }

                // if no valid entering column is found, the problem is unbounded
                if (enteringColumn == -1)
                {
                    Console.WriteLine("The problem is unbounded.");
                    break;
                }

                // step 3 - pivot
                Pivot(tableau, leavingRow, enteringColumn);

                for (int i = 0; i < tableau.GetLength(0); i++)
                {
                    for (int j = 0; j < tableau.GetLength(1); j++)
                    {
                        tableau[i, j] = Math.Round(tableau[i, j], 3);
                    }
                }
            }

            return tableau;
        }

        ///<summary>
        /// This function will perform the pivot operation on the tableau
        /// </summary>
        /// <param name="tableau"></param>
        /// <param name="pivotRow"></param>
        /// <param name="pivotColumn"></param>
        private static void Pivot(double[,] tableau, int pivotRow, int pivotColumn)
        {
            int rows = tableau.GetLength(0);
            int columns = tableau.GetLength(1);

            double pivotValue = tableau[pivotRow, pivotColumn];

            // dividing the pivot row by the pivot element
            for (int j = 0; j < columns; j++)
            {
                tableau[pivotRow, j] /= pivotValue;
            }

            // subtract multiples of the pivot row from the other rows
            for (int i = 0; i < rows; i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotColumn];
                    for (int j = 0; j < columns; j++)
                    {
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }
        }

        ///<summary>
        /// This function will create a new table with the new added constraint
        /// </summary>
        /// <param name="previousTable"></param>
        /// <param name="constraintRow"></param>
        /// <param name="columns"></param>
        /// <param name="numCoefficients"></param>
        /// <returns>a matrix containing the new table with the new constraint</returns>
        public static double[,] CreateNewTable(double[,] previousTable, int constraintRow, int columns, int numCoefficients)
        {
            //testing
            //Console.WriteLine("We will select constraint " + (double)constraintRow);

            // adding values of selected constraint row to array
            double[] constraintValues = new double[previousTable.GetLength(1)];
            for (int j = 0; j < previousTable.GetLength(1); j++)
            {
                constraintValues[j] = previousTable[constraintRow, j];
            }

            //testing
            //double[] coefficents = { 0.0, 1.4, -1.25, 0, 1, 1.4 };
            //double[] newConstraint = CreateNewConstraint(coefficents, columns, numCoefficients);

            double[] newConstraint = CreateNewConstraint(constraintValues, columns, numCoefficients);

            //testing
            //for (int i = 0; i < newConstraint.Length; i++)
            //{
            //    Console.WriteLine(newConstraint[i]);
            //}

            double[,] newTable = new double[previousTable.GetLength(0) + 1, previousTable.GetLength(1) + 1];
            int lastRow = newTable.GetLength(0) - 1;
            int secondLastColumn = previousTable.GetLength(1) - 1;
            int lastColumn = previousTable.GetLength(1);
            double[] rhsValues = new double[previousTable.GetLength(0)];

            for (int i = 0; i < previousTable.GetLength(0); i++)
            {
                rhsValues[i] = previousTable[i, secondLastColumn];
            }

            // adding original optimal values
            for (int i = 0; i < previousTable.GetLength(0); i++)
            {
                for (int j = 0; j < previousTable.GetLength(1); j++)
                {
                    newTable[i, j] = previousTable[i, j];
                    newTable[i, secondLastColumn] = 0;
                }
            }

            // adding rhs values
            for (int i = 0; i < previousTable.GetLength(0); i++)
            {
                newTable[i, lastColumn] = rhsValues[i];
            }

            //adding new constraint
            for (int j = 0; j < newTable.GetLength(1); j++)
            {
                newTable[lastRow, j] = newConstraint[j];
            }

            //testing
            //for (int i = 0; i < previousTable.GetLength(0); i++)
            //{
            //    for (int i = 0; i < previousTable.GetLength(1); i++)
            //    {
            //        Console.Write(previousTable[i, i] + "\t");
            //    }
            //        Console.WriteLine();
            //}

            int numVariables = numCoefficients;
            int numConstraints = newTable.GetLength(0) - 1;

            TableauModel model1 = new TableauModel(newTable, numVariables, numConstraints);
            // displaying table with added constraint
            model1.ToConsole("Adding New Constraint", false);

            double[,] matrix1 = model1.CanonicalForm(false);

            // performing dual simplex on table with new constraint
            double[,] table = DualSimplex(matrix1, numVariables, numConstraints);

            TableauModel model2 = new TableauModel(table, numVariables, numConstraints);
            // displaying table after dual simplex
            model2.ToConsole("Tableau After Pivoting", false);

            double[,] afterPivot = DualSimplex(matrix1, numVariables, numConstraints);

            return afterPivot;
        }

        ///<summary>
        /// This function will perform the cutting plane simplex algorithm
        /// </summary>
        /// <param name="constraintMatrix"></param>
        /// <param name="RHS"></param>
        /// <param name="stCoefficients"></param>
        public static void CuttingPlane(double[,] constraintMatrix, double[] RHS, double[] stCoefficients)
        {
            Console.WriteLine("You've selected to solve with the Cutting Plane Simplex Algorithm...");
            Console.WriteLine("First, we must find the optimal values with the Primal Simplex Algorithm:");

            TableauModel model = new TableauModel(constraintMatrix, RHS, stCoefficients);
            model.ToConsole("Initial Tableau", true);
            model = Algorithms.PrimalSimplex(model);
            model.ToConsole("Primal Simplex Optimal Solution", false);

            double[,] primalOptimal = model.CanonicalForm(false);

            // linking variables with values primal simplex optimal values
            double[,] variableAnswers = GetXValues(stCoefficients.Length, primalOptimal);

            // checking if primal simplex solution already has x-values that are all integers
            bool proceed = true;

            while (proceed)
            {
                // checking if all rhs values are already integers
                bool[] allIntegers = AllIntegers(variableAnswers);

                proceed = false;
                for (int i = 0; i < allIntegers.Length; i++)
                {
                    if (allIntegers[i] == false)
                    {
                        proceed = true;
                        break;
                    }
                }

                if (!proceed)
                {
                    break;
                }

                // slecting the pivot row
                int selectedConstraintRow = (int)SelectConstraint(variableAnswers);
                Console.WriteLine();

                // creating a new table with the new constraint
                double[,] newTable = CreateNewTable(primalOptimal, selectedConstraintRow, (stCoefficients.Length + constraintMatrix.GetLength(1) + stCoefficients.Length - 1), stCoefficients.Length);

                // linking variables with optimal values
                variableAnswers = GetXValues(stCoefficients.Length, newTable);
            }

            if (!proceed)
            {
                Console.WriteLine("All x-variable optimal values are now integers. Cutting Plane Simplex Algorithm complete.");
            }
        }



    }
}
