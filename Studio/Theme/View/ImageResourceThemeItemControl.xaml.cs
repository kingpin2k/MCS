using Advent.Common.UI;
using Advent.MediaCenter.Theme;
using Advent.VmcStudio.Theme.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
namespace Advent.VmcStudio.Theme.View
{
    public partial class ImageResourceThemeItemControl : UserControl, IComponentConnector
    {

        internal ImageResourceModel ViewModel
        {
            get
            {
                return (ImageResourceModel)base.DataContext;
            }
        }
        internal ImageResourceThemeItem ThemeItem
        {
            get
            {
                return (ImageResourceThemeItem)this.ViewModel.ThemeItem;
            }
        }
        public ImageResourceThemeItemControl()
        {
            this.InitializeComponent();
            base.DataContextChanged += new DependencyPropertyChangedEventHandler(this.ImageResourceThemeItemControl_DataContextChanged);
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string imageFile = VmcStudioUtil.GetImageFile();
            if (imageFile != null)
            {
                this.ThemeItem.Image = BitmapDecoder.Create(new Uri(imageFile), BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
            }
        }
        private void ImageResourceThemeItemControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ThemeItem.Load();
        }
        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = this.ViewModel.GetDropEffects(e.Data);
            e.Handled = true;
        }
        private void Image_Drop(object sender, DragEventArgs e)
        {
            this.ViewModel.AcceptData(e.Data);
            e.Handled = true;
        }
        private void Image_Drag(object sender, BeginDragEventArgs e)
        {
            this.ViewModel.DoDragDrop((UIElement)sender, e.DragPoint);
        }
    }
}
