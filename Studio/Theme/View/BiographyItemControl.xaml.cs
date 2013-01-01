using Advent.Common;
using Advent.Common.UI;
using Advent.MediaCenter;
using Advent.VmcStudio;
using Advent.VmcStudio.Theme.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Advent.VmcStudio.Theme.View
{
    public partial class BiographyItemControl : UserControl, IComponentConnector
    {
        // Fields
        //private bool _contentLoaded;
        //internal ListBox imagesList;
        
        protected static readonly DependencyPropertyKey IsPreviewingPropertyKey = DependencyProperty.RegisterReadOnly("IsPreviewing", typeof(bool), typeof(BiographyItemControl), new PropertyMetadata(false));
        public static readonly DependencyProperty IsPreviewingProperty = IsPreviewingPropertyKey.DependencyProperty;
        private bool isStartingPreview;
        private bool isStoppingPreview;
        //internal Border mainScreenshotBorder;
        private Process mediaCenterProcess;
        private WindowPreview preview;
        //internal Grid previewWindow;
        private List<ThemeSummary> previousThemes;
        private ThemeManager themeManager;
        //internal BiographyItemControl @this;

        // Methods
        public BiographyItemControl()
        {
            this.InitializeComponent();
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            this.ThemeItem.Screenshots.Remove(this.SelectedImage);
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.StopScreenshotProcess();
        }

        private void MediaCenterProcess_Exited(object sender, EventArgs e)
        {
            base.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.StopScreenshotProcess));
        }

        private void SetImageAsMain_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource mainScreenshot = this.ThemeItem.MainScreenshot;
            this.ThemeItem.MainScreenshot = this.SelectedImage;
            this.ThemeItem.Screenshots.Remove(this.SelectedImage);
            if (mainScreenshot != null)
            {
                this.ThemeItem.Screenshots.Add(mainScreenshot);
            }
        }

        private void StartScreenshot_Click(object sender, RoutedEventArgs e)
        {
            this.StartScreenshotProcess();
        }

        private void StartScreenshotProcess()
        {
            if (!this.IsPreviewing && !this.isStartingPreview)
            {
                this.isStartingPreview = true;
                this.themeManager = VmcStudioUtil.Application.ThemeManager;
                this.previousThemes = new List<ThemeSummary>(this.themeManager.AppliedThemes);
                this.themeManager.AppliedThemes.Clear();
                this.themeManager.AppliedThemes.Add(Enumerable.First<ThemeSummary>(this.themeManager.Themes, (Func<ThemeSummary, bool>)(o => (this.ThemeItem.Theme.ID == o.ID))));
                IProgressEnabledOperation operation = this.themeManager.ApplyThemesAsync(true, true);
                operation.Completed += delegate
                {
                    Action action2 = null;
                    this.mediaCenterProcess = MediaCenterUtil.LaunchMediaCenter(false, true, true);
                    this.mediaCenterProcess.EnableRaisingEvents = true;
                    this.mediaCenterProcess.Exited += new EventHandler(this.MediaCenterProcess_Exited);
                    base.Dispatcher.ShutdownStarted += new EventHandler(this.Dispatcher_ShutdownStarted);
                    IntPtr windowHandle = IntPtr.Zero;
                    for (int j = 0; (windowHandle == IntPtr.Zero) && (j < 20); j++)
                    {
                        Thread.Sleep(500);
                        windowHandle = MediaCenterUtil.GetMediaCenterWindow();
                    }
                    if (windowHandle != IntPtr.Zero)
                    {
                        if (action2 == null)
                        {
                            action2 = delegate
                            {
                                this.preview = new WindowPreview(this.previewWindow, windowHandle);
                                this.preview.ClientAreaOnly = true;
                                this.preview.IsVisible = true;
                                this.IsPreviewing = true;
                                this.isStartingPreview = false;
                            };
                        }
                        Action method = action2;
                        base.Dispatcher.Invoke(DispatcherPriority.Normal, method);
                    }
                };
                VmcStudioUtil.Application.ExclusiveOperation = operation;
            }
        }

        private void StopScreenshot_Click(object sender, RoutedEventArgs e)
        {
            this.StopScreenshotProcess();
        }

        private void StopScreenshotProcess()
        {
            if (this.IsPreviewing && !this.isStoppingPreview)
            {
                this.isStoppingPreview = true;
                this.mediaCenterProcess.Exited -= new EventHandler(this.MediaCenterProcess_Exited);
                base.Dispatcher.ShutdownStarted -= new EventHandler(this.Dispatcher_ShutdownStarted);
                this.themeManager.AppliedThemes.Clear();
                foreach (ThemeSummary summary in this.previousThemes)
                {
                    this.themeManager.AppliedThemes.Add(summary);
                }
                IProgressEnabledOperation operation = this.themeManager.ApplyThemesAsync(false, true);
                operation.Completed += delegate
                {
                    Action method = delegate
                    {
                        this.IsPreviewing = false;
                        this.isStoppingPreview = false;
                        this.preview.Dispose();
                        this.preview = null;
                        this.previousThemes = null;
                        this.mediaCenterProcess = null;
                        this.themeManager = null;
                    };
                    base.Dispatcher.Invoke(DispatcherPriority.Normal, method);
                };
                VmcStudioUtil.Application.ExclusiveOperation = operation;
            }
        }

        private void TakeScreenshot()
        {
            if (!this.IsPreviewing)
            {
                throw new InvalidOperationException("Screenshot operation not in progress.");
            }
            BitmapSource item = this.preview.TakeScreenshot(true);
            this.ThemeItem.Screenshots.Add(item);
        }

        private void TakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            this.TakeScreenshot();
        }

        // Properties
        public bool IsPreviewing
        {
            get
            {
                return (bool)base.GetValue(IsPreviewingProperty);
            }
            protected set
            {
                base.SetValue(IsPreviewingPropertyKey, value);
            }
        }

        private BitmapSource SelectedImage
        {
            get
            {
                return (this.imagesList.SelectedItem as BitmapSource);
            }
        }

        internal BiographyModel.BiographyThemeItem ThemeItem
        {
            get
            {
                return (BiographyModel.BiographyThemeItem)((BiographyModel)base.DataContext).ThemeItem;
            }
        }
    }


}
