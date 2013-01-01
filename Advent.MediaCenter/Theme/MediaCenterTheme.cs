


using Advent.Common;
using Advent.Common.Interop;
using Advent.Common.IO;
using Advent.Common.UI;
using Advent.MediaCenter;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Advent.MediaCenter.Theme
{
    public enum ThemeType
    {
        Base,
        AddOn,
    }

    public abstract class MediaCenterTheme : NotifyPropertyChangedBase, IDisposable, ISupportInitialize
    {
        private ThemeType themeType;
        private bool isDirty;
        private string name;
        private string author;
        private string comments;
        private BitmapSource mainScreenshot;
        private List<FontFamily> fonts;
        private IEnumerable<string> fontFiles;
        private FontsThemeItem fontsItem;
        private ColorsThemeItem colorsItem;
        private bool isInitialised;
        private EventHandler<CancelEventArgs> saving;
        private EventHandler saved;

        public ThemeType ThemeType
        {
            get
            {
                return this.themeType;
            }
            set
            {
                if (this.themeType == value)
                    return;
                this.themeType = value;
                this.OnPropertyChanged("ThemeType");
            }
        }

        public string File { get; protected set; }

        public ObservableCollection<IThemeItem> ThemeItems { get; private set; }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (!(this.name != value))
                    return;
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public string ID { get; protected set; }

        public string Author
        {
            get
            {
                return this.author;
            }
            set
            {
                if (!(this.author != value))
                    return;
                this.author = value;
                this.OnPropertyChanged("Author");
            }
        }

        public Version Version { get; protected set; }

        public string Comments
        {
            get
            {
                return this.comments;
            }
            set
            {
                if (!(this.comments != value))
                    return;
                this.comments = value;
                this.OnPropertyChanged("Comments");
            }
        }

        public BitmapSource MainScreenshot
        {
            get
            {
                return this.mainScreenshot;
            }
            set
            {
                if (this.mainScreenshot == value)
                    return;
                this.mainScreenshot = value;
                this.OnPropertyChanged("Screenshot");
            }
        }

        public FontsThemeItem FontsItem
        {
            get
            {
                return this.fontsItem;
            }
            protected set
            {
                this.fontsItem = value;
                this.fontsItem.Theme = this;
                this.fontsItem.IsDirtyChanged += new EventHandler(this.ThemeItem_IsDirtyChanged);
            }
        }

        public ColorsThemeItem ColorsItem
        {
            get
            {
                return this.colorsItem;
            }
            set
            {
                this.colorsItem = value;
                this.colorsItem.Theme = this;
                this.colorsItem.IsDirtyChanged += new EventHandler(this.ThemeItem_IsDirtyChanged);
            }
        }

        public ObservableCollection<BitmapSource> Screenshots { get; private set; }

        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            internal set
            {
                if (this.isDirty == value)
                    return;
                this.isDirty = value;
                this.OnPropertyChanged("IsDirty");
            }
        }

        public ICollection<FontFamily> Fonts
        {
            get
            {
                if (this.fonts == null)
                {
                    this.fonts = new List<FontFamily>();
                    foreach (string location in this.FontFiles)
                        this.fonts.AddRange((IEnumerable<FontFamily>)System.Windows.Media.Fonts.GetFontFamilies(location));
                }
                return (ICollection<FontFamily>)this.fonts;
            }
        }

        public abstract bool CanSave { get; }

        protected internal IEnumerable<string> FontFiles
        {
            get
            {
                if (this.fontFiles == null)
                    this.fontFiles = this.LoadFontFiles() ?? (IEnumerable<string>)new List<string>();
                return this.fontFiles;
            }
            protected set
            {
                this.fontFiles = value;
                this.fonts = (List<FontFamily>)null;
            }
        }

        public event EventHandler<CancelEventArgs> Saving
        {
            add
            {
                EventHandler<CancelEventArgs> eventHandler = this.saving;
                EventHandler<CancelEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<CancelEventArgs>>(ref this.saving, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler<CancelEventArgs> eventHandler = this.saving;
                EventHandler<CancelEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler<CancelEventArgs>>(ref this.saving, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public event EventHandler Saved
        {
            add
            {
                EventHandler eventHandler = this.saved;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saved, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.saved;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saved, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        protected MediaCenterTheme()
        {
            this.ThemeItems = new ObservableCollection<IThemeItem>();
            this.ThemeItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ThemeItems_CollectionChanged);
            this.Screenshots = new ObservableCollection<BitmapSource>();
            this.Screenshots.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Screenshots_CollectionChanged);
        }

        ~MediaCenterTheme()
        {
            this.Dispose(false);
        }

        public static bool IsThemeFile(string path)
        {
            switch (Path.GetExtension(path).ToLowerInvariant())
            {
                case ".mct":
                case ".vmcthemepack":
                    return true;
                default:
                    return false;
            }
        }

        public static MediaCenterTheme FromFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException();
            MediaCenterTheme mediaCenterTheme;
            switch (Path.GetExtension(path.ToLowerInvariant()))
            {
                case ".mct":
                    mediaCenterTheme = (MediaCenterTheme)new VmcStudioTheme(path);
                    break;
                case ".vmcthemepack":
                    mediaCenterTheme = (MediaCenterTheme)new MediaCenterFXTheme(path);
                    break;
                default:
                    throw new ArgumentException(string.Format("The file \"{0}\" is not of a known theme type.", (object)path));
            }
            mediaCenterTheme.BeginInit();
            mediaCenterTheme.File = path;
            mediaCenterTheme.IsDirty = false;
            mediaCenterTheme.EndInit();
            return mediaCenterTheme;
        }

        public void AddResourceFileDifferences(string baseResourcesPath, string modifiedResourcesPath)
        {
            using (UnmanagedLibrary unmanagedLibrary1 = new UnmanagedLibrary(baseResourcesPath))
            {
                using (UnmanagedLibrary unmanagedLibrary2 = new UnmanagedLibrary(modifiedResourcesPath))
                {
                    string fileName = Path.GetFileName(baseResourcesPath);
                    foreach (IResource resource in Enumerable.Where<IResource>(unmanagedLibrary2[(object)10], (Func<IResource, bool>)(o => o.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))))
                    {
                        bool flag1 = true;
                        byte[] bytes1 = ResourceExtensions.GetBytes(unmanagedLibrary1.GetResource(resource.Name, (object)10));
                        byte[] bytes2 = ResourceExtensions.GetBytes(resource);
                        if (bytes1 != null && bytes1.Length == bytes2.Length)
                        {
                            bool flag2 = true;
                            for (int index = 0; index < bytes1.Length; ++index)
                            {
                                if ((int)bytes1[index] != (int)bytes2[index])
                                {
                                    flag2 = false;
                                    break;
                                }
                            }
                            flag1 = !flag2;
                        }
                        if (flag1)
                            this.ThemeItems.Add((IThemeItem)new ImageResourceThemeItem(fileName, resource.Name, bytes2));
                    }
                }
            }
        }

        public bool Save()
        {
            if (this.saving != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                this.saving((object)this, e);
                if (e.Cancel)
                    return false;
            }
            this.SaveInternal();
            this.IsDirty = false;
            if (this.saved != null)
                this.saved((object)this, EventArgs.Empty);
            return true;
        }

        public IDelayedProgressOperation CreateApplyOperation(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            return (IDelayedProgressOperation)new MediaCenterTheme.ApplyOperation(this, readCache, writeCache);
        }

        public void Apply(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            this.Apply(readCache, writeCache, new ProgressEnabledOperation());
        }

        public FontFamily GetFontFamily(string fontName)
        {
            return MediaCenterTheme.FindFont(fontName, (IEnumerable<FontFamily>)this.Fonts) ?? MediaCenterTheme.FindFont(fontName, (IEnumerable<FontFamily>)System.Windows.Media.Fonts.SystemFontFamilies);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected override void OnPropertyChanged(string property)
        {
            if (this.isInitialised && property != "IsDirty")
                this.IsDirty = true;
            base.OnPropertyChanged(property);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void Apply(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache, ProgressEnabledOperation operation)
        {
            try
            {
                if (this.Fonts.Count > 0)
                {
                    operation.OnProgress("Installing fonts...", 0);
                    foreach (string file in Enumerable.Distinct<string>(Enumerable.Select<FontFamily, string>(Enumerable.Where<FontFamily>((IEnumerable<FontFamily>)this.Fonts, (Func<FontFamily, bool>)(f => !Enumerable.Any<FontFamily>((IEnumerable<FontFamily>)System.Windows.Media.Fonts.SystemFontFamilies, (Func<FontFamily, bool>)(o => FontUtil.GetName(f) == FontUtil.GetName(o))))), (Func<FontFamily, string>)(o => FontUtil.GetFile(o)))))
                        FontUtil.InstallFonts(file);
                }
                List<IThemeItem> list = Enumerable.ToList<IThemeItem>(Enumerable.Concat<IThemeItem>((IEnumerable<IThemeItem>)new List<IThemeItem>()
        {
          (IThemeItem) this.FontsItem,
          (IThemeItem) this.ColorsItem
        }, (IEnumerable<IThemeItem>)this.ThemeItems));
                for (int currentIndex = 0; currentIndex < list.Count; ++currentIndex)
                {
                    IThemeItem themeItem = list[currentIndex];
                    operation.OnProgress(string.Format("Applying {0}...", (object)themeItem.Name), currentIndex);
                    try
                    {
                        themeItem.Apply(readCache, writeCache);
                    }
                    catch (ThemeApplicationException ex)
                    {
                        Trace.TraceWarning(((object)ex).ToString());
                    }
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

        protected abstract void SaveInternal();

        protected abstract IEnumerable<string> LoadFontFiles();

        private static FontFamily FindFont(string font, IEnumerable<FontFamily> fonts)
        {
            return Enumerable.FirstOrDefault<FontFamily>(Enumerable.Where<FontFamily>(fonts, (Func<FontFamily, bool>)(o => FontUtil.GetName(o) == font)));
        }

        private void ThemeItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (IThemeItem themeItem in (IEnumerable)e.OldItems)
                    themeItem.IsDirtyChanged -= new EventHandler(this.ThemeItem_IsDirtyChanged);
            }
            if (e.NewItems == null)
                return;
            foreach (IThemeItem themeItem in (IEnumerable)e.NewItems)
            {
                themeItem.IsDirtyChanged += new EventHandler(this.ThemeItem_IsDirtyChanged);
                themeItem.Theme = this;
                this.UpdateThemeItemDirty(themeItem);
            }
        }

        private void Screenshots_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        private void ThemeItem_IsDirtyChanged(object sender, EventArgs e)
        {
            this.UpdateThemeItemDirty((IThemeItem)sender);
        }

        private void UpdateThemeItemDirty(IThemeItem item)
        {
            if (this.IsDirty || !item.IsDirty)
                return;
            this.IsDirty = true;
        }

        public void BeginInit()
        {
            this.isInitialised = false;
        }

        public void EndInit()
        {
            this.isInitialised = true;
        }

        private class ApplyOperation : DelayedProgressOperation
        {
            public override int Count
            {
                get
                {
                    return this.Theme.ThemeItems.Count + 2;
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public MediaCenterTheme Theme { get; private set; }

            public MediaCenterLibraryCache WriteCache { get; private set; }

            public MediaCenterLibraryCache ReadCache { get; private set; }

            public ApplyOperation(MediaCenterTheme theme, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
            {
                this.ReadCache = readCache;
                this.WriteCache = writeCache;
                this.Theme = theme;
                this.Description = string.Format("Applying theme {0}", (object)theme.Name);
            }

            public override void Start()
            {
                base.Start();
                ThreadPool.QueueUserWorkItem((WaitCallback)(state =>
                {
                    MediaCenterTheme.ApplyOperation local_0 = (MediaCenterTheme.ApplyOperation)state;
                    local_0.Theme.Apply(local_0.ReadCache, local_0.WriteCache, (ProgressEnabledOperation)local_0);
                }), (object)this);
            }
        }
    }
}
