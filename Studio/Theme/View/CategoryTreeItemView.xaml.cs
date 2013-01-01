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
    public partial class CategoryTreeItemView : UserControl, IComponentConnector
    {

        // Methods
        public CategoryTreeItemView()
        {
            this.InitializeComponent();
        }

        private void CategoryDrag(object sender, BeginDragEventArgs e)
        {
            this.ViewModel.DoDragDrop((UIElement)sender, e.DragPoint);
        }

        private void CategoryDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = this.ViewModel.GetDropEffects(e.Data);
            e.Handled = true;
        }

        private void CategoryDrop(object sender, DragEventArgs e)
        {
            this.ViewModel.AcceptData(e.Data);
            e.Handled = true;
        }


        // Properties
        private ThemeItemCategoryModel ViewModel
        {
            get
            {
                return (ThemeItemCategoryModel)base.DataContext;
            }
        }
    }



}
