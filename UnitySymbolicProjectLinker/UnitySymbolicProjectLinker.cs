using System;
using System.IO;
using static System.Console;

namespace UnitySymbolicProjectLinker
{
    internal class UnitySymbolicProjectLinker
    {
        public static void Main(string[] args)
        {
            //initial prompt
            var initialPrompt = 
@"
------- Unity Symbolic Link Project Creator ----------

This batch file will automatically create a Symbolic Link
of a given Unity project at a specified directory.

In essence the program will create Symbolic links for
the Assets, ProjectSettings and Packages folders.

-[1]- Always ensure the program is run with ADMINISTRATIVE 
privileges or IT WONT WORK. 

------------------------------------------------------";
            WriteLine(initialPrompt);
            
            WriteLine(
                "\nPlease enter the path to the Unity project we will be creating the Symbolic Links from (ex: C:/Users/Name/Games/UnityProject):");
            string originalProjectPath;
            bool isValidDirectory; 
            do
            {
                originalProjectPath = ReadLine();
                isValidDirectory = Directory.Exists(originalProjectPath);
                if (!isValidDirectory)
                {
                    WriteLine("Invalid Directory...");
                }
            } while (!isValidDirectory);
            var originalProjectPathParentFolder = Directory.GetParent(originalProjectPath);
            var originalProjectName = new DirectoryInfo(originalProjectPath).Name;
            WriteLine("\nThe Symbolic Link Unity project will be created at the following destination.");
            var symbolicLinkProjectPath = $"{originalProjectPathParentFolder}\\{originalProjectName} (Symbolic Link)";
            WriteLine($"Symbolic Link Project Path: \"{symbolicLinkProjectPath}\"");
            try
            {
                Directory.CreateDirectory(symbolicLinkProjectPath);
            }
            catch (Exception e)
            {
                WriteLine("The Symbolic Link directory failed to be created...");
            }
            var linkAssetsCommand = $"mklink /D \"{symbolicLinkProjectPath}\\Assets\" \"{originalProjectPath}\\Assets\"";
            var linkProjectSettingsCommand = $"mklink /D \"{symbolicLinkProjectPath}\\ProjectSettings\" \"{originalProjectPath}\\ProjectSettings\"";
            var linkPackagesCommand = $"mklink /D \"{symbolicLinkProjectPath}\\Packages\" \"{originalProjectPath}\\Packages\"";
            WriteLine("\nExecuting link Assets command: " + linkAssetsCommand);
            System.Diagnostics.Process.Start("CMD.exe", $"/C {linkAssetsCommand}");
            WriteLine("Executing link ProjectSettings command: " + linkAssetsCommand);
            System.Diagnostics.Process.Start("CMD.exe", $"/C {linkProjectSettingsCommand}");
            WriteLine("Executing link Packages command: " + linkAssetsCommand);
            System.Diagnostics.Process.Start("CMD.exe", $"/C {linkPackagesCommand}");
        }

        /// <summary>
        /// Waits for a y or n console input from the user.
        /// </summary>
        /// <returns>True if y is entered, false if n is entered</returns>
        private static bool WaitForYesOrNo()
        {
            ConsoleKeyInfo enteredKey;
            bool isValidInput; 
            do
            {
                enteredKey = ReadKey();
                isValidInput = enteredKey.Key == ConsoleKey.Y || enteredKey.Key == ConsoleKey.N;
                if (!isValidInput)
                {
                    WriteLine("\nInvalid input, enter y or n");
                }
            } while (!isValidInput);
            return enteredKey.Key == ConsoleKey.Y;
        }
        
    }
}