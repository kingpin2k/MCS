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
    public partial class FontOverrideView : UserControl, IComponentConnector
    {
        
        public FontOverrideView()
        {
            this.InitializeComponent();
        }
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((StringToFontClassConverter)base.Resources["StringToFontClass"]).FontClasses = ((FontOverrideModel)base.DataContext).FontsItem.FontClasses;
        }
        
    }
}
