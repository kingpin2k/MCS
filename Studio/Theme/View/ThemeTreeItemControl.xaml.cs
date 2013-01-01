using Advent.Common.UI;
using Advent.VmcStudio.Theme.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace Advent.VmcStudio.Theme.View
{
    public partial class ThemeTreeItemControl : UserControl, IComponentConnector
    {

        private ThemeItemModel ViewModel
        {
            get
            {
                return (ThemeItemModel)base.DataContext;
            }
        }
        public ThemeTreeItemControl()
        {
            this.InitializeComponent();
        }
        private void ThemeItemViewModel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = this.ViewModel.GetDropEffects(e.Data);
            e.Handled = true;
        }
        private void ThemeItemViewModel_Drop(object sender, DragEventArgs e)
        {
            this.ViewModel.AcceptData(e.Data);
            e.Handled = true;
        }
        private void ThemeItemViewModel(object sender, BeginDragEventArgs e)
        {
            this.ViewModel.DoDragDrop((UIElement)sender, e.DragPoint);
        }
    
    }
}
