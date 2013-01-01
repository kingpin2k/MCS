


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Fiji
{
    internal class FijiMenuStrip : XmlMenuStrip
    {
        protected static readonly DependencyPropertyKey CanDeletePropertyKey = DependencyProperty.RegisterReadOnly("CanDelete", typeof(bool), typeof(FijiMenuStrip), new PropertyMetadata((object)true));
        protected static readonly DependencyPropertyKey CanSetEnabledPropertyKey = DependencyProperty.RegisterReadOnly("CanSetEnabled", typeof(bool), typeof(FijiMenuStrip), new PropertyMetadata((object)false));
        public static readonly DependencyProperty CanDeleteProperty = FijiMenuStrip.CanDeletePropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanSetEnabledProperty = FijiMenuStrip.CanSetEnabledPropertyKey.DependencyProperty;
        private string originalTitle;
        private string app;
        private string appId;

        public string StartMenuNamespace { get; set; }

        public override bool CanSetEnabled
        {
            get
            {
                return (bool)this.GetValue(FijiMenuStrip.CanSetEnabledProperty);
            }
        }

        public override bool CanDelete
        {
            get
            {
                return (bool)this.GetValue(FijiMenuStrip.CanDeleteProperty);
            }
        }

        private XmlNode StartMenuCategoryNode
        {
            get
            {
                XmlNodeList elementsByTagName1 = this.MenuStripDocument.GetElementsByTagName("home:StartMenuCategory");
                if (elementsByTagName1.Count > 0)
                    return elementsByTagName1[0];
                XmlNodeList elementsByTagName2 = this.MenuStripDocument.GetElementsByTagName("home:NetOpPackageStartMenuCategory");
                if (elementsByTagName2.Count > 0)
                    return elementsByTagName2[0];
                else
                    throw new InvalidOperationException("Could not find StartMenuCategory node.");
            }
        }

        private XmlNode QuickLinksNode
        {
            get
            {
                XmlNodeList elementsByTagName = this.MenuStripDocument.GetElementsByTagName("QuickLinks");
                if (elementsByTagName.Count > 0)
                    return elementsByTagName[0];
                else
                    throw new InvalidOperationException("Could not find QuickLinks node.");
            }
        }

        static FijiMenuStrip()
        {
        }

        public FijiMenuStrip(StartMenuManager manager, XmlDocument doc, XmlElement startMenuElement, string resourceName)
            : base(manager, doc, startMenuElement, resourceName)
        {
            this.QuickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.QuickLinks_CollectionChanged);
            this.UpdateEnabled();
        }

        internal override void Save(IResourceLibrary ehres)
        {
            XmlNode quickLinksNode = this.QuickLinksNode;
            MediaCenterUtil.StripChildComments(quickLinksNode);
            foreach (XmlQuickLink xmlQuickLink in (Collection<IQuickLink>)this.QuickLinks)
            {
                XmlNode parentNode = xmlQuickLink.XmlElement.ParentNode;
                if (parentNode != null)
                {
                    try
                    {
                        parentNode.RemoveChild((XmlNode)xmlQuickLink.XmlElement);
                    }
                    catch
                    {
                    }
                }
                bool flag = true;
                FijiPartnerQuickLink partnerQuickLink = xmlQuickLink as FijiPartnerQuickLink;
                if (partnerQuickLink != null)
                    flag = !partnerQuickLink.RemoveIfEmpty || partnerQuickLink.OemQuickLink != null;
                if (flag)
                {
                    if (xmlQuickLink.IsEnabled)
                    {
                        quickLinksNode.AppendChild((XmlNode)xmlQuickLink.XmlElement);
                    }
                    else
                    {
                        XmlComment comment = this.MenuStripDocument.CreateComment(xmlQuickLink.XmlElement.OuterXml);
                        quickLinksNode.AppendChild((XmlNode)comment);
                    }
                }
            }
            if (this.Title != this.originalTitle)
                this.StartMenuCategoryNode.Attributes["Description"].Value = this.Title;
            base.Save(ehres);
        }

        protected override void Load()
        {
            XmlAttribute xmlAttribute1 = this.StartMenuCategoryNode.Attributes["App"];
            this.app = xmlAttribute1 == null ? (string)null : xmlAttribute1.Value;
            XmlAttribute xmlAttribute2 = this.StartMenuCategoryNode.Attributes["AppId"];
            this.appId = xmlAttribute2 == null ? (string)null : xmlAttribute2.Value;
            string uri = this.StartMenuCategoryNode.Attributes["Description"].Value;
            this.Title = MediaCenterUtil.GetStringResource(this.Manager.Resources, uri, false) ?? uri;
            this.originalTitle = this.Title;
            this.IsEnabled = true;
            XmlNodeList childNodes = this.QuickLinksNode.ChildNodes;
            for (int index = 0; index < childNodes.Count; ++index)
            {
                bool flag = true;
                XmlElement element = childNodes[index] as XmlElement;
                if (element == null)
                {
                    XmlComment comment = childNodes[index] as XmlComment;
                    if (comment != null)
                    {
                        element = MediaCenterUtil.UncommentElement(comment);
                        flag = element == null;
                    }
                }
                if (element != null)
                {
                    IQuickLink quickLink = (IQuickLink)null;
                    try
                    {
                        quickLink = element.Name == "home:PackageMarkupQuickLink" || element.Name == "home:ExtensiblityPackageQuickLink" || element.Name == "home:DualPackageMarkupQuickLink" ? (IQuickLink)new PackageQuickLink(this.Manager, element) : (!(element.Name == "home:DefaultPartnerQuicklink") ? (IQuickLink)new FijiQuickLink(this.Manager, element) : (IQuickLink)new FijiPartnerQuickLink(this.Manager, element));
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Failed to load link. Exception is:\n{0}\nXml for element is:\n{1}", (object)((object)ex).ToString(), (object)element.OuterXml);
                    }
                    if (quickLink != null)
                    {
                        quickLink.BeginInit();
                        int result;
                        if (int.TryParse(element.GetAttribute("Priority"), out result))
                        {
                            quickLink.Priority = result;
                            quickLink.IsEnabled = flag;
                            this.QuickLinks.Add(quickLink);
                        }
                        quickLink.EndInit();
                    }
                }
            }
        }

        protected override IPartnerQuickLink GetFreePartnerQuickLink(OemQuickLink oemLink)
        {
            IPartnerQuickLink partnerQuickLink = base.GetFreePartnerQuickLink(oemLink);
            if (partnerQuickLink == null)
            {
                XmlElement element = this.MenuStripDocument.CreateElement("home", "DefaultPartnerQuicklink", this.MenuStripDocument.DocumentElement.GetAttribute("xmlns:home"));
                element.SetAttribute("ExtensibilityCategory", string.Format("{0}\\Category {{{1}}}", (object)this.Manager.CustomInternalCategory, (object)Guid.NewGuid()));
                int num = 100;
                element.SetAttribute("Priority", num.ToString());
                partnerQuickLink = (IPartnerQuickLink)new FijiPartnerQuickLink(this.Manager, element, true);
                partnerQuickLink.BeginInit();
                partnerQuickLink.Priority = num;
                XmlMenuStrip.XmlQuickLinkCollection quickLinkCollection = (XmlMenuStrip.XmlQuickLinkCollection)this.QuickLinks;
                quickLinkCollection.BeginInit();
                quickLinkCollection.Add((IQuickLink)partnerQuickLink);
                quickLinkCollection.EndInit();
                partnerQuickLink.EndInit();
            }
            return partnerQuickLink;
        }

        protected override void OnPriorityChanged(int oldValue, int newValue)
        {
            this.StartMenuTargetElement.SetAttribute("Priority", newValue.ToString());
        }

        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsInitInProgress && e.NewItems != null)
            {
                foreach (IQuickLink quickLink in (IEnumerable)e.NewItems)
                {
                    FijiQuickLink fijiQuickLink = quickLink as FijiQuickLink;
                    if (fijiQuickLink != null)
                    {
                        if (this.app != null)
                        {
                            fijiQuickLink.XmlElement.SetAttribute("App", this.app);
                            fijiQuickLink.XmlElement.RemoveAttribute("AppId");
                        }
                        else if (this.appId != null)
                        {
                            fijiQuickLink.XmlElement.SetAttribute("AppId", this.appId);
                            fijiQuickLink.XmlElement.RemoveAttribute("App");
                        }
                    }
                }
            }
            this.UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            bool flag1 = false;
            foreach (IQuickLink quickLink in (Collection<IQuickLink>)this.QuickLinks)
            {
                if (quickLink is FijiQuickLink)
                {
                    flag1 = true;
                    this.SetValue(FijiMenuStrip.CanSetEnabledPropertyKey, (object)true);
                    this.SetValue(FijiMenuStrip.CanDeletePropertyKey, (object)false);
                    break;
                }
            }
            if (flag1)
                return;
            bool flag2 = this.app == null && this.appId != null;
            this.SetValue(FijiMenuStrip.CanSetEnabledPropertyKey,!flag2);
            this.SetValue(FijiMenuStrip.CanDeletePropertyKey, flag2 );
        }
    }
}
