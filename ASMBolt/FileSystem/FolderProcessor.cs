using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMBolt.Core
{
    /// <summary>
    /// Handles folder (directory) operations for the ASMBolt application.
    /// </summary>
    internal class FolderProcessor
    {
        /// <summary>
        /// Creates the specified folder if it does not exist.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>True if the folder was created or already exists, false if an error occurs.</returns>
        public static bool CreateFolderIfNotExist(string folderPath)
        {
            try
            {
                // Check if the directory exists.
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the error. Consider using a logging framework.
                Console.WriteLine($"Error creating folder: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified folder exists.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>True if the folder exists, false otherwise.</returns>
        public static bool FolderExists(string folderPath)
        {
            try
            {
                // Use Directory.Exists to check.
                return Directory.Exists(folderPath);
            }
            catch (Exception ex)
            {
                // Log the error.  Consider using a logging framework.
                Console.WriteLine($"Error checking folder existence: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <param name="recursive">True to delete the folder and its contents recursively, false to delete only an empty folder.</param>
        /// <returns>True if the folder was deleted successfully, false otherwise.</returns>
        public static bool DeleteFolder(string folderPath, bool recursive)
        {
            try
            {
                // Use Directory.Delete
                Directory.Delete(folderPath, recursive);
                return true;
            }
            catch (Exception ex)
            {
                // Log the error. Consider using a logging framework.
                Console.WriteLine($"Error deleting folder: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the names of all files in the specified folder.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>A string array containing the names of the files in the folder, or null if an error occurs.</returns>
        public static string[] GetFilesInFolder(string folderPath)
        {
            try
            {
                // Use Directory.GetFiles
                return Directory.GetFiles(folderPath);
            }
            catch (Exception ex)
            {
                // Log the error. Consider using a logging framework.
                Console.WriteLine($"Error getting files in folder: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the names of all subfolders in the specified folder.
        /// </summary>
        /// <param name="folderPath">The path to the folder.</param>
        /// <returns>A string array containing the names of the subfolders in the folder, or null if an error occurs.</returns>
        public static string[] GetSubFoldersInFolder(string folderPath)
        {
            try
            {
                // Use Directory.GetDirectories
                return Directory.GetDirectories(folderPath);
            }
            catch (Exception ex)
            {
                // Log the error.
                Console.WriteLine($"Error getting subfolders in folder: {ex.Message}");
                return null;
            }
        }
    }
}
