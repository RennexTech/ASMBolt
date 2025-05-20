using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMBolt.Core
{
    /// <summary>
    /// Handles file operations for the ASMBolt application.
    /// </summary>
    internal class FileHandler
    {
        /// <summary>
        /// Reads all text from the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The text content of the file, or null if an error occurs.</returns>
        public static string ReadFile(string filePath)
        {
            try
            {
                // Basic file read operation.  Consider using 'using' for better resource management.
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                // Log the error.  Consider using a logging framework.
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null; // Important: Handle the error, don't just crash.
            }
        }

        /// <summary>
        /// Writes text to the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="text">The text to write.</param>
        /// <returns>True if the write was successful, false otherwise.</returns>
        public static bool WriteFile(string filePath, string text)
        {
            try
            {
                // Basic file write operation.  Consider using 'using' and File.WriteAllText.
                File.WriteAllText(filePath, text);
                return true;
            }
            catch (Exception ex)
            {
                // Log the error.  Consider using a logging framework.
                Console.WriteLine($"Error writing to file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified file exists.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public static bool FileExists(string filePath)
        {
            try
            {
                // Simpler and more efficient way to check file existence.
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                // Log the error.  Consider using a logging framework.
                Console.WriteLine($"Error checking file existence: {ex.Message}");
                return false; // Handle the error, return false.
            }
        }

        /// <summary>
        /// Creates a new file at the specified path.
        /// </summary>
        /// <param name="filePath">The path to the file to create.</param>
        /// <returns>True if the file was created successfully, false otherwise.</returns>
        public static bool CreateFile(string filePath)
        {
            try
            {
                // Use File.Create to create the file, and ensure it is closed.
                using (File.Create(filePath)) { }
                return true;
            }
            catch (Exception ex)
            {
                // Log the error.  Consider using a logging framework.
                Console.WriteLine($"Error creating file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="filePath">The path to the file to delete.</param>
        /// <returns>True if the file was deleted successfully, false otherwise.</returns>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                // Use File.Delete to delete the file.
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                // Log the error.  Consider using a logging framework.
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the file extension of the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The file extension (including the leading dot), or string.Empty if there is no extension, or null if an error occurs.</returns>
        public static string GetFileExtension(string filePath)
        {
            try
            {
                // Use Path.GetExtension to get the file extension.
                return Path.GetExtension(filePath);
            }
            catch (Exception ex)
            {
                // Log the error. Consider using a logging framework.
                Console.WriteLine($"Error getting file extension: {ex.Message}");
                return null;
            }
        }
    }
}

