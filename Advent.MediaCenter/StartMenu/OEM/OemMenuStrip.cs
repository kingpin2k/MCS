


using Advent.Common.Serialization;
using Advent.MediaCenter.StartMenu;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public class OemMenuStrip : ApplicationRefObject, IMenuStrip, ISupportInitialize
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(OemMenuStrip));
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(OemMenuStrip));
        public static readonly DependencyProperty TimeStampProperty = DependencyProperty.Register("TimeStamp", typeof(int), typeof(OemMenuStrip));
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(int), typeof(OemMenuStrip));
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(string), typeof(OemMenuStrip), new PropertyMetadata(new PropertyChangedCallback(OemMenuStrip.CategoryChanged)));
        private OemCategory stripCategory;

        public int Priority
        {
            get
            {
                return (int)this.GetValue(OemMenuStrip.PriorityProperty);
            }
            set
            {
                this.SetValue(OemMenuStrip.PriorityProperty, (object)value);
            }
        }

        [RegistryKeyName]
        public override string ApplicationID
        {
            get
            {
                return base.ApplicationID;
            }
            set
            {
                base.ApplicationID = value;
            }
        }

        public ObservableCollection<IQuickLink> QuickLinks
        {
            get
            {
                if (this.stripCategory != null)
                    return this.stripCategory.QuickLinks;
                else
                    return new ObservableCollection<IQuickLink>();
            }
        }

        [RegistryValue]
        public string Title
        {
            get
            {
                return (string)this.GetValue(OemMenuStrip.TitleProperty);
            }
            set
            {
                this.SetValue(OemMenuStrip.TitleProperty, (object)value);
            }
        }

        [RegistryValue("OnStartMenu", ValueKind = RegistryValueKind.String)]
        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(OemMenuStrip.IsEnabledProperty);
            }
            set
            {
                this.SetValue(OemMenuStrip.IsEnabledProperty, value);
            }
        }

        [RegistryValue]
        public string Category
        {
            get
            {
                return (string)this.GetValue(OemMenuStrip.CategoryProperty);
            }
            set
            {
                this.SetValue(OemMenuStrip.CategoryProperty, (object)value);
            }
        }

        [RegistryValue]
        public int TimeStamp
        {
            get
            {
                return (int)this.GetValue(OemMenuStrip.TimeStampProperty);
            }
            set
            {
                this.SetValue(OemMenuStrip.TimeStampProperty, (object)value);
            }
        }

        public bool CanSetLinkPriority
        {
            get
            {
                return false;
            }
        }

        public bool CanSetPriority
        {
            get
            {
                return true;
            }
        }

        public bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public bool CanEditTitle
        {
            get
            {
                return true;
            }
        }

        public bool CanDelete
        {
            get
            {
                return true;
            }
        }

        static OemMenuStrip()
        {
        }

        public bool CanSwapWith(IMenuStrip strip)
        {
            return true;
        }

        public bool CanAddQuickLink(IQuickLink link)
        {
            if (!(link is OemQuickLink))
                return link is IPartnerQuickLink;
            else
                return true;
        }

        protected override bool OnBeforeSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            if (this.Application == null)
                throw new InvalidOperationException("The menu strip must have an application.");
            if (!this.Application.IsSaved || this.Application.IsDirty)
            {
                using (RegistryKey subKey = this.Application.RegHive.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Applications"))
                    this.Manager.RegistrySerialiser.Serialise((object)this.Application, subKey);
            }
            return true;
        }

        protected override void OnAfterSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            if (this.RegHive == Registry.CurrentUser)
                return;
            string regPath = this.RegPath;
            if (string.IsNullOrEmpty(regPath))
                return;
            foreach (object obj in Registry.Users.GetSubKeyNames())
            {
                string subkey = string.Format("{0}\\{1}", obj, (object)regPath);
                Registry.Users.DeleteSubKey(subkey, false);
            }
        }

        private static void CategoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            OemMenuStrip oemMenuStrip = (OemMenuStrip)sender;
            if (oemMenuStrip.stripCategory != null)
                oemMenuStrip.stripCategory.QuickLinks.CollectionChanged -= new NotifyCollectionChangedEventHandler(oemMenuStrip.QuickLinks_CollectionChanged);
            oemMenuStrip.stripCategory = oemMenuStrip.Manager.Categories[(string)args.NewValue];
            oemMenuStrip.stripCategory.QuickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(oemMenuStrip.QuickLinks_CollectionChanged);
        }

        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;
            foreach (ApplicationRefObject applicationRefObject in (IEnumerable)e.NewItems)
                applicationRefObject.Application = this.Application;
        }
    }
}
