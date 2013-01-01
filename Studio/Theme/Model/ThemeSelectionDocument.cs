using Advent.VmcStudio;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ThemeSelectionDocument : VmcDocument
    {
        private ThemeSummary selectedTheme;

        public ThemeSummary SelectedTheme
        {
            get
            {
                return this.selectedTheme;
            }
            set
            {
                if (this.selectedTheme == value)
                    return;
                this.selectedTheme = value;
                this.OnPropertyChanged("SelectedTheme");
            }
        }

        public ThemeManager ThemeManager { get; private set; }

        public ThemeSelectionDocument(VmcStudioApp app)
        {
            this.ThemeManager = app.ThemeManager;
            this.Name = "Themes";
        }
    }
}
