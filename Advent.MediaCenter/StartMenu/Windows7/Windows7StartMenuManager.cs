


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class Windows7StartMenuManager : StartMenuManager
    {
        private const string START_MENU_CATEGORY_PREFIX = "global://";
        private readonly List<Windows7MenuStrip> m_deletedStrips;

        public override int MaxCustomStripCount
        {
            get
            {
                return int.MaxValue;
            }
        }

        public override int MinCustomStripCount
        {
            get
            {
                return Math.Min(20, this.CustomStripCount + 3);
            }
        }

        protected override XmlNode StripParentNode
        {
            get
            {
                XmlNodeList elementsByTagName = this.StartMenuDocument.GetElementsByTagName("Categories");
                if (elementsByTagName.Count > 0)
                    return elementsByTagName[0].FirstChild.FirstChild;
                else
                    throw new InvalidOperationException("Could not find start menu applications node.");
            }
        }

        public Windows7StartMenuManager(IResourceLibraryCache cache, OemManager oemManager)
            : base(cache, oemManager)
        {
            this.m_deletedStrips = new List<Windows7MenuStrip>();
            this.Strips.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Strips_CollectionChanged);
        }

        public override void Save(IResourceLibraryCache cache, bool forceSave)
        {
            base.Save(cache, forceSave);
        }

        protected override void UpdateOemStripCount(int minCustomStrips)
        {
        }

        private void Strips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (IMenuStrip menuStrip in (IEnumerable)e.OldItems)
                {
                    Windows7MenuStrip windows7MenuStrip = menuStrip as Windows7MenuStrip;
                    if (windows7MenuStrip != null)
                        this.m_deletedStrips.Add(windows7MenuStrip);
                }
            }
            if (e.NewItems == null)
                return;
            foreach (IMenuStrip menuStrip in (IEnumerable)e.NewItems)
            {
                Windows7MenuStrip windows7MenuStrip = menuStrip as Windows7MenuStrip;
                if (windows7MenuStrip != null)
                    this.m_deletedStrips.Remove(windows7MenuStrip);
            }
        }

        protected override IMenuStrip CreateMenuStrip(XmlNode node, IResourceLibrary ehres)
        {
            XmlElement startMenuTargetElement = node as XmlElement;
            bool flag = true;
            if (startMenuTargetElement == null)
            {
                XmlComment comment = node as XmlComment;
                if (comment != null)
                {
                    startMenuTargetElement = MediaCenterUtil.UncommentElement(comment);
                    flag = startMenuTargetElement == null;
                }
            }
            IMenuStrip menuStrip = (IMenuStrip)null;
            if (startMenuTargetElement != null)
            {
                XmlElement xmlElement = startMenuTargetElement;
                if (startMenuTargetElement.Name == "home:ConstrainedCategory" || startMenuTargetElement.Name == "constraints:ConstrainedItem")
                    startMenuTargetElement = (XmlElement)startMenuTargetElement.FirstChild.FirstChild;
                if (startMenuTargetElement.Name == "home:NowPlayingStartMenuCategory")
                    menuStrip = (IMenuStrip)new NowPlayingStrip((StartMenuManager)this, xmlElement);
                else if (!(startMenuTargetElement.Name == "home:MSOStartMenuCategory"))
                {
                    string attribute1 = startMenuTargetElement.GetAttribute("StartMenuCategory");
                    if (!string.IsNullOrEmpty(attribute1) && attribute1.StartsWith("global://"))
                    {
                        int num = attribute1.IndexOf(':', "global://".Length);
                        if (num >= 0)
                        {
                            string str = attribute1.Substring("global://".Length, num - "global://".Length);
                            string attribute2 = this.StartMenuDocument.DocumentElement.GetAttribute("xmlns:" + str);
                            if (!string.IsNullOrEmpty(attribute2))
                            {
                                string resourceName;
                                XmlReader xmlResource = MediaCenterUtil.GetXmlResource(this.Resources, attribute2, out resourceName);
                                if (xmlResource != null)
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.Load(xmlResource);
                                    Windows7MenuStrip windows7MenuStrip = new Windows7MenuStrip((StartMenuManager)this, doc, xmlElement, startMenuTargetElement, resourceName);
                                    windows7MenuStrip.StartMenuNamespace = str;
                                    windows7MenuStrip.IsEnabled = flag;
                                    menuStrip = (IMenuStrip)windows7MenuStrip;
                                }
                                else
                                    Trace.TraceWarning("Strip {0} points to null resource: {1}", (object)startMenuTargetElement.Name, (object)attribute2);
                            }
                            else
                                Trace.TraceWarning("Could not find namespace \"{0}\". Element: {1}", (object)str, (object)startMenuTargetElement.OuterXml);
                        }
                        else
                            Trace.TraceWarning("Invalid StartMenuCategory value \"{0}\". Element: {1}", (object)attribute1, (object)startMenuTargetElement.OuterXml);
                    }
                    else
                        Trace.TraceWarning("Unknown application element: {0}", new object[1]
            {
              (object) startMenuTargetElement.OuterXml
            });
                }
            }
            int result;
            if (menuStrip != null && int.TryParse(startMenuTargetElement.GetAttribute("Priority"), out result))
                menuStrip.Priority = result;
            return menuStrip;
        }

        protected override void SaveInternal(IResourceLibrary ehres)
        {
            MediaCenterUtil.StripChildComments(this.StripParentNode);
            foreach (Windows7MenuStrip windows7MenuStrip in this.m_deletedStrips)
            {
                XmlElement startMenuElement = windows7MenuStrip.StartMenuElement;
                if (startMenuElement.ParentNode != null)
                    startMenuElement.ParentNode.RemoveChild((XmlNode)startMenuElement);
                this.StartMenuDocument.DocumentElement.RemoveAttribute(string.Format("xmlns:{0}", (object)windows7MenuStrip.StartMenuNamespace));
                ResourceExtensions.Update(ehres.GetResource(windows7MenuStrip.DocumentResourceName, (object)23), (byte[])null);
            }
            this.m_deletedStrips.Clear();
            base.SaveInternal(ehres);
        }

        protected override XmlNode GetNodeForSave(BaseXmlMenuStrip strip)
        {
            Windows7MenuStrip windows7MenuStrip = strip as Windows7MenuStrip;
            if (windows7MenuStrip == null || windows7MenuStrip.IsEnabled)
                return base.GetNodeForSave(strip);
            else
                return (XmlNode)strip.StartMenuElement.OwnerDocument.CreateComment(strip.StartMenuElement.OuterXml);
        }

        protected override IMenuStrip CreateCustomStrip()
        {
            string str1 = Guid.NewGuid().ToString();
            XmlElement element1 = this.StartMenuDocument.CreateElement("home", "ConstrainedCategory", this.StartMenuDocument.DocumentElement.GetAttribute("xmlns:home"));
            XmlElement element2 = this.StartMenuDocument.CreateElement("Value", this.StartMenuDocument.DocumentElement.GetAttribute("xmlns"));
            XmlElement element3 = this.StartMenuDocument.CreateElement("home", "StartMenuCategory", this.StartMenuDocument.DocumentElement.GetAttribute("xmlns:home"));
            element1.AppendChild((XmlNode)element2);
            element2.AppendChild((XmlNode)element3);
            string str2 = string.Format("m{0}", (object)str1);
            element3.SetAttribute("StartMenuCategory", string.Format("global://{0}:MediaCenterStudioSMC", (object)str2));
            string resourceName = string.Format("SM.MediaCenterStudio.{0}.xml", (object)str1);
            XmlAttribute attribute = this.StartMenuDocument.CreateAttribute(string.Format("xmlns:{0}", (object)str2));
            attribute.Value = string.Format("res://ehres!{0}", (object)resourceName);
            this.StartMenuDocument.DocumentElement.Attributes.Append(attribute);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(string.Format(Advent.MediaCenter.Resources.Windows7MenuStripTemplate, (object)"Custom menu", (object)str1));
            Windows7MenuStrip windows7MenuStrip = new Windows7MenuStrip((StartMenuManager)this, doc, element1, element3, resourceName);
            windows7MenuStrip.BeginInit();
            windows7MenuStrip.StartMenuNamespace = str2;
            windows7MenuStrip.Priority = 100;
            windows7MenuStrip.EndInit();
            return (IMenuStrip)windows7MenuStrip;
        }

        protected override bool IsOemPlaceholderElement(XmlElement element, out int index)
        {
            if (element.Name == "home:ConstrainedCategory")
            {
                XmlElement xmlElement = (XmlElement)element.FirstChild.FirstChild;
                if (xmlElement.Name == "home:ExtensibilityPlaceholderStartMenuCategory")
                {
                    if (int.TryParse(xmlElement.GetAttribute("Index"), out index))
                        return true;
                    Trace.WriteLine("Could not parse OEM strip element:\n" + element.OuterXml);
                    index = -1;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        protected override XmlNode CreateOemStripNode(OemMenuStrip strip, int index)
        {
            string attribute = this.StartMenuDocument.DocumentElement.GetAttribute("xmlns:home");
            XmlElement element1 = this.StartMenuDocument.CreateElement("home", "ConstrainedCategory", attribute);
            XmlElement element2 = this.StartMenuDocument.CreateElement("Value", "http://schemas.microsoft.com/2006/mcml");
            element1.AppendChild((XmlNode)element2);
            XmlElement element3 = this.StartMenuDocument.CreateElement("home", "ExtensibilityPlaceholderStartMenuCategory", attribute);
            element2.AppendChild((XmlNode)element3);
            element3.SetAttribute("Index", index.ToString());
            element3.SetAttribute("Priority", strip == null ? "100" : strip.Priority.ToString());
            return (XmlNode)element1;
        }
    }
}
