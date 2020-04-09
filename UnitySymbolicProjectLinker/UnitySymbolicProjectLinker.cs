namespace UnitySymbolicProjectLinker
{
    internal class UnitySymbolicProjectLinker
    {
        public static void Main(string[] args)
        {
            var consoleHelper = new ConsoleHelper();
            consoleHelper.DisplayInitialPrompt();
            consoleHelper.GetProjectPathsFromUser();
            consoleHelper.CreateSymbolicallyLinkedProject();
            consoleHelper.AddAdditionalSymLinkProjectFoldersAndFiles();
        }
    }
}