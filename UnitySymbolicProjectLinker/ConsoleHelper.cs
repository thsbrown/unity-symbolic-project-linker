using System;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// The startup mode the user selected.
        /// </summary>
        public int StartUpMode { get; private set; }

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
  __  __     _ __         ____           __   _      __     _____             __          
 / / / /__  (_) /___ __  / __/_ ____ _  / /  (_)__  / /__  / ___/______ ___ _/ /____  ____
/ /_/ / _ \/ / __/ // / _\ \/ // /  ' \/ /__/ / _ \/  '_/ / /__/ __/ -_) _ `/ __/ _ \/ __/
\____/_//_/_/\__/\_, / /___/\_, /_/_/_/____/_/_//_/_/\_\  \___/_/  \__/\_,_/\__/\___/_/   
                /___/      /___/                                                          

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

        public void SelectSymbolicLinkMode()
        {
            WriteLine("Please select the start mode:");    
            WriteLine("1. Create new Unity symbolic link project.");    
            WriteLine("2. Fix Unity symbolic links on preexisting project.\n");
            StartUpMode = int.Parse(WaitForAcceptableInput('1','2').ToString());
        }
        
        /// <summary>
        /// Gets Unity project path from user, and sets all other project paths needed for use later
        /// </summary>
        public void RunModeOne()
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
                    WriteLine("Invalid Directory, please try again...");
                }
            } while (!isValidDirectory);
            originalProjectPathParentFolder = Directory.GetParent(originalProjectPath).ToString();
            originalProjectName = new DirectoryInfo(originalProjectPath).Name;
            WriteLine("\nThe Symbolic Link Unity project will be created at the following destination.");
            symbolicLinkProjectPath = $"{originalProjectPathParentFolder}\\{originalProjectName} (Symbolic Link)";
            WriteLine($"Symbolic Link Project Path: \"{symbolicLinkProjectPath}\"");
        }

        /// <summary>
        /// Gets symbolic link and original project path from user
        /// </summary>
        public void RunModeTwo()
        {
            WriteLine(
                "\nPlease enter the path to the Unity Symbolic Link project (ex: C:/Users/Name/Games/UnityProject (Symbolic Link)):");
            bool isValidSymLinkDirectory; 
            do
            {
                symbolicLinkProjectPath = ReadLine();
                isValidSymLinkDirectory = Directory.Exists(symbolicLinkProjectPath);
                if (!isValidSymLinkDirectory)
                {
                    WriteLine("Invalid Directory, please try again...");
                }
            } while (!isValidSymLinkDirectory);
            WriteLine(
                "\nPlease enter the path to the Unity project we will be creating the Symbolic Links from (ex: C:/Users/Name/Games/UnityProject):");
            bool isValidOgPathDirectory; 
            do
            {
                originalProjectPath = ReadLine();
                isValidOgPathDirectory = Directory.Exists(originalProjectPath);
                if (!isValidOgPathDirectory)
                {
                    WriteLine("Invalid Directory, please try again...");
                }
            } while (!isValidOgPathDirectory);
            WriteLine("\nThe Symbolic Link Unity project will be reconfigured given the following paths.");
            WriteLine($"Symbolic Link Project Path: \"{symbolicLinkProjectPath}\"");
            WriteLine($"Original Project Path: \"{originalProjectPath}\"");
        }

        /// <summary>
        /// Creates the symbolically linked folder and and symbolically links the Assets, ProjectSettings & Packages folders
        /// </summary>
        public void CreateSymbolicallyLinkedProject()
        {
            if (StartUpMode == 1)
            {
                try
                {
                    Directory.CreateDirectory(symbolicLinkProjectPath);
                }
                catch (Exception e)
                {
                    WriteLine("The Symbolic Link directory failed to be created...");
                }
            }else if (StartUpMode == 2)
            {
                try
                {
                    //only delete if Assets folder is Symbolic and exists
                    var assetsPath = $"{symbolicLinkProjectPath}\\Assets";
                    if (Directory.Exists(assetsPath) && new FileInfo(assetsPath).Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        Directory.Delete(assetsPath,true);
                    }
                    //only delete if ProjectSettings folder is Symbolic and exists
                    var projectSettingsPath = $"{symbolicLinkProjectPath}\\ProjectSettings";
                    if (Directory.Exists(projectSettingsPath) && new FileInfo(projectSettingsPath).Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        Directory.Delete(projectSettingsPath,true);
                    }
                    //only delete if Packages folder is Symbolic and exists
                    var packagesPath = $"{symbolicLinkProjectPath}\\Packages";
                    if (Directory.Exists(packagesPath) && new FileInfo(packagesPath).Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        Directory.Delete(packagesPath,true);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
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
        /// Waits for a character specified by <see cref="acceptableConsoleKeys"/>
        /// </summary>
        /// <returns>The character that was entered</returns>
        private static char WaitForAcceptableInput(params char[] acceptableConsoleKeys)
        {
            ConsoleKeyInfo enteredKey;
            char? isValidInput = null; 
            do
            {
                enteredKey = ReadKey();
                if (acceptableConsoleKeys.Any(acceptableConsoleKey => enteredKey.KeyChar == acceptableConsoleKey))
                {
                    isValidInput = enteredKey.KeyChar;
                }

                if (isValidInput != null)
                {
                    continue;
                }
                Write("\nInvalid input. Valid inputs are ");
                WriteLine(string.Join(", ", acceptableConsoleKeys.Select(key => key.ToString()).ToArray()));
            } while (isValidInput == null);
            
            return (char)isValidInput;
        }
    }
}