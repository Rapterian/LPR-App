//Atisang


//Primal Simplex
using LPR_App;

double[,] A = {
                { 1, 1 },
                { 2, 1 },
                { 1, 3 }
            };

double[] b = { 4, 5, 6 };
double[] c = { 3, 2 };

double[] solution = Algorithms.PrimalSimplex(A, b, c);

Console.WriteLine("Optimal solution:");
for (int i = 0; i < solution.Length - 1; i++)
{
    Console.WriteLine($"x{i + 1} = {solution[i]}");
}
Console.WriteLine($"Z = {solution[solution.Length - 1]}");

//


//Revised Primal Simplex 
string[] objFunc = "max +2 +3 +3 +5 +2 +4".Split(' ');
string[] constraints = "+11 +8 +6 +14 +10 +10 <=40".Split(' ');
string[] restrictions = "bin bin bin bin bin bin".Split(' ');
double[] basicVariables = { };

Algorithms.RevisedPrimalSimplex(objFunc, constraints, restrictions, basicVariables);
//



Console.ReadLine();