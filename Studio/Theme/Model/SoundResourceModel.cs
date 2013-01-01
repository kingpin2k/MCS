using Advent.MediaCenter.Theme;

namespace Advent.VmcStudio.Theme.Model
{
    internal class SoundResourceModel : ThemeItemModel
    {
        public override string Category
        {
            get
            {
                return "Sounds";
            }
        }

        public SoundResourceModel(IThemeItem themeItem, ThemeModel theme, bool isDefault)
            : base(themeItem, theme, isDefault)
        {
        }
    }
}
