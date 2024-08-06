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

double[,] solution = Algorithms.PrimalSimplex(A, b, c);

Console.WriteLine("Optimal solution:");
for (int i = 0; i < c.Length; i++)
{
    Console.Write($"x{i + 1} \t");
}
int s = 1;
for (int i = c.Length; i < c.Length + b.Length; i++)
{
    Console.Write($"s{s} \t");
    s++;
}
Console.WriteLine("RHS");
for (int i = 0; i < b.Length + 1; i++)
{
    for (int j = 0; j < c.Length + b.Length + 1; j++)
    {
        Console.Write(solution[i, j] + " \t");
    }
    Console.WriteLine("");
}

//



Console.ReadLine();