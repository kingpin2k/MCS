


using Advent.Common.Serialization;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public abstract class MediaCenterRegistryObject : DependencyObject, IRegistryKeySerialisable, ISupportInitialize
    {
        private static readonly DependencyPropertyKey IsDirtyPropertyKey = DependencyProperty.RegisterReadOnly("IsDirty", typeof(bool), typeof(MediaCenterRegistryObject), new PropertyMetadata(new PropertyChangedCallback(MediaCenterRegistryObject.IsDirtyPropertyChanged)));
        public static readonly DependencyProperty IsDirtyProperty = MediaCenterRegistryObject.IsDirtyPropertyKey.DependencyProperty;
        private string regKey;
        private OemManager mcm;
        private bool isInit;
        private EventHandler saved;
        private EventHandler saving;
        private EventHandler isDirtyChanged;

        public string RegKey
        {
            get
            {
                return this.regKey;
            }
            protected set
            {
                this.regKey = value;
            }
        }

        public string RegPath
        {
            get
            {
                if (this.RegKey == null)
                    return (string)null;
                else
                    return this.RegKey.Substring(this.RegKey.IndexOf('\\') + 1);
            }
        }

        public RegistryKey RegHive
        {
            get
            {
                if (!this.IsSaved || this.regKey.StartsWith("HKEY_LOCAL_MACHINE"))
                    return Registry.LocalMachine;
                if (this.regKey.StartsWith("HKEY_CURRENT_USER"))
                    return Registry.CurrentUser;
                Trace.TraceWarning("{0} object has bad registry key location: {1}", (object)this.GetType().Name, (object)this.RegKey);
                return (RegistryKey)null;
            }
        }

        public OemManager Manager
        {
            get
            {
                return this.mcm;
            }
            set
            {
                this.mcm = value;
                if (!this.IsDirty || this.mcm.DirtyObjects.Contains(this))
                    return;
                this.mcm.DirtyObjects.Add(this);
            }
        }

        public bool IsDirty
        {
            get
            {
                return (bool)this.GetValue(MediaCenterRegistryObject.IsDirtyProperty);
            }
            protected set
            {
                this.SetValue(MediaCenterRegistryObject.IsDirtyPropertyKey, value );
            }
        }

        public bool IsSaved
        {
            get
            {
                return this.RegKey != null;
            }
        }

        public bool IsInInit
        {
            get
            {
                return this.isInit;
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

        public event EventHandler Saving
        {
            add
            {
                EventHandler eventHandler = this.saving;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saving, comparand + value, comparand);
                }
                while (eventHandler != comparand);
            }
            remove
            {
                EventHandler eventHandler = this.saving;
                EventHandler comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.saving, comparand - value, comparand);
                }
                while (eventHandler != comparand);
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

        static MediaCenterRegistryObject()
        {
        }

        public void MarkAsDirty()
        {
            this.IsDirty = true;
        }

        private static void IsDirtyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MediaCenterRegistryObject centerRegistryObject = (MediaCenterRegistryObject)sender;
            centerRegistryObject.OnIsDirtyChanged(EventArgs.Empty);
            if (centerRegistryObject.Manager == null)
                return;
            if ((bool)args.NewValue)
            {
                if (centerRegistryObject.Manager.DirtyObjects.Contains(centerRegistryObject))
                    return;
                centerRegistryObject.Manager.DirtyObjects.Add(centerRegistryObject);
            }
            else
                centerRegistryObject.Manager.DirtyObjects.Remove(centerRegistryObject);
        }

        public static RegistrySerialiser CreateRegistrySerialiser(OemManager mcm)
        {
            RegistrySerialiser registrySerialiser = new RegistrySerialiser();
            registrySerialiser.Context[(object)"mcm"] = (object)mcm;
            return registrySerialiser;
        }

        public virtual bool DeleteKey()
        {
            RegistryKey regHive = this.RegHive;
            if (regHive == null || this.RegPath == null)
                return false;
            regHive.DeleteSubKey(this.RegPath, false);
            this.IsDirty = false;
            return true;
        }

        bool IRegistryKeySerialisable.BeforeSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            this.Manager = this.GetMediaCenterManager(rs);
            if (this.saving != null)
                this.saving((object)this, EventArgs.Empty);
            return this.OnBeforeSerialise(rs, key);
        }

        void IRegistryKeySerialisable.AfterSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            this.Manager = this.GetMediaCenterManager(rs);
            this.OnAfterSerialise(rs, key);
            this.RegKey = key.Name;
            this.IsDirty = false;
            if (this.saved == null)
                return;
            this.saved((object)this, EventArgs.Empty);
        }

        bool IRegistryKeySerialisable.BeforeDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
            this.BeginInit();
            this.Manager = this.GetMediaCenterManager(rs);
            return this.OnBeforeDeserialise(rs, key);
        }

        void IRegistryKeySerialisable.AfterDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
            this.Manager = this.GetMediaCenterManager(rs);
            this.OnAfterDeserialise(rs, key);
            if (this.RegKey == null)
                this.RegKey = key.Name;
            this.EndInit();
        }

        public void BeginInit()
        {
            this.isInit = true;
        }

        public void EndInit()
        {
            this.isInit = false;
        }

        protected virtual bool OnBeforeSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            return true;
        }

        protected virtual void OnAfterSerialise(RegistrySerialiser rs, RegistryKey key)
        {
        }

        protected virtual bool OnBeforeDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
            return true;
        }

        protected virtual void OnAfterDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (this.isInit || this.ShouldIgnorePropertyChange(e))
                return;
            this.IsDirty = true;
        }

        protected virtual bool ShouldIgnorePropertyChange(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property != Selector.IsSelectedProperty)
                return e.Property == MediaCenterRegistryObject.IsDirtyProperty;
            else
                return true;
        }

        protected virtual void OnIsDirtyChanged(EventArgs args)
        {
            EventHandler eventHandler = this.isDirtyChanged;
            if (eventHandler == null)
                return;
            eventHandler((object)this, args);
        }

        protected virtual OemManager GetMediaCenterManager(RegistrySerialiser rs)
        {
            OemManager oemManager = rs.Context[(object)"mcm"] as OemManager;
            if (oemManager == null)
                throw new ArgumentException("The registry serialiser must contain a MediaCenterManager.");
            else
                return oemManager;
        }
    }
}
