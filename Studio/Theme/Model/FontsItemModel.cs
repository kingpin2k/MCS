using Advent.MediaCenter.Theme;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Advent.VmcStudio.Theme.Model
{
    internal class FontsItemModel : ThemeItemModel
    {
        private FontsThemeItem fontsThemeItem;

        public ObservableCollection<FontClassModel> FontClasses { get; private set; }

        public ObservableCollection<FontOverrideModel> FontOverrides { get; private set; }

        public FontsItemModel(IThemeItem themeItem, ThemeModel theme, bool isDefault)
            : base(themeItem, theme, isDefault)
        {
            this.Name = "Fonts";
            this.fontsThemeItem = (FontsThemeItem)themeItem;
            this.FontClasses = new ObservableCollection<FontClassModel>();
            Dictionary<string, FontClassModel> dictionary1 = new Dictionary<string, FontClassModel>();
            foreach (FontClass fontClass in (Collection<FontClass>)this.fontsThemeItem.FontClasses)
                dictionary1[fontClass.Name] = new FontClassModel(fontClass);
            foreach (FontClass fontClass in FontsThemeItem.GetFontClasses(theme.ThemeManager.BackupCache))
            {
                FontClassModel fontClassModel;
                if (!dictionary1.TryGetValue(fontClass.Name, out fontClassModel))
                {
                    fontClassModel = new FontClassModel(fontClass);
                    dictionary1[fontClass.Name] = fontClassModel;
                }
                fontClassModel.DefaultValue = new FontFace(fontClass.FontFace);
            }
            foreach (FontClassModel fontClassModel in dictionary1.Values)
            {
                this.FontClasses.Add(fontClassModel);
                fontClassModel.PropertyChanged += new PropertyChangedEventHandler(this.FontClass_PropertyChanged);
            }
            this.FontOverrides = new ObservableCollection<FontOverrideModel>();
            Dictionary<string, FontOverrideModel> dictionary2 = new Dictionary<string, FontOverrideModel>();
            foreach (FontOverride fontOverride in (Collection<FontOverride>)this.fontsThemeItem.FontOverrides)
                dictionary2[fontOverride.Name] = new FontOverrideModel(fontOverride, this);
            foreach (FontOverride fontOverride in FontsThemeItem.GetFontOverrides(theme.ThemeManager.BackupCache, this.Theme.Theme))
            {
                FontOverrideModel fontOverrideModel;
                if (!dictionary2.TryGetValue(fontOverride.Name, out fontOverrideModel))
                {
                    fontOverrideModel = new FontOverrideModel(fontOverride, this);
                    dictionary2[fontOverride.Name] = fontOverrideModel;
                }
                fontOverrideModel.DefaultValue = fontOverrideModel.Clone(fontOverride);
            }
            foreach (FontOverrideModel fontOverrideModel in dictionary2.Values)
            {
                this.FontOverrides.Add(fontOverrideModel);
                fontOverrideModel.PropertyChanged += new PropertyChangedEventHandler(this.FontOverride_PropertyChanged);
            }
        }

        private void FontOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "IsDefault"))
                return;
            FontOverrideModel fontOverrideModel = (FontOverrideModel)sender;
            if (!fontOverrideModel.IsDefault)
            {
                if (this.fontsThemeItem.FontOverrides.Contains(fontOverrideModel.Item))
                    return;
                this.fontsThemeItem.FontOverrides.Add(fontOverrideModel.Item);
            }
            else
                this.fontsThemeItem.FontOverrides.Remove(fontOverrideModel.Item);
        }

        private void FontClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "IsDefault"))
                return;
            FontClassModel fontClassModel = (FontClassModel)sender;
            if (!fontClassModel.IsDefault)
            {
                if (this.fontsThemeItem.FontClasses.Contains(fontClassModel.Item))
                    return;
                this.fontsThemeItem.FontClasses.Add(fontClassModel.Item);
            }
            else
                this.fontsThemeItem.FontClasses.Remove(fontClassModel.Item);
        }
    }
}
