using Advent.MediaCenter.Theme;
using Advent.VmcStudio.Converters;
using Advent.VmcStudio.Theme.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace Advent.VmcStudio.Theme.View
{
    public partial class FontsThemeItemControl : UserControl, IComponentConnector
    {
        
        internal FontsItemModel ViewModel
        {
            get
            {
                return (FontsItemModel)base.DataContext;
            }
        }
        internal FontsThemeItem ThemeItem
        {
            get
            {
                return (FontsThemeItem)this.ViewModel.ThemeItem;
            }
        }
        public FontsThemeItemControl()
        {
            this.InitializeComponent();
            base.DataContextChanged += new DependencyPropertyChangedEventHandler(this.FontsThemeItemControl_DataContextChanged);
        }
        private void FontsThemeItemControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ThemeItem.Load();
            ((StringToFontFamilyConverter)base.Resources["StringToFontFamily"]).FontFamilies = ((FontsItemModel)base.DataContext).Theme.FontFamilies;
        }
        
    }
}
