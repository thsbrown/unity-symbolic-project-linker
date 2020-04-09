using System;
using System.IO;
using System.Runtime.InteropServices;
using static System.Console;

namespace UnitySymbolicProjectLinker
{
    public class ConsoleHelper
    {
        /// <summary>
        /// The directory where our Unity project we will be creating symbolically linked project FROM lives
        /// </summary>
        private string originalProjectPath;
        /// <summary>
        /// The parent folder of our <see cref="originalProjectPath"/>. Our SymLink version of our Unity project will go here
        /// </summary>
        private string originalProjectPathParentFolder;
        /// <summary>
        /// The name of the folder for our <see cref="originalProjectPath"/>
        /// </summary>
        private string originalProjectName;
        /// <summary>
        /// The path to created folder for our Symbolically linked Unity project
        /// </summary>
        private string symbolicLinkProjectPath;
        
        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }
        
        /// <summary>
        /// Displays the initial prompt to the user
        /// </summary>
        public void DisplayInitialPrompt()
        {
            var initialPrompt = 
                @"
------- Unity Symbolic Link Project Creator ----------

This program will automatically create a symbolically linked
Unity project from a preexisting Unity project at a specified directory.

In essence the program will create symbolic links for
the Assets, ProjectSettings and Packages folders.

-[1]- Always ensure the program is run with ADMINISTRATIVE 
privileges or IT WONT WORK. 

------------------------------------------------------";
            WriteLine(initialPrompt);
        }

        /// <summary>
        /// Gets Unity project path from user, and sets all other project paths needed for use later
        /// </summary>
        public void GetProjectPathsFromUser()
        {
            WriteLine(
                "\nPlease enter the path to the Unity project we will be creating the Symbolic Links from (ex: C:/Users/Name/Games/UnityProject):");
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
            originalProjectPathParentFolder = Directory.GetParent(originalProjectPath).ToString();
            originalProjectName = new DirectoryInfo(originalProjectPath).Name;
            WriteLine("\nThe Symbolic Link Unity project will be created at the following destination.");
            symbolicLinkProjectPath = $"{originalProjectPathParentFolder}\\{originalProjectName} (Symbolic Link)";
            WriteLine($"Symbolic Link Project Path: \"{symbolicLinkProjectPath}\"");
        }

        /// <summary>
        /// Creates the symbolically linked folder and and symbolically links the Assets, ProjectSettings & Packages folders
        /// </summary>
        public void CreateSymbolicallyLinkedProject()
        {
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
            WriteLine("\nExecuting link Assets command:\n" + linkAssetsCommand);
            CreateSymbolicLink($"{symbolicLinkProjectPath}\\Assets", $"{originalProjectPath}\\Assets", SymbolicLink.Directory);
            WriteLine("\nExecuting link ProjectSettings command:\n" + linkProjectSettingsCommand);
            CreateSymbolicLink($"{symbolicLinkProjectPath}\\ProjectSettings", $"{originalProjectPath}\\ProjectSettings", SymbolicLink.Directory);
            WriteLine("\nExecuting link Packages command:\n" + linkPackagesCommand);
            CreateSymbolicLink($"{symbolicLinkProjectPath}\\Packages", $"{originalProjectPath}\\Packages", SymbolicLink.Directory);
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