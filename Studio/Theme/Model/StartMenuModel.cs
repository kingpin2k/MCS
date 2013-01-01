using Advent.MediaCenter.Theme;

namespace Advent.VmcStudio.Theme.Model
{
    internal class StartMenuModel : ThemeItemModel
    {
        private readonly StartMenuThemeItem startMenuItem;

        public FontOverrideModel StripTitleFont { get; private set; }

        public StartMenuModel(IThemeItem themeItem, ThemeModel theme, bool isDefault)
            : base(themeItem, theme, isDefault)
        {
            this.startMenuItem = (StartMenuThemeItem)themeItem;
            this.StripTitleFont = new FontOverrideModel(this.startMenuItem.StripTitleFont, theme.FontsItem, "Strip title font");
        }
    }
}
