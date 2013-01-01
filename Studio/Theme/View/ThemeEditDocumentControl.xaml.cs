using Advent.MediaCenter.Theme;
using Advent.VmcStudio.Theme.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
namespace Advent.VmcStudio.Theme.View
{
    public partial class ThemeEditDocumentControl : UserControl, IComponentConnector
    {
        internal ThemeEditDocument Document
        {
            get
            {
                return (ThemeEditDocument)base.DataContext;
            }
        }
        public ThemeEditDocumentControl()
        {
            this.InitializeComponent();
        }
        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MediaCenterTheme theme = this.Document.Theme.Theme;
            e.CanExecute = (theme.CanSave && theme.IsDirty);
            e.Handled = true;
        }
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Document.Theme.Theme.Save();
            e.Handled = true;
        }
        private void ImportImagesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ImagesCategoryModel imagesCategory = this.Document.Theme.ImagesCategory;
            string[] imageFiles = VmcStudioUtil.GetImageFiles();
            if (imageFiles != null)
            {
                imagesCategory.Import(imageFiles);
                this.Document.Theme.UpdateVisibility();
            }
            e.Handled = true;
        }
        
    }
}
