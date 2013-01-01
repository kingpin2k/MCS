using Advent.MediaCenter.Theme;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace Advent.VmcStudio.Theme.Model
{
    internal class ColorsItemModel : ThemeItemModel
    {
        private readonly ColorsThemeItem colorsItem;

        public ObservableCollection<ColorItemModel> DefaultColors { get; private set; }

        public ColorsItemModel(IThemeItem themeItem, ThemeModel theme, bool isDefault)
            : base(themeItem, theme, isDefault)
        {
            this.Name = "Colors";
            this.DefaultColors = new ObservableCollection<ColorItemModel>();
            Dictionary<string, ColorItemModel> dictionary = new Dictionary<string, ColorItemModel>();
            this.colorsItem = (ColorsThemeItem)themeItem;
            foreach (ColorItem colorItem in (Collection<ColorItem>)this.colorsItem.DefaultColors)
            {
                ColorItemModel colorItemModel = new ColorItemModel(colorItem);
                dictionary[colorItem.Name] = colorItemModel;
            }
            foreach (ColorItem colorItem in Enumerable.Where<ColorItem>(ColorsThemeItem.GetColors(theme.ThemeManager.BackupCache), (Func<ColorItem, bool>)(c => Enumerable.FirstOrDefault<ColorItemModel>((IEnumerable<ColorItemModel>)this.DefaultColors, (Func<ColorItemModel, bool>)(o => o.Item.Name == c.Name)) == null)))
            {
                ColorItemModel colorItemModel1;
                if (!dictionary.TryGetValue(colorItem.Name, out colorItemModel1))
                {
                    ColorItemModel colorItemModel2 = new ColorItemModel(colorItem);
                    colorItemModel2.DefaultValue = new Color?(colorItem.Color);
                    colorItemModel1 = colorItemModel2;
                    dictionary[colorItem.Name] = colorItemModel1;
                }
                colorItemModel1.DefaultValue = new Color?(colorItem.Color);
            }
            foreach (ColorItemModel colorItemModel in dictionary.Values)
            {
                this.DefaultColors.Add(colorItemModel);
                colorItemModel.PropertyChanged += new PropertyChangedEventHandler(this.ColorItem_PropertyChanged);
            }
            theme.Theme.Saved += new EventHandler(this.Theme_Saved);
        }

        private void Theme_Saved(object sender, EventArgs e)
        {
            foreach (ViewModelItem<ColorItem, Color?> viewModelItem in (Collection<ColorItemModel>)this.DefaultColors)
                viewModelItem.ClearDirty();
        }

        private void ColorItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "IsDefault"))
                return;
            ColorItemModel colorItemModel = (ColorItemModel)sender;
            if (!colorItemModel.IsDefault)
            {
                if (this.colorsItem.DefaultColors.Contains(colorItemModel.Item))
                    return;
                this.colorsItem.DefaultColors.Add(colorItemModel.Item);
            }
            else
                this.colorsItem.DefaultColors.Remove(colorItemModel.Item);
        }
    }
}
