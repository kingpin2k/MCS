using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu.OEM;
using Advent.VmcExecute;
using Advent.VmcStudio.StartMenu.Views;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Serialization;
namespace Advent.VmcStudio.StartMenu
{
    public partial class StartMenuDocumentView : System.Windows.Controls.UserControl, IComponentConnector
    {
        private const int AccessDenied = 5;
        private const string AccessDeniedMessage = "You do not have permission to perform this action. Please ensure that all extenders are disconnected and all other users are logged off.";
        //internal StartMenu m_startMenu;
        //internal Expander m_entryPointsExpander;
        //internal System.Windows.Controls.ListBox m_entryPointList;
        //private bool _contentLoaded;
        public StartMenuDocumentView()
        {
            this.InitializeComponent();
            
        }
        private void DeleteSelectedEntryPoints()
        {
            if (this.m_entryPointList.SelectedItems.Count == 0)
            {
                return;
            }
            string messageBoxText;
            if (this.m_entryPointList.SelectedItems.Count > 1)
            {
                messageBoxText = "Are you sure you want to delete the selected entry points?";
            }
            else
            {
                Advent.MediaCenter.StartMenu.OEM.EntryPoint entryPoint = (Advent.MediaCenter.StartMenu.OEM.EntryPoint)this.m_entryPointList.SelectedItem;
                messageBoxText = string.Format("Are you sure you want to delete the {0} entry point?", entryPoint.Title);
            }
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(messageBoxText, VmcStudioUtil.ApplicationTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                object[] array = new object[this.m_entryPointList.SelectedItems.Count];
                this.m_entryPointList.SelectedItems.CopyTo(array, 0);
                object[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    Advent.MediaCenter.StartMenu.OEM.EntryPoint item = (Advent.MediaCenter.StartMenu.OEM.EntryPoint)array2[i];
                    this.m_startMenu.StartMenuManager.OemManager.EntryPoints.Remove(item);
                }
            }
        }
        private void m_entryPointList_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this.DeleteSelectedEntryPoints();
            }
        }
        private void m_entryPointList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private void m_entryPointsExpander_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.None;
            e.Handled = true;
        }
        private void NewStripCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.m_startMenu.StartMenuManager.CustomStripCount < this.m_startMenu.StartMenuManager.MaxCustomStripCount);
            e.Handled = true;
        }
        private void NewStripExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.m_startMenu.StartMenuManager.AddCustomStrip();
            e.Handled = true;
        }
        private void NewApplicationCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void NewApplicationExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OemManager oemManager = this.m_startMenu.StartMenuManager.OemManager;
            Advent.MediaCenter.StartMenu.OEM.EntryPoint ep = new Advent.MediaCenter.StartMenu.OEM.EntryPoint();
            ep.Manager = oemManager;
            ep.ID = "{" + Guid.NewGuid() + "}";
            ep.Title = "New Entry Point";
            ep.RawImageUrl = string.Empty;
            ep.AddIn = typeof(VmcExecuteAddIn).AssemblyQualifiedName;
            bool isSaved = false;
            OemCategory category = oemManager.Categories["More Programs"];
            ep.Saving += delegate
            {
                if (!isSaved)
                {
                    Advent.MediaCenter.StartMenu.OEM.Application application = new Advent.MediaCenter.StartMenu.OEM.Application();
                    application.ID = "{" + Guid.NewGuid() + "}";
                    application.Title = ep.Title;
                    oemManager.Applications.Add(application);
                    ep.Application = application;
                    oemManager.EntryPoints.Add(ep);
                    OemQuickLink oemQuickLink = new OemQuickLink(oemManager);
                    oemQuickLink.BeginInit();
                    oemQuickLink.Application = application;
                    oemQuickLink.EntryPoint = ep;
                    oemQuickLink.EndInit();
                    category.QuickLinks.Add(oemQuickLink);
                }
            };
            ep.Saved += delegate
            {
                if (!isSaved)
                {
                    category.Save();
                }
                isSaved = true;
            };
            EntryPointDocumentView.OpenDocument(ep);
            e.Handled = true;
        }
        private void NewGameCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void NewGameExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OemManager oemManager = this.m_startMenu.StartMenuManager.OemManager;
            Game game = (Game)e.Parameter;
            Advent.MediaCenter.StartMenu.OEM.EntryPoint entryPoint = new Advent.MediaCenter.StartMenu.OEM.EntryPoint();
            entryPoint.ID = "{" + Guid.NewGuid() + "}";
            entryPoint.Title = game.Name;
            entryPoint.RawDescription = game.Description;
            entryPoint.CapabilitiesRequired = (EntryPointCapabilities.DirectX | EntryPointCapabilities.IntensiveRendering);
            entryPoint.NowPlayingDirective = "stop";
            entryPoint.ImageOverride = game.Image;
            string fileName="";
            string arguments="";
            if (VmcStudioUtil.IsShortcut(game.PlayTasks[0]))
            {
                WshShell shell = new WshShell();
                IWshShortcut wshShortcut = (IWshShortcut)shell.CreateShortcut(game.PlayTasks[0]);
                fileName = wshShortcut.TargetPath;
                arguments = wshShortcut.Arguments;
            }
            else
            {
                fileName = game.PlayTasks[0];
                arguments = null;
            }
            entryPoint.AddIn = typeof(VmcExecuteAddIn).AssemblyQualifiedName;
            ExecutionInfo executionInfo = new ExecutionInfo();
            executionInfo.FileName = fileName;
            executionInfo.Arguments = arguments;
            executionInfo.CloseKeys = new List<Keys>
			{
				Keys.BrowserBack
			};
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExecutionInfo));
            StringBuilder stringBuilder = new StringBuilder();
            xmlSerializer.Serialize(new StringWriter(stringBuilder), executionInfo);
            entryPoint.Context = stringBuilder.ToString();
            Advent.MediaCenter.StartMenu.OEM.Application application = oemManager.Applications[game.GameID];
            if (application == null)
            {
                application = new Advent.MediaCenter.StartMenu.OEM.Application();
                application.ID = "{" + Guid.NewGuid() + "}";
                application.Title = entryPoint.Title;
                oemManager.Applications.Add(application);
            }
            entryPoint.Manager = oemManager;
            entryPoint.Application = application;
            oemManager.EntryPoints.Add(entryPoint);
            OemQuickLink oemQuickLink = new OemQuickLink(oemManager);
            oemQuickLink.BeginInit();
            oemQuickLink.Application = application;
            oemQuickLink.EntryPoint = entryPoint;
            oemQuickLink.EndInit();
            oemManager.Categories["Services\\Games"].QuickLinks.Add(oemQuickLink);
            EntryPointDocumentView.OpenDocument(entryPoint);
            e.Handled = true;
        }
        private void DeleteEntryPointCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.m_entryPointList.SelectedItems.Count > 0 && this.m_entryPointsExpander.IsExpanded);
            e.Handled = true;
        }
        private void DeleteEntryPointExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.DeleteSelectedEntryPoints();
            e.Handled = true;
        }
        private void EditEntryPointCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (this.m_entryPointList.SelectedItems.Count == 1 && this.m_entryPointsExpander.IsExpanded);
            e.Handled = true;
        }
        private void EditEntryPointExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Advent.MediaCenter.StartMenu.OEM.EntryPoint entryPoint = this.m_entryPointList.SelectedItem as Advent.MediaCenter.StartMenu.OEM.EntryPoint;
            if (entryPoint != null)
            {
                EntryPointDocumentView.OpenDocument(entryPoint);
            }
        }
        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.m_startMenu.StartMenuManager.IsDirty;
            e.Handled = true;
        }
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            try
            {
                VmcStudioUtil.Application.CommonResources.CloseResources();
                try
                {
                    using (MediaCenterLibraryCache mediaCenterLibraryCache = new MediaCenterLibraryCache(MediaCenterUtil.MediaCenterPath, UnmanagedLibraryAccess.Write))
                    {
                        MemoryLibraryCache memoryLibraryCache = new MemoryLibraryCache(1033);
                        VmcStudioUtil.Application.StartMenuManager.Save(memoryLibraryCache);
                        foreach (string current in memoryLibraryCache.Libraries)
                        {
                            mediaCenterLibraryCache.LoadLibrary(current);
                        }
                        VmcStudioUtil.PrepareForSave(mediaCenterLibraryCache);
                        memoryLibraryCache.ApplyTo(mediaCenterLibraryCache);
                    }
                }
                finally
                {
                    VmcStudioUtil.Application.CommonResources.ResetResources();
                }
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode != 5)
                {
                    throw;
                }
                Trace.TraceError("Failed to save changes: {0}", new object[]
				{
					ex.ToString()
				});
                System.Windows.MessageBox.Show(AccessDeniedMessage, VmcStudioUtil.ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            catch (UnauthorizedAccessException ex2)
            {
                Trace.TraceError("Failed to save changes: {0}", new object[]
				{
					ex2.ToString()
				});
                System.Windows.MessageBox.Show(AccessDeniedMessage, VmcStudioUtil.ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            catch (VmcStudioException ex3)
            {
                Trace.TraceError("Failed to save changes: {0}", new object[]
				{
					ex3.ToString()
				});
                System.Windows.MessageBox.Show("Failed to save changes:\n" + ex3.ToString(), VmcStudioUtil.ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
