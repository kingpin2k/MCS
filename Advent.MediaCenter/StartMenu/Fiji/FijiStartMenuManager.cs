


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

namespace Advent.MediaCenter.StartMenu.Fiji
{
    internal class FijiStartMenuManager : StartMenuManager
    {
        private const string StartMenuCategoryPrefix = "global://";
        private List<FijiMenuStrip> deletedStrips;

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
                return this.CustomStripCount + 3;
            }
        }

        protected override XmlNode StripParentNode
        {
            get
            {
                XmlNodeList elementsByTagName = this.StartMenuDocument.GetElementsByTagName("Applications");
                if (elementsByTagName.Count > 0)
                    return elementsByTagName[0];
                else
                    throw new InvalidOperationException("Could not find start menu applications node.");
            }
        }

        public FijiStartMenuManager(IResourceLibraryCache cache, OemManager oemManager)
            : base(cache, oemManager)
        {
            this.deletedStrips = new List<FijiMenuStrip>();
            this.Strips.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Strips_CollectionChanged);
        }

        protected override IMenuStrip CreateMenuStrip(XmlNode node, IResourceLibrary ehres)
        {
            XmlElement xmlElement = node as XmlElement;
            bool flag = true;
            if (xmlElement == null)
            {
                XmlComment comment = node as XmlComment;
                if (comment != null)
                {
                    xmlElement = MediaCenterUtil.UncommentElement(comment);
                    flag = xmlElement == null;
                }
            }
            IMenuStrip menuStrip = (IMenuStrip)null;
            if (xmlElement != null)
            {
                if (xmlElement.Name == "home:NowPlayingApp")
                    menuStrip = (IMenuStrip)new NowPlayingStrip((StartMenuManager)this, xmlElement);
                else if (!(xmlElement.Name == "home:MSOApp"))
                {
                    string attribute1 = xmlElement.GetAttribute("StartMenuCategory");
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
                                    FijiMenuStrip fijiMenuStrip = new FijiMenuStrip((StartMenuManager)this, doc, xmlElement, resourceName);
                                    fijiMenuStrip.StartMenuNamespace = str;
                                    fijiMenuStrip.IsEnabled = flag;
                                    menuStrip = (IMenuStrip)fijiMenuStrip;
                                }
                                else
                                    Trace.TraceWarning("Strip {0} points to null resource: {1}", (object)xmlElement.Name, (object)attribute2);
                            }
                            else
                                Trace.TraceWarning("Could not find namespace \"{0}\". Element: {1}", (object)str, (object)xmlElement.OuterXml);
                        }
                        else
                            Trace.TraceWarning("Invalid StartMenuCategory value \"{0}\". Element: {1}", (object)attribute1, (object)xmlElement.OuterXml);
                    }
                    else
                        Trace.TraceWarning("Unknown application element: {0}", new object[1]
            {
              (object) xmlElement.OuterXml
            });
                }
            }
            int result;
            if (menuStrip != null && int.TryParse(xmlElement.GetAttribute("Priority"), out result))
                menuStrip.Priority = result;
            return menuStrip;
        }

        protected override void SaveInternal(IResourceLibrary ehres)
        {
            MediaCenterUtil.StripChildComments(this.StripParentNode);
            foreach (FijiMenuStrip fijiMenuStrip in this.deletedStrips)
            {
                XmlElement startMenuElement = fijiMenuStrip.StartMenuElement;
                if (startMenuElement.ParentNode != null)
                    startMenuElement.ParentNode.RemoveChild((XmlNode)startMenuElement);
                this.StartMenuDocument.DocumentElement.RemoveAttribute(string.Format("xmlns:{0}", (object)fijiMenuStrip.StartMenuNamespace));
                ResourceExtensions.Update(ehres.GetResource(fijiMenuStrip.DocumentResourceName, (object)23), new byte[0]);
            }
            this.deletedStrips.Clear();
            base.SaveInternal(ehres);
        }

        protected override XmlNode GetNodeForSave(BaseXmlMenuStrip strip)
        {
            FijiMenuStrip fijiMenuStrip = strip as FijiMenuStrip;
            if (fijiMenuStrip == null || fijiMenuStrip.IsEnabled)
                return base.GetNodeForSave(strip);
            else
                return (XmlNode)strip.StartMenuElement.OwnerDocument.CreateComment(strip.StartMenuElement.OuterXml);
        }

        protected override IMenuStrip CreateCustomStrip()
        {
            XmlElement element = this.StartMenuDocument.CreateElement("home", "McmlApplication", this.StartMenuDocument.DocumentElement.GetAttribute("xmlns:home"));
            int num = 100;
            element.SetAttribute("Priority", num.ToString());
            string str1 = Guid.NewGuid().ToString();
            element.SetAttribute("AppID", str1);
            string str2 = string.Format("m{0}", (object)str1);
            element.SetAttribute("StartMenuCategory", string.Format("global://{0}:MenuMenderSMC", (object)str2));
            string resourceName = string.Format("SM.MenuMender.{0}.xml", (object)str1);
            XmlAttribute attribute = this.StartMenuDocument.CreateAttribute(string.Format("xmlns:{0}", (object)str2));
            attribute.Value = string.Format("res://ehres!{0}", (object)resourceName);
            this.StartMenuDocument.DocumentElement.Attributes.Append(attribute);
            XmlDocument doc = new XmlDocument();
            
            doc.LoadXml(string.Format(Advent.MediaCenter.Resources.FijiMenuStripTemplate, (object)"Custom menu", (object)str1));
            FijiMenuStrip fijiMenuStrip = new FijiMenuStrip((StartMenuManager)this, doc, element, resourceName);
            fijiMenuStrip.BeginInit();
            fijiMenuStrip.StartMenuNamespace = str2;
            fijiMenuStrip.Priority = num;
            fijiMenuStrip.EndInit();
            return (IMenuStrip)fijiMenuStrip;
        }

        private void Strips_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (IMenuStrip menuStrip in (IEnumerable)e.OldItems)
                {
                    FijiMenuStrip fijiMenuStrip = menuStrip as FijiMenuStrip;
                    if (fijiMenuStrip != null)
                        this.deletedStrips.Add(fijiMenuStrip);
                }
            }
            if (e.NewItems == null)
                return;
            foreach (IMenuStrip menuStrip in (IEnumerable)e.NewItems)
            {
                FijiMenuStrip fijiMenuStrip = menuStrip as FijiMenuStrip;
                if (fijiMenuStrip != null)
                    this.deletedStrips.Remove(fijiMenuStrip);
            }
        }

        protected override bool IsOemPlaceholderElement(XmlElement element, out int index)
        {
            if (element.Name == "home:OEMPlaceholder")
            {
                if (int.TryParse(element.GetAttribute("Index"), out index))
                    return true;
                Trace.WriteLine("Could not parse OEM strip element:\n" + element.OuterXml);
                index = -1;
                return true;
            }
            else
            {
                index = -1;
                return false;
            }
        }

        protected override XmlNode CreateOemStripNode(OemMenuStrip strip, int index)
        {
            XmlElement element = this.StartMenuDocument.CreateElement("home", "OEMPlaceholder", this.StartMenuDocument.DocumentElement.GetAttribute("xmlns:home"));
            element.SetAttribute("Index", index.ToString());
            element.SetAttribute("MaxQuickLinks", strip == null ? "100" : strip.QuickLinks.Count.ToString());
            element.SetAttribute("Priority", strip == null ? "100" : strip.Priority.ToString());
            return (XmlNode)element;
        }
    }
}
