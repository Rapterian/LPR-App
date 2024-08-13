using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR_App
{
    internal class FileHandling
    {
        // Method to read data from a file
        public string ReadFile(string filePath)
        {
            try
            {
                // Reads all text from the specified file
                string content = File.ReadAllText(filePath);
                return content; // Returns the file content as a string
            }
            catch (Exception ex)
            {
                // In case of an error, it will print the error message
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null; // Returns null if reading fails
            }
        }

        // Method to write data to a file
        public void WriteFile(string filePath, string content)
        {
            try
            {
                // Writes the specified content to the specified file
                File.WriteAllText(filePath, content);
                Console.WriteLine("File written successfully.");
            }
            catch (Exception ex)
            {
                // In case of an error, it will print the error message
                Console.WriteLine($"Error writing file: {ex.Message}");
            }
        }

        // Method to update a file by appending new content
        public void UpdateFile(string filePath, string newContent)
        {
            try
            {
                // Appends the specified content to the specified file
                File.AppendAllText(filePath, newContent);
                Console.WriteLine("File updated successfully.");
            }
            catch (Exception ex)
            {
                // In case of an error, it will print the error message
                Console.WriteLine($"Error updating file: {ex.Message}");
            }
        }
    }
}
