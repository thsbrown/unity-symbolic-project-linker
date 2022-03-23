namespace UnitySymbolicProjectLinker
{
    internal class UnitySymbolicProjectLinker
    {
        public static void Main(string[] args)
        {
            var consoleHelper = new ConsoleHelper();
            consoleHelper.DisplayInitialPrompt();
            consoleHelper.SelectSymbolicLinkMode();
            if (consoleHelper.StartUpMode == 1)
            {
                consoleHelper.RunModeOne();
            }
            else
            {
                consoleHelper.RunModeTwo();
            }
            consoleHelper.PromptForSubmoduleSymlinkInclusion();
            consoleHelper.CreateSymbolicallyLinkedProject();
        }
    }
}