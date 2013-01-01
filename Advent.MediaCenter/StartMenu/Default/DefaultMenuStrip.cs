using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Default
{
    internal class DefaultMenuStrip : XmlMenuStrip
    {
        private string originalTitle;

        public string StripID
        {
            get
            {
                return this.AppElement.Attributes["AppId"].Value;
            }
        }

        protected XmlElement AppElement
        {
            get
            {
                return (XmlElement)this.MenuStripDocument.DocumentElement.FirstChild;
            }
        }

        public DefaultMenuStrip(StartMenuManager manager, XmlDocument doc, XmlElement startMenuElement, string resourceName)
            : base(manager, doc, startMenuElement, resourceName)
        {
        }

        public override bool CanSwapWith(IMenuStrip strip)
        {
            if (!base.CanSwapWith(strip))
                return false;
            int num1 = this.Manager.Strips.IndexOf((IMenuStrip)this);
            int num2 = this.Manager.Strips.IndexOf(strip);
            if (num1 == -1 || num2 == -1)
                return false;
            if (num1 < num2)
                return this.CanSwapStrips((IMenuStrip)this, strip);
            else
                return this.CanSwapStrips(strip, (IMenuStrip)this);
        }

        internal override void Save(IResourceLibrary ehres)
        {
            if (this.originalTitle != this.Title)
                this.AppElement.SetAttribute("Title", this.Title);
            if (this.IsEnabled)
                this.AppElement.RemoveAttribute("Visible");
            else
                this.AppElement.SetAttribute("Visible", "False");
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
                this.AppElement.AppendChild((XmlNode)xmlQuickLink.XmlElement);
            }
            base.Save(ehres);
        }

        protected override void Load()
        {
            int resourceID;
            this.Title = MediaCenterUtil.GetMagicString(this.Manager.Resources, this.AppElement.GetAttribute("Title"), out resourceID);
            this.originalTitle = this.Title;
            string attribute = this.AppElement.GetAttribute("Visible");
            this.IsEnabled = string.IsNullOrEmpty(attribute) || Convert.ToBoolean(attribute);
            XmlNodeList childNodes = this.AppElement.ChildNodes;
            for (int index = 0; index < childNodes.Count; ++index)
            {
                XmlElement element = childNodes[index] as XmlElement;
                if (element != null)
                {
                    IQuickLink quickLink = (IQuickLink)null;
                    try
                    {
                        quickLink = !(element.Name == "home:PackageMarkupQuickLink") ? (!(element.Name == "home:PartnerQuickLink") ? (IQuickLink)new DefaultQuickLink(this.Manager, element) : (IQuickLink)new DefaultPartnerQuickLink(this.Manager, element)) : (IQuickLink)new DefaultPackageQuickLink(this.Manager, element);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Failed to load link. Exception is:\n{0}\nXml for element is:\n{0}", (object)((object)ex).ToString(), (object)element.OuterXml);
                    }
                    if (quickLink != null)
                    {
                        quickLink.BeginInit();
                        int result;
                        if (int.TryParse(element.GetAttribute("Priority"), out result))
                        {
                            quickLink.Priority = result;
                            this.QuickLinks.Add(quickLink);
                        }
                        quickLink.EndInit();
                    }
                }
            }
        }

        protected override void OnPriorityChanged(int oldValue, int newValue)
        {
            this.StartMenuTargetElement.SetAttribute("Priority", newValue.ToString());
        }

        private bool CanSwapStrips(IMenuStrip upperStrip, IMenuStrip lowerStrip)
        {
            foreach (IQuickLink quickLink in (Collection<IQuickLink>)lowerStrip.QuickLinks)
            {
                if (quickLink.OriginalStrip == upperStrip)
                    return false;
            }
            return true;
        }
    }
}
