using Advent.Common;
using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.Common.UI;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using Advent.MediaCenter.Theme;
using Advent.VmcStudio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Advent.VmcStudio.Theme.Model
{
    public class ThemeManager : NotifyPropertyChangedBase
    {
        private const string AppliedThemesDocumentName = "VmcStudio.Themes.xml";
        private readonly string themesPath;
        private readonly ObservableCollection<ThemeSummary> themes;
        private MediaCenterLibraryCache backupCache;
        private List<IResource> defaultResources;
        private bool isDirty;
        private EventHandler<ApplyThemesEventArgs> applyingThemes;

        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            set
            {
                this.isDirty = value;
                this.OnPropertyChanged("IsDirty");
            }
        }

        public ObservableCollection<ThemeSummary> AppliedThemes { get; private set; }

        public IEnumerable<ThemeSummary> Themes { get; private set; }

        public MediaCenterLibraryCache BackupCache
        {
            get
            {
                if (this.backupCache == null)
                {
                    this.backupCache = new MediaCenterLibraryCache(VmcStudioUtil.BackupsPath, UnmanagedLibraryAccess.Read);
                    this.backupCache.LibraryLoading += (EventHandler<LibraryLoadEventArgs>)((sender, e) => VmcStudioUtil.BackupFile(Path.Combine(MediaCenterUtil.MediaCenterPath, e.FileName)));
                }
                return this.backupCache;
            }
        }

        public IEnumerable<IResource> DefaultResources
        {
            get
            {
                if (this.defaultResources == null)
                {
                    this.defaultResources = new List<IResource>();
                    string[] strArray = new string[2]
          {
            "ehres.dll",
            "Microsoft.MediaCenter.Shell.dll"
          };
                    foreach (string index in strArray)
                    {
                        foreach (IResource resource in this.BackupCache[index][(object)10])
                            this.defaultResources.Add(resource);
                    }
                }
                return (IEnumerable<IResource>)this.defaultResources;
            }
        }

        public event EventHandler<ApplyThemesEventArgs> ApplyingThemes
        {
            add
            {
                EventHandler<ApplyThemesEventArgs> eventHandler = this.applyingThemes;
                EventHandler<ApplyThemesEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<ApplyThemesEventArgs>>(ref this.applyingThemes, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<ApplyThemesEventArgs> eventHandler = this.applyingThemes;
                EventHandler<ApplyThemesEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<ApplyThemesEventArgs>>(ref this.applyingThemes, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public ThemeManager(string themesPath)
        {
            if (themesPath == null)
                throw new ArgumentNullException();
            this.themesPath = themesPath;
            this.AppliedThemes = new ObservableCollection<ThemeSummary>();
            this.themes = new ObservableCollection<ThemeSummary>();
            this.Themes = (IEnumerable<ThemeSummary>)this.themes;
            try
            {
                foreach (string path in Directory.GetDirectories(themesPath))
                {
                    try
                    {
                        var theme_summary = ThemeSummary.Load(path);
                        if(theme_summary!=null)
                            this.themes.Add(theme_summary);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
            using (MediaCenterLibraryCache centerLibraryCache = new MediaCenterLibraryCache(MediaCenterUtil.MediaCenterPath))
            {
                foreach (ThemeSummary themeSummary1 in ThemeManager.GetAppliedThemes((IResourceLibraryCache)centerLibraryCache))
                {
                    ThemeSummary summary = themeSummary1;
                    ThemeSummary themeSummary2 = Enumerable.FirstOrDefault<ThemeSummary>(this.Themes, (Func<ThemeSummary, bool>)(o => o.ID == summary.ID));
                    if (themeSummary2 != null)
                        this.AppliedThemes.Add(themeSummary2);
                }
            }
            this.AppliedThemes.CollectionChanged += (NotifyCollectionChangedEventHandler)delegate
            {
                this.IsDirty = true;
            };
        }

        public void DeleteTheme(ThemeSummary theme)
        {
            this.themes.Remove(theme);
            Directory.Delete(theme.BasePath, true);
        }

        public ThemeSummary CreateTheme()
        {
            string str = Guid.NewGuid().ToString();
            string file = Path.Combine(Path.Combine(this.themesPath, str), str + ".mct");
            using (VmcStudioTheme vmcStudioTheme = new VmcStudioTheme(file, str))
            {
                vmcStudioTheme.Name = "New theme (" + (object)DateTime.Now + ")";
                vmcStudioTheme.Author = Environment.UserName;
                vmcStudioTheme.Save();
            }
            return this.ImportTheme(file);
        }

        public IProgressEnabledOperation ImportThemes(IEnumerable<string> files)
        {
            ProgressEnabledOperation operation = new ProgressEnabledOperation("Importing themes...");
            List<string> fileList = Enumerable.ToList<string>(files);
            operation.Count = fileList.Count;
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                try
                {
                    for (int currentIndex = 0; currentIndex < fileList.Count; ++currentIndex)
                    {
                        string str = fileList[currentIndex];
                        operation.OnProgress(string.Format("Importing {0}...", (object)Path.GetFileName(str)), currentIndex);
                        this.ImportTheme(str);
                    }
                    operation.OnCompleted();
                }
                catch (Exception ex)
                {
                    operation.OnAbandoned(ex);
                    throw;
                }
            });
            return (IProgressEnabledOperation)operation;
        }

        public ThemeSummary ImportTheme(string file)
        {
            if (Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                ThemeSummary theme = this.CreateTheme();
                using (MediaCenterTheme mediaCenterTheme = theme.OpenTheme())
                {
                    mediaCenterTheme.AddResourceFileDifferences(VmcStudioUtil.BackupFile(Path.Combine(MediaCenterUtil.MediaCenterPath, "ehres.dll")), file);
                    mediaCenterTheme.Save();
                }
                return theme;
            }
            else
            {
                string directory = Path.Combine(this.themesPath, Guid.NewGuid().ToString());
                string themeFileName = Path.GetFileName(file);
                string str = Path.Combine(directory, themeFileName);
                Directory.CreateDirectory(directory);
                try
                {
                    File.Copy(file, str);
                    try
                    {
                        Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
                        ThemeSummary summary = new ThemeSummary();
                        Action action = (Action)(() =>
                        {
                            summary.BasePath = directory;
                            summary.ThemeFile = themeFileName;
                            summary.Save();
                            this.themes.Add(summary);
                        });
                        dispatcher.Invoke((Delegate)action, new object[0]);
                        return summary;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Error importing theme {0}: {1}", (object)themeFileName, (object)((object)ex).ToString());
                        File.Delete(str);
                        throw;
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch (Exception ex2)
                    {
                        Trace.TraceError("Could not delete failed import ({1}): {0}", (object)((object)ex2).ToString(), (object)directory);
                    }
                    throw;
                }
            }
        }

        public IProgressEnabledOperation ApplyThemesAsync(bool verifyBeforeClose, bool preserveMenu)
        {
            VmcStudioUtil.EnsureMediaCenterClosed(verifyBeforeClose);
            ProgressEnabledOperation enabledOperation = new ProgressEnabledOperation()
            {
                Description = "Preparing..."
            };
            ThreadPool.QueueUserWorkItem((WaitCallback)(state => this.ApplyThemes((ProgressEnabledOperation)state, preserveMenu)), (object)enabledOperation);
            return (IProgressEnabledOperation)enabledOperation;
        }

        public void ApplyThemes()
        {
            this.ApplyThemes(true, true);
        }

        public void ApplyThemes(bool verifyBeforeClose, bool preserveMenu)
        {
            VmcStudioUtil.EnsureMediaCenterClosed(verifyBeforeClose);
            this.ApplyThemes(new ProgressEnabledOperation(), preserveMenu);
        }

        protected virtual void OnApplyingThemes(ApplyThemesEventArgs args)
        {
            EventHandler<ApplyThemesEventArgs> eventHandler = this.applyingThemes;
            if (eventHandler == null)
                return;
            eventHandler((object)this, args);
        }

        protected void ApplyThemes(ProgressEnabledOperation operation, bool preserveMenu)
        {
            this.OnApplyingThemes(new ApplyThemesEventArgs(operation));
            try
            {
                MediaCenterLibraryCache readCache = new MediaCenterLibraryCache(VmcStudioUtil.BackupsPath, UnmanagedLibraryAccess.Read);
                MediaCenterLibraryCache writeCache = new MediaCenterLibraryCache(MediaCenterUtil.MediaCenterPath, UnmanagedLibraryAccess.Write);
                MemoryLibraryCache memoryLibraryCache = (MemoryLibraryCache)null;
                if (preserveMenu)
                {
                    memoryLibraryCache = new MemoryLibraryCache((ushort)1033);
                    using (MediaCenterLibraryCache centerLibraryCache = new MediaCenterLibraryCache())
                        StartMenuManager.Create((IResourceLibraryCache)centerLibraryCache).Save((IResourceLibraryCache)memoryLibraryCache, true);
                }
                readCache.LibraryLoading += (EventHandler<LibraryLoadEventArgs>)((sender, e) => VmcStudioUtil.BackupFile(Path.Combine(MediaCenterUtil.MediaCenterPath, e.FileName)));
                writeCache.LibraryLoading += (EventHandler<LibraryLoadEventArgs>)((sender, e) =>
                {
                    string local_0 = VmcStudioUtil.BackupFile(Path.Combine(MediaCenterUtil.MediaCenterPath, e.FileName));
                    string local_1 = Path.Combine(writeCache.SearchPath, e.FileName);
                    VmcStudioUtil.TakeOwnership(local_1);
                    File.Copy(local_0, local_1, true);
                    VmcStudioUtil.TakeOwnership(local_1);
                });
                if (Directory.Exists(VmcStudioUtil.BackupsPath))
                {
                    foreach (string path in Directory.GetFiles(VmcStudioUtil.BackupsPath))
                        writeCache.LoadLibrary(Path.GetFileName(path));
                }
                int num = 0;
                int operationCompleted = 0;
                Exception applyException = (Exception)null;
                List<MediaCenterTheme> list1 = Enumerable.ToList<MediaCenterTheme>(Enumerable.Select<ThemeSummary, MediaCenterTheme>((IEnumerable<ThemeSummary>)this.AppliedThemes, (Func<ThemeSummary, MediaCenterTheme>)(o => o.OpenTheme())));
                try
                {
                    List<IDelayedProgressOperation> list2 = new List<IDelayedProgressOperation>();
                    foreach (MediaCenterTheme mediaCenterTheme in list1)
                    {
                        IDelayedProgressOperation applyOperation = mediaCenterTheme.CreateApplyOperation(readCache, writeCache);
                        num += applyOperation.Count;
                        applyOperation.Progress += (EventHandler<ProgressEventArgs>)((sender, args) => operation.OnProgress(args.Message, args.CurrentIndex + operationCompleted));
                        applyOperation.Abandoned += (EventHandler<ExceptionEventArgs>)((sender, args) => applyException = args.Exception);
                        list2.Add(applyOperation);
                    }
                    operation.Count = num;
                    using (readCache)
                    {
                        using (writeCache)
                        {
                            foreach (IDelayedProgressOperation progressOperation in list2)
                            {
                                operation.Description = progressOperation.Description;
                                progressOperation.Start();
                                progressOperation.WaitForCompletion();
                                if (applyException != null)
                                    throw applyException;
                                operationCompleted += progressOperation.Count;
                            }
                            if (this.AppliedThemes.Count > 0)
                            {
                                IResource resource = writeCache["ehres.dll"].GetResource("VmcStudio.Themes.xml", (object)23);
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    new XmlSerializer(typeof(ThemeManager.AppliedThemesDocument)).Serialize((Stream)memoryStream, (object)new ThemeManager.AppliedThemesDocument()
                                    {
                                        AppliedThemes = Enumerable.ToArray<ThemeSummary>((IEnumerable<ThemeSummary>)this.AppliedThemes)
                                    });
                                    memoryStream.Flush();
                                    ResourceExtensions.Update(resource, memoryStream.GetBuffer());
                                }
                            }
                            if (memoryLibraryCache != null)
                                memoryLibraryCache.ApplyTo((IResourceLibraryCache)writeCache);
                        }
                    }
                    this.IsDirty = false;
                }
                finally
                {
                    foreach (MediaCenterTheme mediaCenterTheme in list1)
                        mediaCenterTheme.Dispose();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(((object)ex).ToString());
                operation.OnAbandoned(ex);
                throw;
            }
            operation.OnCompleted();
        }

        private static ThemeSummary[] GetAppliedThemes(IResourceLibraryCache cache)
        {
            IResource resource = cache["ehres.dll"].GetResource("VmcStudio.Themes.xml", (object)23);
            if (!resource.Exists(new ushort?()))
                return new ThemeSummary[0];
            using (MemoryStream memoryStream = new MemoryStream(ResourceExtensions.GetBytes(resource)))
                return ((ThemeManager.AppliedThemesDocument)new XmlSerializer(typeof(ThemeManager.AppliedThemesDocument)).Deserialize((Stream)memoryStream)).AppliedThemes;
        }

        [XmlRoot("VmcStudio")]
        public class AppliedThemesDocument
        {
            public ThemeSummary[] AppliedThemes { get; set; }
        }

        private class EmptyOemManager : OemManager
        {
            protected override void BuildInfo(ICollection<OemMenuStrip> strips)
            {
            }
        }
    }
}
