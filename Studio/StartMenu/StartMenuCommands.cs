using System.Windows.Input;

namespace Advent.VmcStudio.StartMenu
{
    public static class StartMenuCommands
    {
        private static readonly RoutedUICommand NewStripCommand = new RoutedUICommand("New Strip", "NewStrip", typeof(StartMenuCommands));
        private static readonly RoutedUICommand NewApplicationCommand = new RoutedUICommand("New Application", "NewApplication", typeof(StartMenuCommands));
        private static readonly RoutedUICommand NewGameCommand = new RoutedUICommand("New Game", "NewGame", typeof(StartMenuCommands));

        public static RoutedUICommand NewStrip
        {
            get
            {
                return StartMenuCommands.NewStripCommand;
            }
        }

        public static RoutedUICommand NewApplication
        {
            get
            {
                return StartMenuCommands.NewApplicationCommand;
            }
        }

        public static RoutedUICommand NewGame
        {
            get
            {
                return StartMenuCommands.NewGameCommand;
            }
        }

        static StartMenuCommands()
        {
        }
    }
}
