using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
     public class FileHandler
    {
        public static double[,] ReadMatrixFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            int rows = lines.Length;
            int columns = lines[0].Split(',').Length;
            double[,] matrix = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                var values = lines[i].Split(',');
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] = double.Parse(values[j]);
                }
            }

            return matrix;
        }

        public static void WriteMatrixToCsv(string filePath, double[,] matrix)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int rows = matrix.GetLength(0);
                int columns = matrix.GetLength(1);

                for (int i = 0; i < rows; i++)
                {
                    string[] values = new string[columns];
                    for (int j = 0; j < columns; j++)
                    {
                        values[j] = matrix[i, j].ToString();
                    }
                    writer.WriteLine(string.Join(",", values));
                }
            }
        }

        public static double[] ReadArrayFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            double[] array = new double[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                array[i] = double.Parse(lines[i]);
            }

            return array;
        }

        public static void WriteArrayToCsv(string filePath, double[] array)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var value in array)
                {
                    writer.WriteLine(value);
                }
            }
        }
    }
}
