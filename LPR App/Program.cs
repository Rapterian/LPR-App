using LPR_App;

double[,] A = {
                { 1, 1 },
                { 2, 1 },
                { 1, 3 }
              };

double[] b = { 0, 4, 5, 6 };
double[] c = { 3, 2 };

//double[,] solution = Algorithms.PrimalSimplex(A, b, c);
//Algorithms.displayTableau(solution, c.Length, b.Length-1, "Optimal Solution:");

//Primal Simplex
TableauModel model = new TableauModel(A,b,c);
model = Algorithms.PrimalSimplex(model);
model.ToConsole("Optimal Solution:", false);

//Cutting Plane Simplex
Algorithms.CuttingPlane(A, b, c);


double[] weight = { 12, 2, 1, 1, 4 };
double[] value = { 4, 2, 2, 1, 10 };
double weightLimit = 15;

Algorithms.BranchBoundKnapsack(value,weight , weightLimit);

Console.ReadLine();