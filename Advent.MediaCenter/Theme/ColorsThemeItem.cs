// Type: Advent.MediaCenter.Theme.ColorsThemeItem



using Advent.Common.UI;
using Advent.MediaCenter;
using Advent.MediaCenter.Theme.Default;
using Advent.MediaCenter.Theme.TVPack;
using Advent.MediaCenter.Theme.Windows7;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.TVPack.ColorsApplicator), BuildVersionMax = 7599, MajorVersion = 6, MinorVersion = 1)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Windows7.ColorsApplicator), BuildVersionMin = 7600, MajorVersion = 6, MinorVersion = 1)]
    [MediaCenterVersion(typeof(Advent.MediaCenter.Theme.Default.ColorsApplicator), MajorVersion = 6, MinorVersion = 0)]
    public class ColorsThemeItem : ThemeItemBase
    {
        public override string Name
        {
            get
            {
                return "Colors";
            }
        }

        [XmlArray("Default")]
        public ObservableCollection<ColorItem> DefaultColors { get; set; }

        public ColorsThemeItem()
        {
            this.DefaultColors = new ObservableCollection<ColorItem>();
            this.DefaultColors.CollectionChanged += new NotifyCollectionChangedEventHandler(this.DefaultColors_CollectionChanged);
        }

        public static IEnumerable<ColorItem> GetColors(MediaCenterLibraryCache cache)
        {
            return ThemeItemBase.CreateApplicator<ColorsThemeItem, ColorsThemeItem.IColorsThemeItemApplicator>().GetColors(cache);
        }

        protected override void LoadInternal()
        {
        }

        private void DefaultColors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (NotifyPropertyChangedBase propertyChangedBase in (IEnumerable)e.OldItems)
                    propertyChangedBase.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
            }
            if (e.NewItems != null)
            {
                foreach (NotifyPropertyChangedBase propertyChangedBase in (IEnumerable)e.NewItems)
                    propertyChangedBase.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
            }
            this.IsDirty = true;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        internal interface IColorsThemeItemApplicator : IThemeItemApplicator
        {
            IEnumerable<ColorItem> GetColors(MediaCenterLibraryCache cache);
        }
    }
}
