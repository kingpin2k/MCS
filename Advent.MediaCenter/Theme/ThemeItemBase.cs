


using Advent.Common.UI;
using Advent.MediaCenter;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace Advent.MediaCenter.Theme
{
    public abstract class ThemeItemBase : NotifyPropertyChangedBase, IThemeItem, ISupportInitialize
    {
        private bool isInitialised;
        private bool isLoaded;
        private bool isDirty;
        private IThemeItemApplicator applicator;
        private EventHandler isDirtyChanged;

        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }
            protected internal set
            {
                if (!this.IsInitialised || this.isDirty == value)
                    return;
                this.isDirty = value;
                this.OnIsDirtyChanged(EventArgs.Empty);
            }
        }

        public bool IsLoaded
        {
            get
            {
                return this.isLoaded;
            }
        }

        [XmlIgnore]
        public bool IsInitialised
        {
            get
            {
                return this.isInitialised;
            }
        }

        [XmlIgnore]
        public abstract string Name { get; }

        [XmlIgnore]
        public MediaCenterTheme Theme { get; set; }

        internal IThemeItemApplicator Applicator
        {
            get
            {
                if (this.applicator == null)
                    this.applicator = ThemeItemBase.CreateApplicator<IThemeItemApplicator>(this.GetType());
                return this.applicator;
            }
        }

        public event EventHandler IsDirtyChanged
        {
            add
            {
                EventHandler eventHandler = this.isDirtyChanged;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.isDirtyChanged, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.isDirtyChanged;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.isDirtyChanged, comparand - value, comparand);
                }
                while (eventHandler != comparand);
            }
        }

        public virtual void Apply(MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            this.Applicator.Apply((IThemeItem)this, readCache, writeCache);
        }

        public void Load()
        {
            if (!this.isLoaded)
            {
                this.BeginInit();
                this.LoadInternal();
                this.EndInit();
            }
            this.isLoaded = true;
        }

        public virtual void ClearDirty()
        {
            this.IsDirty = false;
        }

        public void BeginInit()
        {
            this.isInitialised = false;
        }

        public void EndInit()
        {
            this.isInitialised = true;
        }

        internal static U CreateApplicator<T, U>()
            where T : ThemeItemBase
            where U : class, IThemeItemApplicator
        {
            return ThemeItemBase.CreateApplicator<U>(typeof(T));
        }

        protected void OnIsDirtyChanged(EventArgs args)
        {
            if (!this.IsInitialised || this.isDirtyChanged == null)
                return;
            this.isDirtyChanged((object)this, args);
            base.OnPropertyChanged("IsDirty");
        }

        protected abstract void LoadInternal();

        protected override void OnPropertyChanged(string property)
        {
            this.IsDirty = true;
            base.OnPropertyChanged(property);
        }

        private static T CreateApplicator<T>(Type itemType) where T : class, IThemeItemApplicator
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(MediaCenterUtil.MediaCenterPath, "ehshell.exe"));
            Type type = (Type)null;
            foreach (MediaCenterVersionAttribute versionAttribute in itemType.GetCustomAttributes(typeof(MediaCenterVersionAttribute), true))
            {
                if (versionAttribute.AppliesToVersion(versionInfo))
                {
                    type = versionAttribute.ApplicatorType;
                    break;
                }
            }
            if (type == null)
            {
                throw new NotSupportedException("No applicator found for applying " + itemType.Name + " to Media Center version " + versionInfo.ProductVersion + ".");
            }
            else
            {
                object instance = Activator.CreateInstance(type);
                T obj = instance as T;
                if ((object)obj == null)
                    throw new NotSupportedException(string.Format("{0} does not implement {1}.", (object)instance.GetType().FullName, (object)typeof(T).FullName));
                else
                    return obj;
            }
        }
    }
}
