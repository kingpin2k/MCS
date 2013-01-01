using Advent.MediaCenter.Theme;
using System;
using System.Windows;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ThemeItemModel : ThemeTreeItem
    {
        public static readonly DependencyPropertyKey IsDefaultPropertyKey = DependencyProperty.RegisterReadOnly("IsDefault", typeof(bool), typeof(ThemeItemModel), new PropertyMetadata(new PropertyChangedCallback(ThemeItemModel.OnIsDefaultChanged)));
        public static readonly DependencyProperty IsDefaultProperty = ThemeItemModel.IsDefaultPropertyKey.DependencyProperty;

        public ThemeModel Theme { get; private set; }

        public IThemeItem ThemeItem { get; private set; }

        public bool IsDefault
        {
            get
            {
                return (bool)this.GetValue(ThemeItemModel.IsDefaultProperty);
            }
        }

        public virtual string Category
        {
            get
            {
                return string.Empty;
            }
        }

        static ThemeItemModel()
        {
        }

        public ThemeItemModel(IThemeItem themeItem, ThemeModel theme, bool isDefault)
        {
            this.ThemeItem = themeItem;
            this.Theme = theme;
            this.Name = themeItem.Name;
            themeItem.IsDirtyChanged += new EventHandler(this.ThemeItem_IsDirtyChanged);
            this.SetValue(ThemeItemModel.IsDefaultPropertyKey, isDefault);
        }

        public virtual ThemeItemCategoryModel CreateCategory(string name, ThemeTreeItem parent)
        {
            return new ThemeItemCategoryModel(name, parent);
        }

        private static void OnIsDefaultChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ThemeItemModel themeItemModel = (ThemeItemModel)dependencyObject;
            if (themeItemModel.Theme == null)
                return;
            if ((bool)e.NewValue)
            {
                if (!themeItemModel.Theme.Theme.ThemeItems.Contains(themeItemModel.ThemeItem))
                    return;
                themeItemModel.Theme.Theme.ThemeItems.Remove(themeItemModel.ThemeItem);
            }
            else
            {
                if (themeItemModel.Theme.Theme.ThemeItems.Contains(themeItemModel.ThemeItem))
                    return;
                themeItemModel.Theme.Theme.ThemeItems.Add(themeItemModel.ThemeItem);
            }
        }

        private void ThemeItem_IsDirtyChanged(object sender, EventArgs e)
        {
            if (!this.ThemeItem.IsDirty)
                return;
            this.SetValue(ThemeItemModel.IsDefaultPropertyKey, (object)false);
        }
    }
}
