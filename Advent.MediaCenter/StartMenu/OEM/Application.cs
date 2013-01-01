


using Advent.Common.Serialization;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Advent.MediaCenter.StartMenu.OEM
{
    public class Application : MediaCenterRegistryObject, IComparable
    {
        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(string), typeof(Advent.MediaCenter.StartMenu.OEM.Application));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Advent.MediaCenter.StartMenu.OEM.Application), new PropertyMetadata(new PropertyChangedCallback(Advent.MediaCenter.StartMenu.OEM.Application.TitleChanged)));
        public static readonly DependencyProperty RawTitleProperty = DependencyProperty.Register("RawTitle", typeof(string), typeof(Advent.MediaCenter.StartMenu.OEM.Application), new PropertyMetadata(new PropertyChangedCallback(Advent.MediaCenter.StartMenu.OEM.Application.RawTitleChanged)));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(Advent.MediaCenter.StartMenu.OEM.Application));
        public static readonly DependencyProperty CompanyNameProperty = DependencyProperty.Register("CompanyName", typeof(string), typeof(Advent.MediaCenter.StartMenu.OEM.Application));
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(Advent.MediaCenter.StartMenu.OEM.Application), new PropertyMetadata((object)true));
        private readonly ObservableCollection<EntryPoint> entryPoints;

        [RegistryKeyName]
        public string ID
        {
            get
            {
                return (string)this.GetValue(Advent.MediaCenter.StartMenu.OEM.Application.IDProperty);
            }
            set
            {
                this.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.IDProperty, (object)value);
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(Advent.MediaCenter.StartMenu.OEM.Application.TitleProperty);
            }
            set
            {
                this.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.TitleProperty, (object)value);
            }
        }

        [RegistryValue("Title")]
        public string RawTitle
        {
            get
            {
                return (string)this.GetValue(Advent.MediaCenter.StartMenu.OEM.Application.RawTitleProperty);
            }
            set
            {
                this.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.RawTitleProperty, (object)value);
            }
        }

        [RegistryValue]
        public string Description
        {
            get
            {
                return (string)this.GetValue(Advent.MediaCenter.StartMenu.OEM.Application.DescriptionProperty);
            }
            set
            {
                this.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.DescriptionProperty, (object)value);
            }
        }

        [RegistryValue]
        public string CompanyName
        {
            get
            {
                return (string)this.GetValue(Advent.MediaCenter.StartMenu.OEM.Application.CompanyNameProperty);
            }
            set
            {
                this.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.CompanyNameProperty, (object)value);
            }
        }

        [RegistryValue("Enabled", ValueKind = RegistryValueKind.String)]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(Advent.MediaCenter.StartMenu.OEM.Application.IsEnabledProperty);
            }
            set
            {
                //TODO original (object)(bool)(value ? 1 : 0)
                this.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.IsEnabledProperty, value);
            }
        }

        public ObservableCollection<EntryPoint> EntryPoints
        {
            get
            {
                return this.entryPoints;
            }
        }

        static Application()
        {
        }

        public Application()
        {
            this.entryPoints = new ObservableCollection<EntryPoint>();
            this.entryPoints.CollectionChanged += new NotifyCollectionChangedEventHandler(this.EntryPoints_CollectionChanged);
        }

        public override string ToString()
        {
            return this.Title ?? "(no title)";
        }

        public int CompareTo(object obj)
        {
            return this.ToString().CompareTo(obj.ToString());
        }

        protected override bool OnBeforeSerialise(RegistrySerialiser rs, RegistryKey key)
        {
            if (this.ID == null || this.ID.Trim() == string.Empty)
                throw new InvalidOperationException("The application must have an ID.");
            if (this.Title == null || this.Title.Trim() == string.Empty)
                throw new InvalidOperationException("The application must have a title.");
            else
                return true;
        }

        protected override void OnAfterDeserialise(RegistrySerialiser rs, RegistryKey key)
        {
            this.EntryPoints.Clear();
            foreach (EntryPoint entryPoint in (Collection<EntryPoint>)this.Manager.EntryPoints)
            {
                if (entryPoint.ApplicationID == this.ID)
                    this.EntryPoints.Add(entryPoint);
            }
        }

        private static void TitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!((string)args.NewValue != (string)args.OldValue))
                return;
            sender.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.RawTitleProperty, args.NewValue);
        }

        private static void RawTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.SetValue(Advent.MediaCenter.StartMenu.OEM.Application.TitleProperty, (object)OemManager.ResolveString((string)args.NewValue));
        }

        private void EntryPoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move || e.NewItems == null)
                return;
            foreach (ApplicationRefObject applicationRefObject in (IEnumerable)e.NewItems)
                applicationRefObject.Application = this;
        }
    }
}
