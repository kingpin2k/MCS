using System.Windows.Input;

namespace Advent.VmcStudio
{
    public class VmcStudioCommands
    {
        private static readonly RoutedUICommand RestoreDefaultCommand = new RoutedUICommand("Restore Default", "RestoreDefault", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand AboutCommand = new RoutedUICommand("About", "About", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand CreateSupportPackageCommand = new RoutedUICommand("Create Support Package", "CreateSupportPackage", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand InsertImageCommand = new RoutedUICommand("Image", "InsertImage", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand InsertVideoCommand = new RoutedUICommand("Video", "InsertVideo", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand InsertAudioCommand = new RoutedUICommand("Audio", "InsertAudio", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand ThemesCommand = new RoutedUICommand("Themes", "Themes", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand StartMenuCommand = new RoutedUICommand("Start Menu", "StartMenu", typeof(VmcStudioCommands));
        private static readonly RoutedUICommand StartMediaCenterCommand = new RoutedUICommand("Start Media Center", "StartMediaCenter", typeof(VmcStudioCommands));

        public static RoutedUICommand RestoreDefault
        {
            get
            {
                return VmcStudioCommands.RestoreDefaultCommand;
            }
        }

        public static RoutedUICommand About
        {
            get
            {
                return VmcStudioCommands.AboutCommand;
            }
        }

        public static RoutedUICommand CreateSupportPackage
        {
            get
            {
                return VmcStudioCommands.CreateSupportPackageCommand;
            }
        }

        public static RoutedUICommand InsertImage
        {
            get
            {
                return VmcStudioCommands.InsertImageCommand;
            }
        }

        public static RoutedUICommand InsertVideo
        {
            get
            {
                return VmcStudioCommands.InsertVideoCommand;
            }
        }

        public static RoutedUICommand InsertAudio
        {
            get
            {
                return VmcStudioCommands.InsertAudioCommand;
            }
        }

        public static RoutedUICommand Themes
        {
            get
            {
                return VmcStudioCommands.ThemesCommand;
            }
        }

        public static RoutedUICommand StartMenu
        {
            get
            {
                return VmcStudioCommands.StartMenuCommand;
            }
        }

        public static RoutedUICommand StartMediaCenter
        {
            get
            {
                return VmcStudioCommands.StartMediaCenterCommand;
            }
        }

        static VmcStudioCommands()
        {
        }
    }
}
