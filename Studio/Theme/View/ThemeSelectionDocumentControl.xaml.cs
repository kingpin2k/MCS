using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.Common.UI;
using Advent.MediaCenter.Theme;
using Advent.VmcStudio.Theme.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
namespace Advent.VmcStudio.Theme.View
{
    public partial class ThemeSelectionDocumentControl : UserControl, IComponentConnector, IStyleConnector
    {
        internal ThemeSelectionDocument Document
        {
            get
            {
                return (ThemeSelectionDocument)base.DataContext;
            }
        }
        public ThemeSelectionDocumentControl()
        {
            this.InitializeComponent();
        }
        private void HandleThemeDrag(object sender, BeginDragEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            ThemeSummary themeSummary = frameworkElement.DataContext as ThemeSummary;
            IDataObject dataObject = Advent.Common.Interop.DataObject.CreateDataObject();
            dataObject.SetVirtualFiles(new VirtualFile[]
			{
				new VirtualFile(themeSummary.Name + Path.GetExtension(themeSummary.ThemeFullPath), FileUtil.ReadAllBytes(themeSummary.ThemeFullPath, FileShare.ReadWrite))
			});
            dataObject.DoDragDrop((UIElement)sender, e.DragPoint, DragDropEffects.Copy);
        }
        private void ThemesList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            string[] array = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (array != null)
            {
                bool flag = true;
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string path = array2[i];
                    if (!MediaCenterTheme.IsThemeFile(path) && !Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    e.Effects = DragDropEffects.Copy;
                    string insert = (array.Length == 1) ? Path.GetFileName(array[0]) : "themes";
                    Advent.Common.UI.DragDrop.SetDropDescriptionInsert(this.themesList, insert);
                }
            }
        }
        private void ThemesList_Drop(object sender, DragEventArgs e)
        {
            string[] array = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (array != null)
            {
                VmcStudioUtil.Application.ExclusiveOperation = VmcStudioUtil.Application.ThemeManager.ImportThemes(array);
            }
        }
        private void OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.Document.SelectedTheme != null && (
                from d in VmcStudioUtil.Application.Documents.OfType<ThemeEditDocument>()
                where d.Theme.Theme.ID == this.Document.SelectedTheme.ID
                select d).Count<ThemeEditDocument>() == 0);
            e.Handled = true;
        }
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            VmcDocument vmcDocument = new ThemeEditDocument(new ThemeModel(this.Document.SelectedTheme.OpenTheme(), this.Document.ThemeManager));
            VmcStudioUtil.Application.Documents.Add(vmcDocument);
            VmcStudioUtil.Application.SelectedDocument = vmcDocument;
            e.Handled = true;
        }
        private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.Document.SelectedTheme != null);
            e.Handled = true;
        }
        private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete {0}?", this.Document.SelectedTheme.Name), VmcStudioUtil.ApplicationTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.Document.ThemeManager.DeleteTheme(this.Document.SelectedTheme);
                e.Handled = true;
            }
        }
        private void NewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MediaCenterTheme theme = this.Document.ThemeManager.CreateTheme().OpenTheme();
            VmcDocument vmcDocument = new ThemeEditDocument(new ThemeModel(theme, this.Document.ThemeManager));
            VmcStudioUtil.Application.Documents.Add(vmcDocument);
            VmcStudioUtil.Application.SelectedDocument = vmcDocument;
            e.Handled = true;
        }
        private void ApplyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.Document.SelectedTheme != null);
            e.Handled = true;
        }
        private void ApplyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            switch (this.Document.SelectedTheme.ThemeType)
            {
                case ThemeType.Base:
                    foreach (ThemeSummary current in new List<ThemeSummary>(
                        from o in this.Document.ThemeManager.AppliedThemes
                        where o.ThemeType == ThemeType.Base
                        select o))
                    {
                        this.Document.ThemeManager.AppliedThemes.Remove(current);
                    }
                    this.Document.ThemeManager.AppliedThemes.Insert(0, this.Document.SelectedTheme);
                    break;
                case ThemeType.AddOn:
                    this.Document.ThemeManager.AppliedThemes.Add(this.Document.SelectedTheme);
                    break;
            }
            VmcStudioUtil.Application.ExclusiveOperation = this.Document.ThemeManager.ApplyThemesAsync(true, true);
            e.Handled = true;
        }
        private void ExportCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.Document.SelectedTheme != null && Path.GetExtension(this.Document.SelectedTheme.ThemeFile) == ".mct");
            e.Handled = true;
        }
        private void ExportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = this.Document.SelectedTheme.Name,
                AddExtension = true,
                OverwritePrompt = true,
                ValidateNames = true,
                DefaultExt = ".mct",
                Filter = string.Format("Media Center Themes ({0})|*{0}", ".mct")
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.Copy(this.Document.SelectedTheme.ThemeFullPath, saveFileDialog.FileName, true);
            }
            e.Handled = true;
        }
        private void ImportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                FileName = string.Empty,
                DefaultExt = ".mct",
                Multiselect = true,
                Filter = string.Format("Media Center Themes ({0})|*{0}|MediaCenterFX Themes ({1})|*{1}|Modified ehres.dll (.dll)|*.dll", ".mct", ".vmcthemepack")
            };
            if (openFileDialog.ShowDialog() == true)
            {
                VmcStudioUtil.Application.ExclusiveOperation = VmcStudioUtil.Application.ThemeManager.ImportThemes(openFileDialog.FileNames);
            }
            e.Handled = true;
        }
        private void ThemeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ApplicationCommands.Open.Execute(null, (IInputElement)sender);
            }
        }

        void IStyleConnector.Connect(int connectionId, object target)
        {
            if (connectionId != 1)
            {
                return;
            }
            ((Grid)target).AddHandler(Advent.Common.UI.DragDrop.DragEvent, new BeginDragEventHandler(this.HandleThemeDrag));
            ((Grid)target).MouseLeftButtonDown += new MouseButtonEventHandler(this.ThemeMouseLeftButtonDown);
        }
    }
}
