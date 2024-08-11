//Atisang


//Primal Simplex
using LPR_App;

double[,] A = {
                { 1, 1 },
                { 2, 1 },
                { 1, 3 }
            };

double[] b = { 0, 4, 5, 6 };
double[] c = { 3, 2 };

double[,] solution = Algorithms.PrimalSimplex(A, b, c);

Algorithms.displayTableau(solution, c.Length, b.Length-1, "Optimal Solution:");

TableauModel model = new TableauModel(A,b,c);

model.ToConsole();

model=Algorithms.PrimalSimplex(model);

model.ToConsole("Optimal Solution:");


Algorithms.CuttingPlane(A, b, c, c.Length, b.Length, "Primal Simplex Optimal Solution:");



Console.ReadLine();