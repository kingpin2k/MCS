


using Advent.MediaCenter.StartMenu.OEM;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal class PartnerQuickLinkBase : XmlQuickLink, IPartnerQuickLink, IQuickLink, ISupportInitialize
    {
        public static readonly DependencyProperty OemQuickLinkProperty = DependencyProperty.Register("OemQuickLink", typeof(OemQuickLink), typeof(PartnerQuickLinkBase), new PropertyMetadata(new PropertyChangedCallback(PartnerQuickLinkBase.OemQuickLinkChanged)));

        public OemQuickLink OemQuickLink
        {
            get
            {
                return (OemQuickLink)this.GetValue(PartnerQuickLinkBase.OemQuickLinkProperty);
            }
            set
            {
                this.SetValue(PartnerQuickLinkBase.OemQuickLinkProperty, (object)value);
            }
        }

        public override bool CanDelete
        {
            get
            {
                return true;
            }
        }

        static PartnerQuickLinkBase()
        {
            XmlQuickLink.IsValidPropertyKey.OverrideMetadata(typeof(PartnerQuickLinkBase), new PropertyMetadata((object)false));
        }

        public PartnerQuickLinkBase(StartMenuManager manager, XmlElement element)
            : base(manager, element)
        {
        }

        protected override void Load()
        {
            BindingOperations.SetBinding((DependencyObject)this, XmlQuickLink.TitleProperty, (BindingBase)new Binding("OemQuickLink.Title")
            {
                Source = (object)this,
                Mode = BindingMode.TwoWay
            });
            BindingOperations.SetBinding((DependencyObject)this, XmlQuickLink.ImageProperty, (BindingBase)new Binding("OemQuickLink.Image")
            {
                Source = (object)this
            });
            BindingOperations.SetBinding((DependencyObject)this, XmlQuickLink.NonFocusImageProperty, (BindingBase)new Binding("OemQuickLink.NonFocusImage")
            {
                Source = (object)this
            });
        }

        protected override bool CanSetDirty(DependencyProperty property)
        {
            if (base.CanSetDirty(property) && property != XmlQuickLink.TitleProperty && property != XmlQuickLink.ImageProperty)
                return property != XmlQuickLink.NonFocusImageProperty;
            else
                return false;
        }

        protected virtual void OnQuickLinkChanged(OemQuickLink oldValue, OemQuickLink newValue)
        {
        }

        private static void OemQuickLinkChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            sender.SetValue(XmlQuickLink.IsValidPropertyKey, (object)(args.NewValue != null ? true : false));
            PartnerQuickLinkBase partnerQuickLinkBase = (PartnerQuickLinkBase)sender;
            partnerQuickLinkBase.OnQuickLinkChanged((OemQuickLink)args.OldValue, (OemQuickLink)args.NewValue);
            if (partnerQuickLinkBase.OemQuickLink != null)
                partnerQuickLinkBase.IsEnabled = partnerQuickLinkBase.OemQuickLink.IsEnabled;
            else
                partnerQuickLinkBase.IsEnabled = true;
        }
    }
}
