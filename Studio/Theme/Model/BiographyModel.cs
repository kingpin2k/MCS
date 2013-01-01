using Advent.MediaCenter.Theme;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Advent.VmcStudio.Theme.Model
{
    internal class BiographyModel : ThemeItemModel
    {
        public BiographyModel(ThemeModel theme)
            : base((IThemeItem)new BiographyModel.BiographyThemeItem(theme.Theme), theme, false)
        {
            this.ThemeItem.BeginInit();
            this.ThemeItem.EndInit();
        }

        public class BiographyThemeItem : ThemeItemBase
        {
            public string ThemeName
            {
                get
                {
                    return this.Theme.Name;
                }
                set
                {
                    this.Theme.Name = value;
                    this.OnPropertyChanged("ThemeName");
                }
            }

            public string Author
            {
                get
                {
                    return this.Theme.Author;
                }
                set
                {
                    this.Theme.Author = value;
                    this.OnPropertyChanged("Author");
                }
            }

            public string Comments
            {
                get
                {
                    return this.Theme.Comments;
                }
                set
                {
                    this.Theme.Comments = value;
                    this.OnPropertyChanged("Comments");
                }
            }

            public BitmapSource MainScreenshot
            {
                get
                {
                    return this.Theme.MainScreenshot;
                }
                set
                {
                    this.Theme.MainScreenshot = value;
                    this.OnPropertyChanged("MainScreenshot");
                }
            }

            public ObservableCollection<BitmapSource> Screenshots
            {
                get
                {
                    return this.Theme.Screenshots;
                }
            }

            public override string Name
            {
                get
                {
                    return "Biography";
                }
            }

            public BiographyThemeItem(MediaCenterTheme theme)
            {
                this.Theme = theme;
                theme.Saved += new EventHandler(this.ThemeSaved);
            }

            protected override void LoadInternal()
            {
            }

            private void ThemeSaved(object sender, EventArgs e)
            {
                this.IsDirty = false;
            }
        }
    }
}
