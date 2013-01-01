


using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal abstract class XmlQuickLink : StartMenuObject, IQuickLink, ISupportInitialize
    {
        protected static readonly DependencyPropertyKey IsValidPropertyKey = DependencyProperty.RegisterReadOnly("IsValid", typeof(bool), typeof(XmlQuickLink), new PropertyMetadata((object)true));
        public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority", typeof(int), typeof(XmlQuickLink), new PropertyMetadata(new PropertyChangedCallback(XmlQuickLink.PriorityChanged)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(XmlQuickLink));
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(XmlQuickLink));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(XmlQuickLink));
        public static readonly DependencyProperty NonFocusImageProperty = DependencyProperty.Register("NonFocusImage", typeof(ImageSource), typeof(XmlQuickLink));
        public static readonly DependencyProperty IsValidProperty = XmlQuickLink.IsValidPropertyKey.DependencyProperty;

        public XmlElement XmlElement { get; internal set; }

        public virtual XmlElement LinkElement
        {
            get
            {
                return this.XmlElement;
            }
        }

        public virtual string Title
        {
            get
            {
                return (string)this.GetValue(XmlQuickLink.TitleProperty);
            }
            set
            {
                this.SetValue(XmlQuickLink.TitleProperty, (object)value);
            }
        }

        public ImageSource Image
        {
            get
            {
                return (ImageSource)this.GetValue(XmlQuickLink.ImageProperty);
            }
        }

        public ImageSource NonFocusImage
        {
            get
            {
                return (ImageSource)this.GetValue(XmlQuickLink.NonFocusImageProperty);
            }
        }

        public bool IsValid
        {
            get
            {
                return (bool)this.GetValue(XmlQuickLink.IsValidProperty);
            }
        }

        public int Priority
        {
            get
            {
                return (int)this.GetValue(XmlQuickLink.PriorityProperty);
            }
            set
            {
                this.SetValue(XmlQuickLink.PriorityProperty, (object)value);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(XmlQuickLink.IsEnabledProperty);
            }
            set
            {
                this.SetValue(XmlQuickLink.IsEnabledProperty, value);
            }
        }

        public virtual bool CanSetEnabled
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanDelete
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanEditTitle
        {
            get
            {
                return true;
            }
        }

        public virtual IMenuStrip OriginalStrip
        {
            get
            {
                return (IMenuStrip)null;
            }
        }

        static XmlQuickLink()
        {
        }

        protected XmlQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm)
        {
            this.XmlElement = element;
        }

        public override void EndInit()
        {
            this.Load();
            base.EndInit();
        }

        protected abstract void Load();

        private static void PriorityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((XmlQuickLink)sender).LinkElement.SetAttribute("Priority", args.NewValue.ToString());
        }
    }
}
