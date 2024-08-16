using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LPR_App
{
    public static class SensitivityAnalysis
    {
        public enum DualityType
        {
            StrongDuality,
            WeakDuality,
            NoDuality // This should theoretically never happen in a well-posed LP problem
        }
        public static double[] GetCBV(TableauModel initialTableau, TableauModel optimalTableau)
        {
            var basicVariablePositions = optimalTableau.BasicVariablePos();
            double[] cbv = new double[basicVariablePositions.Count];

            for (int i = 0; i < basicVariablePositions.Count; i++)
            {
                cbv[i] = initialTableau.CanonicalForm(true)[0, basicVariablePositions[i]];
            }

            return cbv;
        }
        public static double[] ChangeCBV(TableauModel initialTableau, TableauModel optimalTableau, int valuePos, double newValue)
        {
            var basicVariablePositions = optimalTableau.BasicVariablePos();
            double[] cbv = new double[basicVariablePositions.Count];

            for (int i = 0; i < basicVariablePositions.Count; i++)
            {

                if (i != valuePos)
                {
                    cbv[i] = initialTableau.CanonicalForm(true)[0, basicVariablePositions[i]];
                }
                else
                {
                    cbv[i] = newValue;
                }
            }

            return cbv;
        }

        public static double[,] GetB(TableauModel initialTableau, TableauModel optimalTableau)
        {
            int numberOfConstraints = optimalTableau.NumberOfConstraints();
            var basicVariablePositions = optimalTableau.BasicVariablePos();

            double[,] B = new double[numberOfConstraints, basicVariablePositions.Count];

            for (int j = 0; j < basicVariablePositions.Count; j++)
            {
                for (int i = 0; i < numberOfConstraints; i++)
                {
                    B[i, j] = initialTableau.CanonicalForm(true)[i + 1, basicVariablePositions[j]];
                }
            }

            return B;
        }
        public static double[] Getb(TableauModel initialTableau)
        {
            int numberOfConstraints = initialTableau.NumberOfConstraints();

            double[] b = new double[numberOfConstraints];

            for (int j = 1; j < numberOfConstraints + 1; j++)
            {
                b[j - 1] = initialTableau.CanonicalForm(true)[j, initialTableau.NumberOfVariables + initialTableau.NumberOfConstraints()];
            }

            return b;
        }

        public static double[,] GetBInverse(TableauModel initialTableau, TableauModel optimalTableau)
        {
            double[,] B = GetB(initialTableau, optimalTableau);
            var matrix = Matrix<double>.Build.DenseOfArray(B);

            // Invert the matrix
            var inverseMatrix = matrix.Inverse();


            return inverseMatrix.ToArray();
        }

        public static double[] GetDotProduct(TableauModel initialTableau, TableauModel optimalTableau)
        {
            double[] cbv = GetCBV(initialTableau, optimalTableau);
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);

            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var cbvVector = Vector<double>.Build.DenseOfArray(cbv);
            var dotProductVector = cbvVector * BInverseMatrix;

            return dotProductVector.AsArray();
        }
        public static double[] GetSi(TableauModel initialTableau, TableauModel optimalTableau)
        {
            double[] CBV = GetCBV(initialTableau, optimalTableau);
            var CBVVector = Vector<double>.Build.DenseOfArray(CBV);
            var BInverse = Matrix<double>.Build.DenseOfArray(GetBInverse(initialTableau, optimalTableau));
            double[] Si = new double[optimalTableau.NumberOfConstraints()];
            var SIVector = Vector<double>.Build.DenseOfArray(Si);
            SIVector = BInverse * CBVVector;
            Si = SIVector.AsArray();
            return Si;
        }
        public static double[] GetSi(TableauModel initialTableau, TableauModel optimalTableau, double[] CBV)
        {
            var CBVVector = Vector<double>.Build.DenseOfArray(CBV);
            var BInverse = Matrix<double>.Build.DenseOfArray(GetBInverse(initialTableau, optimalTableau));
            double[] Si = new double[optimalTableau.NumberOfConstraints()];
            var SIVector = Vector<double>.Build.DenseOfArray(Si);
            SIVector = BInverse * CBVVector;
            Si = SIVector.AsArray();
            return Si;
        }

        public static double CalculateNewObjectiveCoefficient(TableauModel initialTableau, TableauModel optimalTableau, int variableIndex, double newCoefficient)
        {

            // Step 1: Get the current CBV, B inverse, and the A_i column
            double[] cbv = GetCBV(initialTableau, optimalTableau);
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);
            int numberOfConstraints = optimalTableau.NumberOfConstraints();



            // Step 2: Extract the A_i column from the initial tableau
            double[] Ai = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                Ai[i] = initialTableau.CanonicalForm(true)[i + 1, variableIndex];
            }

            // Step 3: Convert BInverse and Ai into MathNet vectors/matrices
            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var AiVector = Vector<double>.Build.DenseOfArray(Ai);

            // Step 4: Calculate (CBV * B-1 * Ai)
            var cbvVector = Vector<double>.Build.DenseOfArray(cbv);
            var result = cbvVector * BInverseMatrix * AiVector;

            // Step 5: Subtract the original objective coefficient
            double Ci = newCoefficient;
            double newObjectiveCoefficient = result - Ci;

            return newObjectiveCoefficient;
        }
        public static double CalculateNewObjectiveCoefficient(TableauModel initialTableau, TableauModel optimalTableau, int variableIndex, double newCoefficient, double[] cbv)
        {

            // Step 1: Get the current CBV, B inverse, and the A_i column
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);
            int numberOfConstraints = optimalTableau.NumberOfConstraints();


            // Step 2: Extract the A_i column from the initial tableau
            double[] Ai = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                Ai[i] = initialTableau.CanonicalForm(true)[i + 1, variableIndex];
            }

            // Step 3: Convert BInverse and Ai into MathNet vectors/matrices
            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var AiVector = Vector<double>.Build.DenseOfArray(Ai);

            // Step 4: Calculate (CBV * B-1 * Ai)
            var cbvVector = Vector<double>.Build.DenseOfArray(cbv);
            var result = cbvVector * BInverseMatrix * AiVector;

            // Step 5: Subtract the original objective coefficient
            double Ci = newCoefficient;
            double newObjectiveCoefficient = result - Ci;

            return newObjectiveCoefficient;
        }


        public static double[] CalculateConstraintCoefficientColumn(TableauModel initialTableau, TableauModel optimalTableau, int constraintIndex, int variableIndex, double newCoefficient)
        {

            // Step 1: Get the current CBV, B inverse, and the A_i column
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);
            int numberOfConstraints = optimalTableau.NumberOfConstraints();


            // Step 2: Extract the A_i column from the initial tableau
            double[] Ai = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                if (i == constraintIndex)
                {
                    Ai[i] = newCoefficient;
                }
                else
                {
                    Ai[i] = initialTableau.CanonicalForm(true)[i + 1, variableIndex];
                }
            }

            // Step 3: Convert BInverse and Ai into MathNet vectors/matrices
            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var AiVector = Vector<double>.Build.DenseOfArray(Ai);

            // Step 4: Calculate (CBV * B-1 * Ai)
            var cbvVector = Vector<double>.Build.DenseOfArray(GetCBV(initialTableau, optimalTableau));
            var result = cbvVector * BInverseMatrix * AiVector;

            // Step 5: Subtract the original objective coefficient
            double Ci = newCoefficient;
            double newObjectiveCoefficient = result - Ci;

            double[] newAi = new double[numberOfConstraints];
            newAi = (BInverseMatrix * AiVector).AsArray();

            double[] newConstraintColumn = new double[numberOfConstraints + 1];
            newConstraintColumn[0] = newCoefficient;
            for (int i = 1; i < numberOfConstraints + 1; i++)
            {
                newConstraintColumn[i] = newAi[i - 1];
            }

            return newConstraintColumn;
        }

        public static double[] CalculateVariableColumn(TableauModel initialTableau, TableauModel optimalTableau, double[] variableConstraintColumn, double variableCoefficient)
        {

            // Step 1: Get the current CBV, B inverse, and the A_i column
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);
            int numberOfConstraints = optimalTableau.NumberOfConstraints();


            // Step 2: Extract the A_i column 
            double[] Ai = variableConstraintColumn;


            // Step 3: Convert BInverse and Ai into MathNet vectors/matrices
            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var AiVector = Vector<double>.Build.DenseOfArray(Ai);

            // Step 4: Calculate (CBV * B-1 * Ai)
            var cbvVector = Vector<double>.Build.DenseOfArray(GetCBV(initialTableau, optimalTableau));
            var result = cbvVector * BInverseMatrix * AiVector;

            // Step 5: Subtract the original objective coefficient
            double Ci = variableCoefficient;
            double newObjectiveCoefficient = result - Ci;

            double[] newAi = new double[numberOfConstraints];
            newAi = (BInverseMatrix * AiVector).AsArray();

            double[] newVariableColumn = new double[numberOfConstraints + 1];
            newVariableColumn[0] = variableCoefficient;
            for (int i = 1; i < numberOfConstraints + 1; i++)
            {
                newVariableColumn[i] = newAi[i - 1];
            }

            return newVariableColumn;
        }

        public static double[,] changeObjectiveCoefficient(TableauModel initialTableau, TableauModel optimalTableau, int variableIndex, double newCoefficient, bool isBasic)
        {
            double[,] newTableau = optimalTableau.CanonicalForm(false).Clone() as double[,];

            if (isBasic)
            {
                double[] changedCBV = ChangeCBV(initialTableau, optimalTableau, variableIndex, newCoefficient);
                var chagedCBVVector = Vector<double>.Build.DenseOfArray(changedCBV);

                var Si = Vector<double>.Build.DenseOfArray(GetSi(initialTableau, optimalTableau, changedCBV));

                var b = Vector<double>.Build.DenseOfArray(Getb(initialTableau));

                double Z = Si.DotProduct(b);
                for (int j = 0; j < optimalTableau.NumberOfVariables; j++)
                {
                    if (j == variableIndex)
                    {
                        newTableau[0, j] = CalculateNewObjectiveCoefficient(initialTableau, optimalTableau, j, newCoefficient, changedCBV);
                    }
                    else
                    {
                        newTableau[0, j] = CalculateNewObjectiveCoefficient(initialTableau, optimalTableau, j, initialTableau.CanonicalForm(true)[0, -j], changedCBV);
                    }
                }
                for (int j = optimalTableau.NumberOfVariables; j < optimalTableau.NumberOfConstraints(); j++)
                {

                    newTableau[0, j] = Si[j - optimalTableau.NumberOfVariables];
                }
                newTableau[0, optimalTableau.NumberOfConstraints() + optimalTableau.NumberOfVariables] = Z;
            }
            else
            {
                newTableau[0, variableIndex] = CalculateNewObjectiveCoefficient(initialTableau, optimalTableau, variableIndex, newCoefficient);
            }




            return Algorithms.PrimalSimplex(new TableauModel(newTableau, optimalTableau.NumberOfVariables, optimalTableau.NumberOfMaxConstraints, optimalTableau.NumberOfMinConstraints)).CanonicalForm(false);
        }

        public static double[] rangeObjectiveCoefficient(TableauModel initialTableau, TableauModel optimalTableau, int variableIndex, bool isBasic)
        {
            double upperBound = double.PositiveInfinity;
            double lowerBound = double.NegativeInfinity;

            // Get the current CBV, B inverse, and the A_i column
            double[] cbv = GetCBV(initialTableau, optimalTableau);
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);
            int numberOfConstraints = optimalTableau.NumberOfConstraints();

            // Extract the A_i column from the initial tableau
            double[] Ai = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                Ai[i] = initialTableau.CanonicalForm(true)[i + 1, variableIndex];
            }

            // Convert BInverse and Ai into MathNet vectors/matrices
            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var AiVector = Vector<double>.Build.DenseOfArray(Ai);

            // Calculate CBV * B-1 * Ai
            var cbvVector = Vector<double>.Build.DenseOfArray(cbv);
            var result = cbvVector * BInverseMatrix * AiVector;

            double originalCoefficient = initialTableau.CanonicalForm(true)[0, variableIndex];
            double coefficientDifference = optimalTableau.CanonicalForm(true)[0, variableIndex] - originalCoefficient;

            if (isBasic)
            {
                // For basic variables, we need to adjust CBV and calculate new bounds

                // Calculate the new Si (reduced cost vector)
                var changedCBV = ChangeCBV(initialTableau, optimalTableau, variableIndex, optimalTableau.CanonicalForm(true)[0, variableIndex]);
                var Si = GetSi(initialTableau, optimalTableau, changedCBV);

                // Determine the upper and lower bounds by analyzing the impact of the change on all other variables
                for (int j = 0; j < Si.Length; j++)
                {
                    if (Si[j] < 0)
                    {
                        double ratio = -cbv[j] / Si[j];
                        upperBound = Math.Min(upperBound, ratio);
                    }
                    else if (Si[j] > 0)
                    {
                        double ratio = -cbv[j] / Si[j];
                        lowerBound = Math.Max(lowerBound, ratio);
                    }
                }
            }
            else
            {
                // For non-basic variables, simply determine the impact of the coefficient change
                double delta = coefficientDifference;

                if (result - delta < 0)
                {
                    upperBound = coefficientDifference + result;
                }
                else
                {
                    lowerBound = coefficientDifference - result;
                }
            }

            return new double[] { lowerBound, upperBound };
        }

        public static double[,] changeConstraintCoefficient(TableauModel initialTableau, TableauModel optimalTableau, int constraintIndex, int variableIndex, double newCoefficient)
        {
            double[,] newTableau = optimalTableau.CanonicalForm(false).Clone() as double[,];

            double[] newConstraintColumn = CalculateConstraintCoefficientColumn(initialTableau, optimalTableau, constraintIndex, variableIndex, newCoefficient);

            newTableau[0, variableIndex] = newConstraintColumn[0];
            for (int i = 1; i < optimalTableau.NumberOfConstraints(); i++)
            {
                newTableau[i, variableIndex] = newConstraintColumn[i];
            }

            return Algorithms.PrimalSimplex(new TableauModel(newTableau, optimalTableau.NumberOfVariables, optimalTableau.NumberOfMaxConstraints, optimalTableau.NumberOfMinConstraints)).CanonicalForm(false);
        }

        public static double[] rangeConstraintCoefficient(TableauModel initialTableau, TableauModel optimalTableau, int constraintIndex, int variableIndex)
        {
            double upperBound = double.PositiveInfinity;
            double lowerBound = double.NegativeInfinity;

            int numberOfConstraints = optimalTableau.NumberOfConstraints();
            int numberOfVariables = optimalTableau.NumberOfVariables;

            // Get the current basis inverse matrix (B-1) and the right-hand side (b) from the optimal tableau
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);
            double[] rhs = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                rhs[i] = optimalTableau.CanonicalForm(true)[i + 1, 0];  // The right-hand side values in the tableau
            }

            // Calculate the current value of the tableau entry
            double currentCoefficient = initialTableau.CanonicalForm(true)[constraintIndex + 1, variableIndex];
            double coefficientDifference = optimalTableau.CanonicalForm(true)[constraintIndex + 1, variableIndex] - currentCoefficient;

            // Modify the A matrix to reflect the new coefficient
            double[] modifiedAi = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                modifiedAi[i] = initialTableau.CanonicalForm(true)[i + 1, variableIndex];
            }
            modifiedAi[constraintIndex] = optimalTableau.CanonicalForm(true)[constraintIndex + 1, variableIndex];

            // Compute the impact on the right-hand side (b')
            var BInverseMatrix = Matrix<double>.Build.DenseOfArray(BInverse);
            var modifiedAiVector = Vector<double>.Build.DenseOfArray(modifiedAi);
            var bPrime = BInverseMatrix * modifiedAiVector;

            // Analyze the effect on the feasibility by checking each element of b'
            for (int i = 0; i < bPrime.Count; i++)
            {
                if (bPrime[i] < 0)
                {
                    double ratio = -rhs[i] / bPrime[i];
                    upperBound = Math.Min(upperBound, ratio);
                }
                else if (bPrime[i] > 0)
                {
                    double ratio = -rhs[i] / bPrime[i];
                    lowerBound = Math.Max(lowerBound, ratio);
                }
            }

            return new double[] { lowerBound, upperBound };
        }
        public static double[,] changeRHSvalues(TableauModel initialTableau, TableauModel optimalTableau, int rhsIndex, double newRHS)
        {
            double[,] newTableau = optimalTableau.CanonicalForm(false).Clone() as double[,];

            var Si = Vector<double>.Build.DenseOfArray(GetSi(initialTableau, optimalTableau));

            var b = Vector<double>.Build.DenseOfArray(Getb(initialTableau));

            b[rhsIndex] = newRHS;

            double Z = Si.DotProduct(b);

            newTableau[0, optimalTableau.NumberOfConstraints() + optimalTableau.NumberOfVariables] = Z;
            for (int i = 1; i < optimalTableau.NumberOfConstraints() + 1; i++)
            {
                newTableau[i, optimalTableau.NumberOfConstraints() + optimalTableau.NumberOfVariables] = b[i - 1];
            }
            return Algorithms.PrimalSimplex(new TableauModel(newTableau, optimalTableau.NumberOfVariables, optimalTableau.NumberOfMaxConstraints, optimalTableau.NumberOfMinConstraints)).CanonicalForm(false);
        }
        public static double[] changeRHSvalues(TableauModel initialTableau, TableauModel optimalTableau, double[] CBV)
        {
            double[,] newTableau = optimalTableau.CanonicalForm(false).Clone() as double[,];
            double[] newRHScolumn = new double[optimalTableau.NumberOfConstraints() + 1];

            var Si = Vector<double>.Build.DenseOfArray(GetSi(initialTableau, optimalTableau, CBV));

            var b = Vector<double>.Build.DenseOfArray(Getb(initialTableau));

            double Z = Si.DotProduct(b);
            newRHScolumn[0] = Z;
            for (int i = 1; i < optimalTableau.NumberOfConstraints() + 1; i++)
            {
                newRHScolumn[i] = b[i - 1];
            }
            return newRHScolumn;
        }

        public static double[] rangeRHS(TableauModel initialTableau, TableauModel optimalTableau, int constraintIndex)
        {
            double upperBound = double.PositiveInfinity;
            double lowerBound = double.NegativeInfinity;

            int numberOfConstraints = optimalTableau.NumberOfConstraints();

            // Get the current basis inverse matrix (B-1) from the optimal tableau
            double[,] BInverse = GetBInverse(initialTableau, optimalTableau);

            // Get the right-hand side vector (b) from the optimal tableau
            double[] rhs = new double[numberOfConstraints];
            for (int i = 0; i < numberOfConstraints; i++)
            {
                rhs[i] = optimalTableau.CanonicalForm(true)[i + 1, 0];  // The right-hand side values in the tableau
            }

            // Extract the row of B-1 that corresponds to the constraint we are interested in
            double[] bRow = new double[numberOfConstraints];
            for (int j = 0; j < numberOfConstraints; j++)
            {
                bRow[j] = BInverse[constraintIndex, j];
            }

            // Calculate the effect of changing the RHS on the current basic feasible solution
            for (int i = 0; i < bRow.Length; i++)
            {
                if (bRow[i] > 0)
                {
                    double ratio = rhs[i] / bRow[i];
                    upperBound = Math.Min(upperBound, ratio);
                }
                else if (bRow[i] < 0)
                {
                    double ratio = rhs[i] / bRow[i];
                    lowerBound = Math.Max(lowerBound, ratio);
                }
            }

            // Convert back the lower and upper bounds based on the original RHS
            double originalRHS = initialTableau.CanonicalForm(true)[0, constraintIndex + 1];
            lowerBound = originalRHS - lowerBound;
            upperBound = originalRHS - upperBound;

            return new double[] { lowerBound, upperBound };
        }

        public static double[,] addActivity(TableauModel initialTableau, TableauModel optimalTableau, double variableCoefficient, double[] variableConstraintColumn)
        {
            double[,] originalOptimal = optimalTableau.CanonicalForm(false).Clone() as double[,];
            double[,] newTableau = new double[originalOptimal.GetLength(0), originalOptimal.GetLength(1) + 1];
            int newVariableColumnIndex = optimalTableau.NumberOfVariables + 1;
            double[] newVariableConstraintColumn = CalculateVariableColumn(initialTableau, optimalTableau, variableConstraintColumn, variableCoefficient);

            for (int i = 0; i < optimalTableau.NumberOfVariables; i++)
            {
                for (int j = 0; j < originalOptimal.GetLength(1); j++)
                {
                    newTableau[i, j] = originalOptimal[i, j];
                }
            }

            for (int i = newVariableColumnIndex + 1; i < optimalTableau.NumberOfVariables + optimalTableau.NumberOfConstraints(); i++)
            {
                for (int j = 0; j < originalOptimal.GetLength(1); j++)
                {
                    newTableau[i, j] = originalOptimal[i, j];
                }
            }

            for (int i = 0; i < optimalTableau.NumberOfConstraints(); i++)
            {
                newTableau[i, newVariableColumnIndex] = newVariableConstraintColumn[i];
            }

            return Algorithms.PrimalSimplex(new TableauModel(newTableau, optimalTableau.NumberOfVariables, optimalTableau.NumberOfMaxConstraints, optimalTableau.NumberOfMinConstraints)).CanonicalForm(false);
        }
        public static double[,] addConstraint(TableauModel initialTableau, TableauModel optimalTableau, double[] constraintCoefficients, double constraintRHS)
        {
            double[,] originalOptimal = optimalTableau.CanonicalForm(false).Clone() as double[,];
            double[,] newTableau = new double[originalOptimal.GetLength(0) + 1, originalOptimal.GetLength(1) + 1];
            int newVariableColumnIndex = originalOptimal.GetLength(0);


            for (int i = 0; i < originalOptimal.GetLength(0); i++)
            {

                for (int j = 0; j < originalOptimal.GetLength(1) - 1; j++)
                {
                    newTableau[i, j] = originalOptimal[i, j];
                }
                newTableau[i, newTableau.GetLength(1) - 1] = originalOptimal[i, originalOptimal.GetLength(1) - 1];

            }
            newTableau[newTableau.GetLength(0) - 1, newTableau.GetLength(1) - 1] = constraintRHS;

            for (int i = 0; i < constraintCoefficients.Length; i++)
            {
                newTableau[newTableau.GetLength(0) - 1, i] = constraintCoefficients[i];
            }

            for (int j = 0; j < constraintCoefficients.Length; j++)
            {
                if (constraintCoefficients[j] != 0)
                {
                    int indexOfOne = -1;
                    if (basicVariableConflict(optimalTableau, j))
                    {
                        for (int i = 0; i < originalOptimal.GetLength(0); i++)
                        {
                            if (newTableau[i, j] == 1)
                            {
                                indexOfOne = i;
                            }
                        }
                        for (int columns = 0; columns < newTableau.GetLength(1); columns++)
                        {
                            newTableau[newTableau.GetLength(0) - 1, columns] = newTableau[indexOfOne, columns] * newTableau[newTableau.GetLength(0) - 1, columns] - newTableau[newTableau.GetLength(0) - 1, columns];
                        }

                    }
                }
            }

            if (newTableau[newTableau.GetLength(0) - 1, newTableau.GetLength(1) - 1] < 0)
            {
                for (int columns = 0; columns < newTableau.GetLength(1); columns++)
                {
                    newTableau[newTableau.GetLength(0) - 1, columns] = newTableau[newTableau.GetLength(0) - 1, columns] * -1;
                }
            }

            return Algorithms.DualSimplex(newTableau, optimalTableau.NumberOfVariables + 1, optimalTableau.NumberOfConstraints() + 1);
        }

        public static bool basicVariableConflict(TableauModel optimalTableau, int variableIndex)
        {
            var basicVariablePositions = optimalTableau.BasicVariablePos();
            return basicVariablePositions.Contains(variableIndex);
        }

        public static DualityType CheckDuality(TableauModel initialTableau, TableauModel optimalTableau)
        {

            // Calculate the optimal objective value for the primal problem
            double primalOptimalValue = optimalTableau.CanonicalForm(false)[0, optimalTableau.NumberOfVariables + optimalTableau.NumberOfConstraints()];

            // Calculate the optimal objective value for the dual problem
            double dualOptimalValue = initialTableau.GetDualTableau().CanonicalForm(false)[0, initialTableau.NumberOfVariables + initialTableau.NumberOfConstraints()];

            // Check for strong duality (optimal objective values are equal)
            if (Math.Abs(primalOptimalValue - dualOptimalValue) < 1e-6) // Tolerance for floating-point comparison
            {
                return DualityType.StrongDuality;
            }
            // If strong duality doesn't hold, weak duality always does, as long as the problems are feasible
            else if (primalOptimalValue < dualOptimalValue)
            {
                return DualityType.WeakDuality;
            }

            // If the primal objective is greater than the dual (which shouldn't happen if both problems are feasible)
            // return NoDuality. This is a fallback check for completeness.
            return DualityType.NoDuality;
        }
    }
}
