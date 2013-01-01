


using Advent.Common.Serialization;
using Advent.MediaCenter.StartMenu;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public class OemQuickLink : ApplicationRefObject, IQuickLink, ISupportInitialize, IComparable<OemQuickLink>
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(OemQuickLink));
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(int), typeof(OemQuickLink));
        public static readonly DependencyProperty TimeStampProperty = DependencyProperty.Register("TimeStamp", typeof(int), typeof(OemQuickLink));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(OemQuickLink));
        public static readonly DependencyProperty NonFocusImageProperty = DependencyProperty.Register("NonFocusImage", typeof(ImageSource), typeof(OemQuickLink));
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(OemQuickLink), new PropertyMetadata((object)true));
        public static readonly DependencyProperty EntryPointIDProperty = DependencyProperty.Register("EntryPointID", typeof(string), typeof(OemQuickLink), new PropertyMetadata(new PropertyChangedCallback(OemQuickLink.EntryPointIDChanged)));
        public static readonly DependencyProperty EntryPointProperty = DependencyProperty.Register("EntryPoint", typeof(EntryPoint), typeof(OemQuickLink), new PropertyMetadata(new PropertyChangedCallback(OemQuickLink.EntryPointChanged)));

        [RegistryValue("AppId")]
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

        [RegistryValue]
        public int TimeStamp
        {
            get
            {
                return (int)this.GetValue(OemQuickLink.TimeStampProperty);
            }
            set
            {
                this.SetValue(OemQuickLink.TimeStampProperty, (object)value);
            }
        }

        [RegistryKeyName]
        public string EntryPointID
        {
            get
            {
                return (string)this.GetValue(OemQuickLink.EntryPointIDProperty);
            }
            set
            {
                this.SetValue(OemQuickLink.EntryPointIDProperty, (object)value);
            }
        }

        public EntryPoint EntryPoint
        {
            get
            {
                return (EntryPoint)this.GetValue(OemQuickLink.EntryPointProperty);
            }
            set
            {
                this.SetValue(OemQuickLink.EntryPointProperty, (object)value);
            }
        }

        public IMenuStrip OriginalStrip
        {
            get
            {
                return (IMenuStrip)null;
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(OemQuickLink.TitleProperty);
            }
            set
            {
                this.SetValue(OemQuickLink.TitleProperty, (object)value);
            }
        }

        public ImageSource Image
        {
            get
            {
                return (ImageSource)this.GetValue(OemQuickLink.ImageProperty);
            }
        }

        public ImageSource NonFocusImage
        {
            get
            {
                return (ImageSource)this.GetValue(OemQuickLink.NonFocusImageProperty);
            }
        }

        public int Priority
        {
            get
            {
                return (int)this.GetValue(OemQuickLink.PriorityProperty);
            }
            set
            {
                this.SetValue(OemQuickLink.PriorityProperty, (object)value);
            }
        }

        public bool IsValid
        {
            get
            {
                return this.EntryPoint != null;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(OemQuickLink.IsEnabledProperty);
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool CanSetEnabled
        {
            get
            {
                return false;
            }
        }

        public bool CanDelete
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

        static OemQuickLink()
        {
        }

        public OemQuickLink()
        {
            Binding binding1 = new Binding("EntryPoint.Title")
            {
                Source = (object)this,
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding((DependencyObject)this, OemQuickLink.TitleProperty, (BindingBase)binding1);
            Binding binding2 = new Binding("TimeStamp")
            {
                Source = (object)this,
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding((DependencyObject)this, OemQuickLink.PriorityProperty, (BindingBase)binding2);
            Binding binding3 = new Binding("EntryPoint.Image")
            {
                Source = (object)this
            };
            BindingOperations.SetBinding((DependencyObject)this, OemQuickLink.ImageProperty, (BindingBase)binding3);
            Binding binding4 = new Binding("EntryPoint.NonFocusImage")
            {
                Source = (object)this
            };
            BindingOperations.SetBinding((DependencyObject)this, OemQuickLink.NonFocusImageProperty, (BindingBase)binding4);
            Binding binding5 = new Binding("Application.IsEnabled")
            {
                Source = (object)this
            };
            BindingOperations.SetBinding((DependencyObject)this, OemQuickLink.IsEnabledProperty, (BindingBase)binding5);
        }

        public OemQuickLink(OemManager manager)
            : this()
        {
            this.Manager = manager;
        }

        public int CompareTo(OemQuickLink other)
        {
            return this.TimeStamp.CompareTo(other.TimeStamp);
        }

        protected override bool ShouldIgnorePropertyChange(DependencyPropertyChangedEventArgs e)
        {
            if (!base.ShouldIgnorePropertyChange(e) && e.Property != OemQuickLink.TitleProperty && e.Property != OemQuickLink.ImageProperty)
                return e.Property == OemQuickLink.NonFocusImageProperty;
            else
                return true;
        }

        protected override bool OnBeforeSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            EntryPoint entryPoint = this.EntryPoint;
            if (entryPoint != null && !entryPoint.RegKey.Contains("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Media Center\\Extensibility\\Entry Points"))
                rs.Serialise((object)entryPoint, key, false);
            return true;
        }

        private static void EntryPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
                return;
            EntryPoint entryPoint = (EntryPoint)args.NewValue;
            ((OemQuickLink)sender).EntryPointID = entryPoint.ID;
        }

        private static void EntryPointIDChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            OemQuickLink oemQuickLink = (OemQuickLink)sender;
            oemQuickLink.EntryPoint = oemQuickLink.Manager.EntryPoints[(string)args.NewValue];
        }
    }
}
