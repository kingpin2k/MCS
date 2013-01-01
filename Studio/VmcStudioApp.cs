using Advent.Common;
using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Advent.VmcStudio.Theme.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Advent.VmcStudio
{
    internal class VmcStudioApp : DependencyObject
    {
        public static readonly DependencyProperty SelectedDocumentProperty = DependencyProperty.Register("SelectedDocument", typeof(VmcDocument), typeof(VmcStudioApp));
        protected static readonly DependencyPropertyKey IsExclusiveOperationInProgressPropertyKey = DependencyProperty.RegisterReadOnly("IsExclusiveOperationInProgress", typeof(bool), typeof(VmcStudioApp), new PropertyMetadata((object)false));
        public static readonly DependencyProperty IsExclusiveOperationInProgressProperty = VmcStudioApp.IsExclusiveOperationInProgressPropertyKey.DependencyProperty;
        protected static readonly DependencyPropertyKey ExclusiveOperationProgressPropertyKey = DependencyProperty.RegisterReadOnly("ExclusiveOperationProgress", typeof(double), typeof(VmcStudioApp), new PropertyMetadata((object)0.0));
        public static readonly DependencyProperty ExclusiveOperationProgressProperty = VmcStudioApp.ExclusiveOperationProgressPropertyKey.DependencyProperty;
        protected static readonly DependencyPropertyKey ExclusiveOperationMessagePropertyKey = DependencyProperty.RegisterReadOnly("ExclusiveOperationMessage", typeof(string), typeof(VmcStudioApp), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty ExclusiveOperationMessageProperty = VmcStudioApp.ExclusiveOperationMessagePropertyKey.DependencyProperty;
        protected static readonly DependencyPropertyKey ExclusiveOperationProgressMessagePropertyKey = DependencyProperty.RegisterReadOnly("ExclusiveOperationProgressMessage", typeof(string), typeof(VmcStudioApp), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty ExclusiveOperationProgressMessageProperty = VmcStudioApp.ExclusiveOperationProgressMessagePropertyKey.DependencyProperty;
        private IProgressEnabledOperation exclusiveOperation;

        public string ApplicationTitle
        {
            get
            {
                return VmcStudioUtil.ApplicationTitle;
            }
        }

        public ThemeManager ThemeManager { get; private set; }

        public StartMenuManager StartMenuManager { get; private set; }

        public CommonResourceManager CommonResources { get; private set; }

        public ObservableCollection<VmcDocument> Documents { get; private set; }

        public VmcDocument SelectedDocument
        {
            get
            {
                return (VmcDocument)this.GetValue(VmcStudioApp.SelectedDocumentProperty);
            }
            set
            {
                this.SetValue(VmcStudioApp.SelectedDocumentProperty, (object)value);
            }
        }

        public bool IsExclusiveOperationInProgress
        {
            get
            {
                return (bool)this.GetValue(VmcStudioApp.IsExclusiveOperationInProgressProperty);
            }
            protected set
            {
                this.SetValue(VmcStudioApp.IsExclusiveOperationInProgressPropertyKey, value);
            }
        }

        public IProgressEnabledOperation ExclusiveOperation
        {
            get
            {
                return this.exclusiveOperation;
            }
            set
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    this.exclusiveOperation = value;
                    this.exclusiveOperation.Progress += new EventHandler<ProgressEventArgs>(this.ExclusiveOperation_Progress);
                    this.exclusiveOperation.Completed += new EventHandler<EventArgs>(this.ExclusiveOperation_Completed);
                    if (this.exclusiveOperation.IsCompleted)
                        return;
                    this.ExclusiveOperationMessage = this.exclusiveOperation.Description;
                    this.ExclusiveOperationProgress = 0.0;
                    this.ExclusiveOperationProgressMessage = string.Empty;
                    this.IsExclusiveOperationInProgress = true;
                }));
            }
        }

        public double ExclusiveOperationProgress
        {
            get
            {
                return (double)this.GetValue(VmcStudioApp.ExclusiveOperationProgressProperty);
            }
            protected set
            {
                this.SetValue(VmcStudioApp.ExclusiveOperationProgressPropertyKey, (object)value);
            }
        }

        public string ExclusiveOperationMessage
        {
            get
            {
                return (string)this.GetValue(VmcStudioApp.ExclusiveOperationMessageProperty);
            }
            protected set
            {
                this.SetValue(VmcStudioApp.ExclusiveOperationMessagePropertyKey, (object)value);
            }
        }

        public string ExclusiveOperationProgressMessage
        {
            get
            {
                return (string)this.GetValue(VmcStudioApp.ExclusiveOperationProgressMessageProperty);
            }
            protected set
            {
                this.SetValue(VmcStudioApp.ExclusiveOperationProgressMessagePropertyKey, (object)value);
            }
        }

        static VmcStudioApp()
        {
        }

        public VmcStudioApp()
        {
            this.Documents = new ObservableCollection<VmcDocument>();
            this.Documents.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Documents_CollectionChanged);
            this.ThemeManager = new ThemeManager(Path.Combine(VmcStudioUtil.ApplicationDataPath, "Themes"));
            using (MediaCenterLibraryCache centerLibraryCache = new MediaCenterLibraryCache(MediaCenterUtil.MediaCenterPath))
                this.StartMenuManager = StartMenuManager.Create((IResourceLibraryCache)centerLibraryCache);
            Application.Current.Resources.Add((object)"StartMenuManager", (object)this.StartMenuManager);
            this.StartMenuManager.CustomCategory = VmcStudioUtil.ApplicationName;
            this.CommonResources = new CommonResourceManager(this);
        }

        private void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null)
                return;
            foreach (VmcDocument vmcDocument in (IEnumerable)e.OldItems)
                vmcDocument.Dispose();
        }

        private void ExclusiveOperation_Completed(object sender, EventArgs e)
        {
            //Todo Was Delegate, Now Action
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => this.IsExclusiveOperationInProgress = false));
        }

        private void ExclusiveOperation_Progress(object sender, ProgressEventArgs e)
        {
            //Todo Was Delegate, Now Action
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                IProgressEnabledOperation local_0 = (IProgressEnabledOperation)sender;
                this.ExclusiveOperationProgress = (double)e.CurrentIndex / (double)local_0.Count;
                this.ExclusiveOperationMessage = local_0.Description;
                this.ExclusiveOperationProgressMessage = e.Message;
            }));
        }
    }
}
