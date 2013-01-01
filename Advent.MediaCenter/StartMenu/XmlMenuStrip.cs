


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu.OEM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal abstract class XmlMenuStrip : BaseXmlMenuStrip, IMenuStrip, ISupportInitialize
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(XmlMenuStrip));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(XmlMenuStrip));
        private XmlMenuStrip.XmlQuickLinkCollection quickLinks;
        private XmlDocument xml;
        private string resourceName;

        public string DocumentResourceName
        {
            get
            {
                return this.resourceName;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(XmlMenuStrip.IsEnabledProperty);
            }
            set
            {
                this.SetValue(XmlMenuStrip.IsEnabledProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(XmlMenuStrip.TitleProperty);
            }
            set
            {
                this.SetValue(XmlMenuStrip.TitleProperty, (object)value);
            }
        }

        public ObservableCollection<IQuickLink> QuickLinks
        {
            get
            {
                return (ObservableCollection<IQuickLink>)this.quickLinks;
            }
        }

        public virtual bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public bool CanSetLinkPriority
        {
            get
            {
                return true;
            }
        }

        public bool CanSetPriority
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

        public virtual bool CanDelete
        {
            get
            {
                return false;
            }
        }

        protected XmlDocument MenuStripDocument
        {
            get
            {
                return this.xml;
            }
        }

        static XmlMenuStrip()
        {
        }

        public XmlMenuStrip(StartMenuManager manager, XmlDocument doc, XmlElement startMenuElement, string resourceName)
            : base(manager, startMenuElement)
        {
            this.quickLinks = new XmlMenuStrip.XmlQuickLinkCollection(this);
            this.quickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.QuickLinks_CollectionChanged);
            this.resourceName = resourceName;
            this.xml = doc;
            this.LoadFromXml();
        }

        public virtual bool CanSwapWith(IMenuStrip strip)
        {
            return true;
        }

        public bool CanAddQuickLink(IQuickLink link)
        {
            OemQuickLink link1 = link as OemQuickLink;
            if (link1 == null)
            {
                IPartnerQuickLink partnerQuickLink = link as IPartnerQuickLink;
                if (partnerQuickLink != null)
                    link1 = partnerQuickLink.OemQuickLink;
            }
            if (link1 != null)
            {
                if (!this.quickLinks.GetPartnerQuickLinks().Contains(link1))
                    return this.GetFreePartnerQuickLink(link1) != null;
                else
                    return true;
            }
            else
            {
                if (!(link is XmlQuickLink))
                    return false;
                if (link.OriginalStrip != null && link.OriginalStrip != this)
                    return this.Manager.Strips.IndexOf((IMenuStrip)this) > this.Manager.Strips.IndexOf(link.OriginalStrip);
                else
                    return true;
            }
        }

        internal override void Save(IResourceLibrary ehres)
        {
            foreach (StartMenuObject startMenuObject in (Collection<IQuickLink>)this.quickLinks)
                startMenuObject.Save(ehres);
            MediaCenterUtil.SaveXmlResource(ehres, this.DocumentResourceName, 23, this.xml);
            base.Save(ehres);
        }

        protected abstract void Load();

        protected virtual IPartnerQuickLink GetFreePartnerQuickLink(OemQuickLink link)
        {
            return this.quickLinks.GetFreePartnerQuickLink();
        }

        private void LoadFromXml()
        {
            this.BeginInit();
            this.quickLinks.Clear();
            try
            {
                this.quickLinks.BeginInit();
                this.Load();
            }
            finally
            {
                this.quickLinks.EndInit();
                this.EndInit();
            }
        }

        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInitInProgress)
                return;
            this.IsDirty = true;
        }

        protected class XmlQuickLinkCollection : ObservableCollection<IQuickLink>, ISupportInitialize
        {
            private bool inInit;
            private XmlMenuStrip strip;

            public XmlQuickLinkCollection(XmlMenuStrip strip)
            {
                this.strip = strip;
                this.inInit = false;
            }

            public List<OemQuickLink> GetPartnerQuickLinks()
            {
                List<OemQuickLink> list = new List<OemQuickLink>();
                foreach (IQuickLink quickLink in (Collection<IQuickLink>)this)
                {
                    IPartnerQuickLink partnerQuickLink = quickLink as IPartnerQuickLink;
                    if (partnerQuickLink != null && partnerQuickLink.OemQuickLink != null)
                    {
                        OemQuickLink oemQuickLink = partnerQuickLink.OemQuickLink;
                        if (oemQuickLink != null)
                            list.Add(oemQuickLink);
                    }
                }
                return list;
            }

            public virtual IPartnerQuickLink GetFreePartnerQuickLink()
            {
                foreach (IQuickLink quickLink in (Collection<IQuickLink>)this)
                {
                    IPartnerQuickLink partnerQuickLink = quickLink as IPartnerQuickLink;
                    if (partnerQuickLink != null && partnerQuickLink.OemQuickLink == null)
                        return partnerQuickLink;
                }
                return (IPartnerQuickLink)null;
            }

            public void BeginInit()
            {
                this.inInit = true;
            }

            public void EndInit()
            {
                this.inInit = false;
            }

            protected override void ClearItems()
            {
                for (int index = this.Count - 1; index >= 0; --index)
                    this.RemoveItem(index);
            }

            protected override void RemoveItem(int index)
            {
                IPartnerQuickLink partnerQuickLink = this[index] as IPartnerQuickLink;
                if (partnerQuickLink != null)
                    partnerQuickLink.OemQuickLink = (OemQuickLink)null;
                else
                    base.RemoveItem(index);
            }

            protected override void InsertItem(int index, IQuickLink item)
            {
                if (!this.inInit)
                {
                    IPartnerQuickLink partnerQuickLink1 = item as IPartnerQuickLink;
                    OemQuickLink oemQuickLink = partnerQuickLink1 == null ? item as OemQuickLink : partnerQuickLink1.OemQuickLink;
                    if (oemQuickLink != null)
                    {
                        IPartnerQuickLink partnerQuickLink2 = this.GetFreePartnerQuickLink();
                        if (partnerQuickLink2 != null)
                        {
                            partnerQuickLink2.OemQuickLink = oemQuickLink;
                            int oldIndex = this.IndexOf((IQuickLink)partnerQuickLink2);
                            this.Move(oldIndex, oldIndex < index ? index - 1 : index);
                        }
                    }
                    else
                        base.InsertItem(index, item);
                    XmlQuickLink xmlQuickLink = item as XmlQuickLink;
                    if (xmlQuickLink == null || xmlQuickLink.XmlElement.OwnerDocument == this.strip.xml || xmlQuickLink is IPartnerQuickLink)
                        return;
                    if (xmlQuickLink.XmlElement.ParentNode != null)
                    {
                        XmlElement documentElement = this.strip.MenuStripDocument.DocumentElement;
                        foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap)xmlQuickLink.XmlElement.OwnerDocument.DocumentElement.Attributes)
                        {
                            XmlAttribute attributeNode = documentElement.GetAttributeNode(xmlAttribute.Name);
                            if (xmlAttribute.Name.StartsWith("xmlns:") && attributeNode == null)
                                documentElement.SetAttribute(xmlAttribute.Name, xmlAttribute.Value);
                        }
                        xmlQuickLink.XmlElement.ParentNode.RemoveChild((XmlNode)xmlQuickLink.XmlElement);
                    }
                    xmlQuickLink.XmlElement = (XmlElement)this.strip.xml.ImportNode((XmlNode)xmlQuickLink.XmlElement, true);
                }
                else
                    base.InsertItem(index, item);
            }
        }
    }
}
