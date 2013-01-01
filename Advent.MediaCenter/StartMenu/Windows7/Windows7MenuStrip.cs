


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class Windows7MenuStrip : XmlMenuStrip
    {
        protected static readonly DependencyPropertyKey CanDeletePropertyKey = DependencyProperty.RegisterReadOnly("CanDelete", typeof(bool), typeof(Windows7MenuStrip), new PropertyMetadata((object)true));
        public static readonly DependencyProperty CanDeleteProperty = Windows7MenuStrip.CanDeletePropertyKey.DependencyProperty;
        protected static readonly DependencyPropertyKey CanSetEnabledPropertyKey = DependencyProperty.RegisterReadOnly("CanSetEnabled", typeof(bool), typeof(Windows7MenuStrip), new PropertyMetadata((object)false));
        public static readonly DependencyProperty CanSetEnabledProperty = Windows7MenuStrip.CanSetEnabledPropertyKey.DependencyProperty;
        private const int DefaultLinkPriority = 2147483647;
        private string m_originalTitle;
        private string m_app;
        private string m_appId;
        private XmlElement targetElement;

        protected override XmlElement StartMenuTargetElement
        {
            get
            {
                return this.targetElement;
            }
        }

        public string StartMenuNamespace { get; set; }

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
                if (elementsByTagName.Count <= 0)
                    throw new InvalidOperationException("Could not find QuickLinks node.");
                XmlNode firstChild = elementsByTagName[0].FirstChild;
                if (firstChild.Name == "constraints:ConstrainedList")
                    firstChild = firstChild.FirstChild;
                return firstChild;
            }
        }

        public override bool CanSetEnabled
        {
            get
            {
                return (bool)this.GetValue(Windows7MenuStrip.CanSetEnabledProperty);
            }
        }

        public override bool CanDelete
        {
            get
            {
                return (bool)this.GetValue(Windows7MenuStrip.CanDeleteProperty);
            }
        }

        static Windows7MenuStrip()
        {
        }

        public Windows7MenuStrip(StartMenuManager manager, XmlDocument doc, XmlElement startMenuElement, XmlElement startMenuTargetElement, string resourceName)
            : base(manager, doc, startMenuElement, resourceName)
        {
            this.BeginInit();
            this.targetElement = startMenuTargetElement;
            this.QuickLinks.CollectionChanged += new NotifyCollectionChangedEventHandler(this.QuickLinks_CollectionChanged);
            this.UpdateEnabled();
            XmlElement menuCategoryElement = this.GetStartMenuCategoryElement();
            if (menuCategoryElement != null)
            {
                string attribute = menuCategoryElement.GetAttribute("Priority");
                int result;
                if (attribute != null && int.TryParse(attribute, out result))
                    this.Priority = result;
            }
            this.EndInit();
        }

        protected override IPartnerQuickLink GetFreePartnerQuickLink(OemQuickLink oemLink)
        {
            IPartnerQuickLink partnerQuickLink = base.GetFreePartnerQuickLink(oemLink);
            if (partnerQuickLink == null)
            {
                string attribute1 = this.MenuStripDocument.DocumentElement.GetAttribute("xmlns");
                string attribute2 = this.MenuStripDocument.DocumentElement.GetAttribute("xmlns:home");
                XmlElement element1 = this.MenuStripDocument.CreateElement("home", "CommandQuickLink", attribute2);
                element1.SetAttribute("SqmTrackingId", "-1");
                element1.SetAttribute("ViewTemplate", "@res://Microsoft.MediaCenter.Shell!StartMenuQuickLink.mcml#PartnerQuickLinkContent");
                XmlElement element2 = this.MenuStripDocument.CreateElement("Image", attribute1);
                element1.AppendChild((XmlNode)element2);
                XmlElement element3 = this.MenuStripDocument.CreateElement("shll", "ImageSet", this.MenuStripDocument.DocumentElement.GetAttribute("xmlns:shll"));
                element3.SetAttribute("DefaultImageName", "Default");
                element2.AppendChild((XmlNode)element3);
                XmlElement element4 = this.MenuStripDocument.CreateElement("Values", attribute1);
                element3.AppendChild((XmlNode)element4);
                XmlElement element5 = this.MenuStripDocument.CreateElement("Image", attribute1);
                element5.SetAttribute("Name", "Default");
                element4.AppendChild((XmlNode)element5);
                XmlElement element6 = this.MenuStripDocument.CreateElement("Image", attribute1);
                element6.SetAttribute("Name", "Focus");
                element4.AppendChild((XmlNode)element6);
                XmlElement element7 = this.MenuStripDocument.CreateElement("Command", attribute1);
                element1.AppendChild((XmlNode)element7);
                XmlElement element8 = this.MenuStripDocument.CreateElement("home", "LaunchExtensibilityEntryPointCommand", attribute2);
                element7.AppendChild((XmlNode)element8);
                partnerQuickLink = (IPartnerQuickLink)new Windows7PartnerQuickLink(this.Manager, element1);
                partnerQuickLink.BeginInit();
                partnerQuickLink.Priority = int.MaxValue;
                XmlMenuStrip.XmlQuickLinkCollection quickLinkCollection = (XmlMenuStrip.XmlQuickLinkCollection)this.QuickLinks;
                quickLinkCollection.BeginInit();
                quickLinkCollection.Add((IQuickLink)partnerQuickLink);
                quickLinkCollection.EndInit();
                partnerQuickLink.EndInit();
            }
            return partnerQuickLink;
        }

        private void QuickLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsInitInProgress && e.NewItems != null)
            {
                foreach (IQuickLink quickLink in (IEnumerable)e.NewItems)
                {
                    XmlQuickLink xmlQuickLink = quickLink as XmlQuickLink;
                    if (xmlQuickLink != null)
                    {
                        if (this.m_app != null)
                        {
                            xmlQuickLink.LinkElement.SetAttribute("App", this.m_app);
                            xmlQuickLink.LinkElement.RemoveAttribute("AppId");
                        }
                        else if (this.m_appId != null)
                        {
                            xmlQuickLink.LinkElement.SetAttribute("AppId", this.m_appId);
                            xmlQuickLink.LinkElement.RemoveAttribute("App");
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
                if (quickLink is Windows7QuickLink)
                {
                    flag1 = true;
                    this.SetValue(Windows7MenuStrip.CanSetEnabledPropertyKey, (object)true);
                    this.SetValue(Windows7MenuStrip.CanDeletePropertyKey, (object)false);
                    break;
                }
            }
            if (flag1)
                return;
            bool flag2 = this.m_app == null && this.m_appId != null;
            //TODO Original (object)(bool)(!flag2 ? true : false)
            this.SetValue(Windows7MenuStrip.CanSetEnabledPropertyKey, (object)(!flag2 ? true : false));
            this.SetValue(Windows7MenuStrip.CanDeletePropertyKey, (object)(flag2 ? true : false));
        }

        protected override void Load()
        {
            XmlAttribute xmlAttribute1 = this.StartMenuCategoryNode.Attributes["App"];
            this.m_app = xmlAttribute1 == null ? (string)null : xmlAttribute1.Value;
            XmlAttribute xmlAttribute2 = this.StartMenuCategoryNode.Attributes["AppId"];
            this.m_appId = xmlAttribute2 == null ? (string)null : xmlAttribute2.Value;
            string uri = this.StartMenuCategoryNode.Attributes["Description"].Value;
            this.Title = MediaCenterUtil.GetStringResource(this.Manager.Resources, uri, false) ?? uri;
            this.m_originalTitle = this.Title;
            this.IsEnabled = true;
            XmlNodeList childNodes = this.QuickLinksNode.ChildNodes;
            for (int index = 0; index < childNodes.Count; ++index)
            {
                bool flag = true;
                XmlElement xmlElement1 = childNodes[index] as XmlElement;
                if (xmlElement1 == null)
                {
                    XmlComment comment = childNodes[index] as XmlComment;
                    if (comment != null)
                    {
                        xmlElement1 = MediaCenterUtil.UncommentElement(comment);
                        flag = xmlElement1 == null;
                    }
                }
                if (xmlElement1 != null)
                {
                    XmlElement linkElement = Windows7QuickLinkBase.GetLinkElement(xmlElement1);
                    IQuickLink quickLink = (IQuickLink)null;
                    try
                    {
                        if (linkElement.Name == "home:PackageMarkupQuickLink" || linkElement.Name == "home:ExtensiblityPackageQuickLink" || linkElement.Name == "home:DualPackageMarkupQuickLink")
                            quickLink = (IQuickLink)new Windows7PackageQuickLink(this.Manager, xmlElement1);
                        else if (linkElement.Name == "home:BroadbandPromoQuicklink")
                            quickLink = (IQuickLink)new BroadbandPromoQuickLink(this.Manager, xmlElement1);
                        else if (linkElement.Name == "home:FavoritePartnerQuicklink")
                        {
                            quickLink = (IQuickLink)new FavouritePartnerQuickLink(this.Manager, xmlElement1);
                        }
                        else
                        {
                            XmlElement xmlElement2 = Enumerable.FirstOrDefault<XmlElement>(Enumerable.OfType<XmlElement>((IEnumerable)linkElement.ChildNodes), (Func<XmlElement, bool>)(o => o.Name == "Command"));
                            quickLink = xmlElement2 == null || !(xmlElement2.FirstChild.Name == "home:LaunchExtensibilityEntryPointCommand") ? (IQuickLink)new Windows7QuickLink(this.Manager, xmlElement1) : (IQuickLink)new Windows7PartnerQuickLink(this.Manager, xmlElement1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Failed to load link. Exception is:\n{0}\nXml for element is:\n{1}", (object)((object)ex).ToString(), (object)xmlElement1.OuterXml);
                    }
                    if (quickLink != null)
                    {
                        quickLink.BeginInit();
                        XmlQuickLink xmlQuickLink = quickLink as XmlQuickLink;
                        if (xmlQuickLink != null)
                        {
                            int result = int.MaxValue;
                            int.TryParse(xmlQuickLink.LinkElement.GetAttribute("Priority"), out result);
                            xmlQuickLink.Priority = result;
                        }
                        quickLink.IsEnabled = flag;
                        this.QuickLinks.Add(quickLink);
                        quickLink.EndInit();
                    }
                }
            }
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
                    catch (Exception)
                    {
                    }
                }
                bool flag = true;
                Windows7PartnerQuickLink partnerQuickLink = xmlQuickLink as Windows7PartnerQuickLink;
                if (partnerQuickLink != null)
                    flag = partnerQuickLink.OemQuickLink != null;
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
            if (this.Title != this.m_originalTitle)
                this.StartMenuCategoryNode.Attributes["Description"].Value = this.Title;
            base.Save(ehres);
        }

        private XmlElement GetStartMenuCategoryElement()
        {
            XmlNodeList elementsByTagName = this.MenuStripDocument.GetElementsByTagName("home:StartMenuCategory");
            if (elementsByTagName.Count > 0)
                return (XmlElement)elementsByTagName[0];
            else
                return (XmlElement)null;
        }

        protected override void OnPriorityChanged(int oldValue, int newValue)
        {
            XmlElement menuCategoryElement = this.GetStartMenuCategoryElement();
            if (menuCategoryElement == null)
                return;
            menuCategoryElement.SetAttribute("Priority", newValue.ToString());
        }
    }
}
