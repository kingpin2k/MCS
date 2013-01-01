using System.Windows.Input;

namespace Advent.VmcStudio.Theme.Model
{
    public class ThemeCommands
    {
        private static readonly RoutedUICommand ApplyTheme = new RoutedUICommand("Apply", "ApplyTheme", typeof(ThemeCommands));
        private static readonly RoutedUICommand ImportTheme = new RoutedUICommand("Import", "ImportTheme", typeof(ThemeCommands));
        private static readonly RoutedUICommand ExportTheme = new RoutedUICommand("Export", "ExportTheme", typeof(ThemeCommands));

        public static RoutedUICommand Apply
        {
            get
            {
                return ThemeCommands.ApplyTheme;
            }
        }

        public static RoutedUICommand Import
        {
            get
            {
                return ThemeCommands.ImportTheme;
            }
        }

        public static RoutedUICommand Export
        {
            get
            {
                return ThemeCommands.ExportTheme;
            }
        }

        static ThemeCommands()
        {
        }
    }
}
