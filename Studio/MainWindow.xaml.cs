using Advent.Common.GAC;
using Advent.MediaCenter;
using Advent.VmcStudio.StartMenu;
using Advent.VmcStudio.Theme.Model;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Advent.VmcStudio
{
    public partial class MainWindow : RibbonWindow, IComponentConnector, IStyleConnector
    {
        private ObservableCollection<FrameworkElement> documents;
        
        public IEnumerable<FrameworkElement> Documents
        {
            get
            {
                return this.documents;
            }
        }
        public static MainWindow Instance
        {
            get;
            private set;
        }
        public MainWindow()
        {
            try
            {
                VmcExecuteInstaller vmcExecuteInstaller = new VmcExecuteInstaller(VmcStudioUtil.Application);
                vmcExecuteInstaller.Install();
                MediaCenterUnmanagedLibrary.MediaCenterLibraryUpdated += new EventHandler<MediaCenterLibraryUpdatedEventArgs>(this.MediaCenterLibraryUpdated);
                this.documents = new ObservableCollection<FrameworkElement>();
                this.InitializeComponent();
                VmcStudioApp app = VmcStudioUtil.Application;
                base.DataContext = app;
                DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(VmcStudioApp.SelectedDocumentProperty, typeof(VmcStudioApp));
                dependencyPropertyDescriptor.AddValueChanged(app, delegate
                {
                    if (app.SelectedDocument != null)
                    {
                        FrameworkElement selectedItem = this.documents.First((FrameworkElement o) => o.DataContext == app.SelectedDocument);
                        this.tabs.SelectedItem = selectedItem;
                    }
                    else
                    {
                        this.tabs.SelectedItem = null;
                    }
                    this.gamesDropDown.IsEnabled = (app.SelectedDocument is StartMenuDocument);
                });
                app.Documents.CollectionChanged += new NotifyCollectionChangedEventHandler(this.DocumentsCollectionChanged);
                app.Documents.Add(new ThemeSelectionDocument(app));
                app.Documents.Add(new StartMenuDocument(app));
                app.SelectedDocument = app.Documents[0];
                MainWindow.Instance = this;
            }
            catch (Exception ex)
            {
                String why = ex.Message;
            }
        }
        private void MediaCenterLibraryUpdated(object sender, MediaCenterLibraryUpdatedEventArgs e)
        {
            foreach (string current in AssemblyCache.Global.SearchAssemblies(System.IO.Path.GetFileNameWithoutExtension(e.File)))
            {
                Trace.TraceInformation("Attempting to uninstall assembly " + current + " from global cache.");
                try
                {
                    foreach (AssemblyReference current2 in AssemblyCache.Global.References[current])
                    {
                        AssemblyCache.Global.UninstallAssembly(current2);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }
        internal FrameworkElement ElementFromDocument(VmcDocument document)
        {
            FrameworkElement frameworkElement = this.documents.First((FrameworkElement o) => o.DataContext == document);
            if (frameworkElement != null && VisualTreeHelper.GetChildrenCount(frameworkElement) > 0)
            {
                return (FrameworkElement)VisualTreeHelper.GetChild(frameworkElement, 0);
            }
            return null;
        }
        private void DocumentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                IEnumerator enumerator = e.OldItems.GetEnumerator();
                try
                {
                    VmcDocument document;
                    while (enumerator.MoveNext())
                    {
                        document = (VmcDocument)enumerator.Current;
                        this.documents.Remove(this.documents.First((FrameworkElement o) => o.DataContext == document));
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (VmcDocument vmcDocument in e.NewItems)
                {
                    this.documents.Add(new ContentPresenter
                    {
                        DataContext = vmcDocument,
                        Content = vmcDocument
                    });
                }
            }
        }
        private bool QueryClose(VmcDocument document)
        {
            if (document.IsDirty)
            {
                FrameworkElement target = this.ElementFromDocument(document);
                if (ApplicationCommands.Save.CanExecute(null, target))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show(this, string.Format("Do you want to save changes to {0}?", document.Name), VmcStudioUtil.ApplicationTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        ApplicationCommands.Save.Execute(null, target);
                    }
                    else
                    {
                        if (messageBoxResult == MessageBoxResult.Cancel)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            foreach (VmcDocument current in VmcStudioUtil.Application.Documents.ToList<VmcDocument>())
            {
                if (e.Cancel)
                {
                    break;
                }
                e.Cancel = this.QueryClose(current);
            }
        }
        private void RestoreDefaultCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void RestoreDefaultExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ThemeManager themeManager = VmcStudioUtil.Application.ThemeManager;
            themeManager.AppliedThemes.Clear();
            VmcStudioUtil.RestoreModifiedFiles();
            VmcStudioUtil.Application.StartMenuManager.Reset();
            e.Handled = true;
        }
        private void CreateSupportPackageExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string arg = VmcStudioUtil.CreateSupportPackage();
            Process.Start("explorer", string.Format("/select,\"{0}\"", arg));
        }
        private void ThemesExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            VmcStudioApp application = VmcStudioUtil.Application;
            VmcDocument vmcDocument = application.Documents.FirstOrDefault((VmcDocument o) => o is ThemeSelectionDocument);
            if (vmcDocument == null)
            {
                vmcDocument = new ThemeSelectionDocument(application);
                application.Documents.Add(vmcDocument);
            }
            application.SelectedDocument = vmcDocument;
            e.Handled = true;
        }
        private void StartMenuExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            VmcStudioApp application = VmcStudioUtil.Application;
            VmcDocument vmcDocument = application.Documents.FirstOrDefault((VmcDocument o) => o is StartMenuDocument);
            if (vmcDocument == null)
            {
                vmcDocument = new StartMenuDocument(application);
                application.Documents.Add(vmcDocument);
            }
            application.SelectedDocument = vmcDocument;
            e.Handled = true;
        }
        private void TabItemCloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            FrameworkElement frameworkElement2 = (FrameworkElement)frameworkElement.DataContext;
            VmcDocument vmcDocument = (VmcDocument)frameworkElement2.DataContext;
            if (!this.QueryClose(vmcDocument))
            {
                VmcStudioUtil.Application.Documents.Remove(vmcDocument);
            }
        }
        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)this.tabs.SelectedItem;
            if (frameworkElement != null)
            {
                VmcStudioUtil.Application.SelectedDocument = (VmcDocument)frameworkElement.DataContext;
                return;
            }
            VmcStudioUtil.Application.SelectedDocument = null;
        }
        private void StartMediaCenter(object sender, ExecutedRoutedEventArgs e)
        {
            ThreadStart start = delegate
            {
                Process process = MediaCenterUtil.LaunchMediaCenter(new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true
                }, false, false, false);
                process.ErrorDataReceived += delegate(object s, DataReceivedEventArgs args)
                {
                    Trace.TraceError("Media Center Error: " + args.Data);
                };
                process.BeginErrorReadLine();
                process.WaitForExit();
            };
            new Thread(start)
            {
                IsBackground = true
            }.Start();
        }
        private void GameMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            if (this.gamesDropDown.Command.CanExecute(frameworkElement.DataContext))
            {
                this.gamesDropDown.Command.Execute(frameworkElement.DataContext);
            }
        }
        
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 8:
                    {
                        EventSetter eventSetter = new EventSetter();
                        eventSetter.Event = MenuItem.ClickEvent;
                        eventSetter.Handler = new RoutedEventHandler(this.GameMouseLeftButtonDown);
                        ((Style)target).Setters.Add(eventSetter);
                        return;
                    }
                case 9:
                    ((System.Windows.Shapes.Path)target).MouseLeftButtonDown += new MouseButtonEventHandler(this.TabItemCloseButton_MouseLeftButtonDown);
                    return;
                default:
                    return;
            }
        }
    }
}
