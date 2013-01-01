using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.Common.UI;
using Advent.MediaCenter.Theme;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ThemeModel : ThemeItemCategoryModel
    {
        public static readonly DependencyProperty HideDefaultItemsProperty = DependencyProperty.Register("HideDefaultItems", typeof(bool), typeof(ThemeModel), new PropertyMetadata(new PropertyChangedCallback(ThemeModel.OnHideDefaultItemsChanged)));
        private ObservableCollection<FontFamily> fontFamilies;
        private List<IThemeItem> defaultItems;
        private Dictionary<ResourceThemeItem, IResource> defaultResources;

        public ImagesCategoryModel ImagesCategory { get; private set; }

        public ThemeManager ThemeManager { get; private set; }

        public FontsItemModel FontsItem { get; private set; }

        public ColorsItemModel ColorsItem { get; private set; }

        public BiographyModel BiographyItem { get; private set; }

        public bool HideDefaultItems
        {
            get
            {
                return (bool)this.GetValue(ThemeModel.HideDefaultItemsProperty);
            }
            set
            {
                this.SetValue(ThemeModel.HideDefaultItemsProperty, value);
            }
        }

        public ObservableCollection<FontFamily> FontFamilies
        {
            get
            {
                if (this.fontFamilies == null)
                {
                    this.fontFamilies = new ObservableCollection<FontFamily>();
                    foreach (FontFamily fontFamily in (IEnumerable<FontFamily>)Fonts.SystemFontFamilies)
                        this.AddFont((ICollection<FontFamily>)this.fontFamilies, fontFamily);
                    using (IEnumerator<FontFamily> enumerator = this.Theme.Fonts.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            FontFamily font = enumerator.Current;
                            if (Enumerable.FirstOrDefault<FontFamily>((IEnumerable<FontFamily>)this.fontFamilies, (Func<FontFamily, bool>)(o => FontUtil.GetName(o) == FontUtil.GetName(font))) == null)
                                this.AddFont((ICollection<FontFamily>)this.fontFamilies, font);
                        }
                    }
                }
                return this.fontFamilies;
            }
        }

        public MediaCenterTheme Theme { get; private set; }

        private IEnumerable<IThemeItem> DefaultThemeItems
        {
            get
            {
                if (this.defaultItems == null)
                {
                    this.defaultItems = new List<IThemeItem>();
                    this.defaultResources = new Dictionary<ResourceThemeItem, IResource>();
                    foreach (IResource resource in this.ThemeManager.DefaultResources)
                    {
                        IThemeItem themeItem = (IThemeItem)null;
                        if (Path.GetExtension(resource.Name).ToLowerInvariant() == ".png")
                        {
                            themeItem = (IThemeItem)new ImageResourceThemeItem(Path.GetFileName(((UnmanagedLibrary)resource.Library).File), resource.Name, (Func<ResourceThemeItem, byte[]>)(o => ResourceExtensions.GetBytes(this.defaultResources[o])));
                            this.defaultResources[(ResourceThemeItem)themeItem] = resource;
                        }
                        if (themeItem != null)
                            this.defaultItems.Add(themeItem);
                    }
                }
                return (IEnumerable<IThemeItem>)this.defaultItems;
            }
        }

        static ThemeModel()
        {
        }

        public ThemeModel(MediaCenterTheme theme, ThemeManager manager)
            : base((string)null, (ThemeTreeItem)null)
        {
            this.Theme = theme;
            this.ThemeManager = manager;
            this.BiographyItem = new BiographyModel(this);
            this.FontsItem = new FontsItemModel((IThemeItem)theme.FontsItem, this, false);
            this.ColorsItem = new ColorsItemModel((IThemeItem)theme.ColorsItem, this, false);
            this.ImagesCategory = new ImagesCategoryModel("Images", (ThemeTreeItem)this);
            this.Children.Add((ThemeTreeItem)this.BiographyItem);
            this.Children.Add((ThemeTreeItem)this.FontsItem);
            this.Children.Add((ThemeTreeItem)this.ColorsItem);
            this.Children.Add((ThemeTreeItem)this.ImagesCategory);
            this.BiographyItem.IsSelected = true;
            foreach (IThemeItem themeItem in (Collection<IThemeItem>)this.Theme.ThemeItems)
                this.AddThemeItem(themeItem, false);
            foreach (IThemeItem themeItem in this.DefaultThemeItems)
                this.AddThemeItem(themeItem, true);
            this.Sort();
        }

        private void AddFont(ICollection<FontFamily> fontCollection, FontFamily fontFamily)
        {
            bool flag = false;
            try
            {
                foreach (Typeface typeface in (IEnumerable<Typeface>)fontFamily.GetTypefaces())
                {
                    GlyphTypeface glyphTypeface;
                    if (typeface.TryGetGlyphTypeface(out glyphTypeface) && glyphTypeface.Symbol)
                        flag = true;
                }
            }
            catch
            {
                flag = true;
            }
            if (flag)
                return;
            this.fontFamilies.Add(fontFamily);
        }

        private static bool ThemeItemsEqual(IThemeItem item1, IThemeItem item2)
        {
            ImageResourceThemeItem resourceThemeItem1 = item1 as ImageResourceThemeItem;
            ImageResourceThemeItem resourceThemeItem2 = item2 as ImageResourceThemeItem;
            if (resourceThemeItem1 == null || resourceThemeItem2 == null)
                return item1 == item2;
            if (resourceThemeItem1.ResourceName == resourceThemeItem2.ResourceName && resourceThemeItem1.ResourceType == resourceThemeItem2.ResourceType)
                return resourceThemeItem1.DllName == resourceThemeItem2.DllName;
            else
                return false;
        }

        public void UpdateVisibility()
        {
            ThemeModel.UpdateVisibility(this, (ThemeTreeItem)this);
        }

        private static void OnHideDefaultItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((ThemeModel)dependencyObject).UpdateVisibility();
        }

        private static void UpdateVisibility(ThemeModel themeModel, ThemeTreeItem item)
        {
            ThemeItemModel themeItemModel = item as ThemeItemModel;
            if (themeItemModel != null)
            {
                if (themeModel.HideDefaultItems)
                    themeItemModel.Visibility = themeItemModel.IsDefault ? Visibility.Collapsed : Visibility.Visible;
                else
                    themeItemModel.Visibility = Visibility.Visible;
            }
            else
            {
                ThemeItemCategoryModel itemCategoryModel = item as ThemeItemCategoryModel;
                if (itemCategoryModel == null)
                    return;
                foreach (ThemeTreeItem themeTreeItem in (Collection<ThemeTreeItem>)itemCategoryModel.Children)
                    ThemeModel.UpdateVisibility(themeModel, themeTreeItem);
                if (Enumerable.FirstOrDefault<ThemeTreeItem>((IEnumerable<ThemeTreeItem>)itemCategoryModel.Children, (Func<ThemeTreeItem, bool>)(o => o.Visibility != Visibility.Collapsed)) == null)
                    itemCategoryModel.Visibility = Visibility.Collapsed;
                else
                    itemCategoryModel.Visibility = Visibility.Visible;
            }
        }

        private ThemeItemCategoryModel GetCategory(ThemeItemModel item)
        {
            string[] categoryNames = (item.Category ?? string.Empty).Split(new char[1]
      {
        '\\'
      });
            ThemeItemCategoryModel itemCategoryModel1 = (ThemeItemCategoryModel)this;
            for (int index = 0; index < categoryNames.Length; ++index)
            {
                if (categoryNames[index] != string.Empty)
                {
                    int i1 = index;
                    ThemeItemCategoryModel itemCategoryModel2 = Enumerable.FirstOrDefault<ThemeItemCategoryModel>(Enumerable.Where<ThemeItemCategoryModel>(Enumerable.OfType<ThemeItemCategoryModel>((IEnumerable)itemCategoryModel1.Children), (Func<ThemeItemCategoryModel, bool>)(c => c.Name == categoryNames[i1])));
                    if (itemCategoryModel2 == null)
                    {
                        itemCategoryModel2 = item.CreateCategory(categoryNames[index], (ThemeTreeItem)itemCategoryModel1);
                        itemCategoryModel1.Children.Add((ThemeTreeItem)itemCategoryModel2);
                    }
                    itemCategoryModel1 = itemCategoryModel2;
                }
            }
            return itemCategoryModel1;
        }

        private void AddThemeItem(IThemeItem item, bool isDefault)
        {
            ThemeItemModel model;
            if (item is ImageResourceThemeItem)
                model = (ThemeItemModel)new ImageResourceModel(item, this, isDefault);
            else if (item is SoundResourceThemeItem)
            {
                model = (ThemeItemModel)new SoundResourceModel(item, this, isDefault);
            }
            else
            {
                if (item is StartMenuThemeItem || item is AnimationsItem)
                    return;
                model = new ThemeItemModel(item, this, isDefault);
            }
            this.AddThemeItem(model, isDefault);
        }

        private void AddThemeItem(ThemeItemModel model, bool isDefault)
        {
            ThemeItemCategoryModel category = this.GetCategory(model);
            if (Enumerable.FirstOrDefault<ThemeItemModel>(Enumerable.Where<ThemeItemModel>(Enumerable.OfType<ThemeItemModel>((IEnumerable)category.Children), (Func<ThemeItemModel, bool>)(t => ThemeModel.ThemeItemsEqual(t.ThemeItem, model.ThemeItem)))) != null)
                return;
            model.Parent = (ThemeTreeItem)category;
            category.Children.Add((ThemeTreeItem)model);
        }
    }
}
