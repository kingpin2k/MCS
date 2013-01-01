


using Advent.MediaCenter;
using Advent.MediaCenter.Theme.Default;
using Advent.MediaCenter.Theme.TVPack;
using Advent.MediaCenter.Theme.Windows7;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Advent.MediaCenter.Theme
{
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Default.FontsApplicator), MajorVersion = 6, MinorVersion = 0)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Windows7.FontsApplicator), BuildVersionMin = 7600, MajorVersion = 6, MinorVersion = 1)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.TVPack.FontsApplicator), BuildVersionMax = 7599, MajorVersion = 6, MinorVersion = 1)]
    public class FontsThemeItem : ThemeItemBase
    {
        public override string Name
        {
            get
            {
                return "Fonts";
            }
        }

        public ObservableCollection<FontClass> FontClasses { get; set; }

        public ObservableCollection<FontOverride> FontOverrides { get; set; }

        public FontsThemeItem()
        {
            this.FontClasses = new ObservableCollection<FontClass>();
            this.FontClasses.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
            this.FontOverrides = new ObservableCollection<FontOverride>();
            this.FontOverrides.CollectionChanged += new NotifyCollectionChangedEventHandler(this.CollectionChanged);
        }

        public static IEnumerable<FontClass> GetFontClasses(MediaCenterLibraryCache cache)
        {
            return ThemeItemBase.CreateApplicator<FontsThemeItem, FontsThemeItem.IFontsThemeItemApplicator>().GetFontClasses(cache);
        }

        public static IEnumerable<FontOverride> GetFontOverrides(MediaCenterLibraryCache cache, MediaCenterTheme theme)
        {
            return ThemeItemBase.CreateApplicator<FontsThemeItem, FontsThemeItem.IFontsThemeItemApplicator>().GetFontOverrides(cache, theme);
        }

        protected override void LoadInternal()
        {
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged notifyPropertyChanged in (IEnumerable)e.OldItems)
                    notifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged notifyPropertyChanged in (IEnumerable)e.NewItems)
                    notifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
            }
            this.IsDirty = true;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        internal interface IFontsThemeItemApplicator : IThemeItemApplicator
        {
            IEnumerable<FontClass> GetFontClasses(MediaCenterLibraryCache cache);

            IEnumerable<FontOverride> GetFontOverrides(MediaCenterLibraryCache cache, MediaCenterTheme theme);
        }
    }
}
