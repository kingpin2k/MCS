


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Default
{
    internal class DefaultStartMenuManager : StartMenuManager
    {
        private IDictionary<string, ImageSet> managerImages;

        public override int MaxCustomStripCount
        {
            get
            {
                return 2;
            }
        }

        public override int MinCustomStripCount
        {
            get
            {
                return this.MaxCustomStripCount;
            }
        }

        protected override XmlNode StripParentNode
        {
            get
            {
                return this.StartMenuDocument.DocumentElement.FirstChild.FirstChild;
            }
        }

        public DefaultStartMenuManager(IResourceLibraryCache cache, OemManager oemManager)
            : base(cache, oemManager)
        {
        }

        public ImageSet GetImages(string id)
        {
            if (this.managerImages == null)
            {
                this.managerImages = (IDictionary<string, ImageSet>)new Dictionary<string, ImageSet>();
                string @string = ResourceExtensions.GetString(this.Resources["Microsoft.MediaCenter.Shell.dll"].GetResource("IMAGES.MCML", (object)10), Encoding.UTF8);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(@string);
                XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
                for (int index = 0; index < childNodes.Count; ++index)
                {
                    XmlElement element = childNodes[index] as XmlElement;
                    if (element != null)
                    {
                        ImageSet imageSet = ImageSet.FromXml(this.Resources, element);
                        if (imageSet.Name != null)
                            this.managerImages[imageSet.Name] = imageSet;
                    }
                }
            }
            ImageSet imageSet1;
            this.managerImages.TryGetValue(id, out imageSet1);
            if (imageSet1 == null)
                Trace.TraceWarning("Could not find image set {0}.", new object[1]
        {
          (object) id
        });
            return imageSet1;
        }

        protected override IMenuStrip CreateCustomStrip()
        {
            OemManager oemManager = this.OemManager;
            Application application = new Application();
            application.Title = "Custom menu";
            application.ID = "{" + (object)Guid.NewGuid() + "}";
            oemManager.Applications.Add(application);
            OemMenuStrip oemMenuStrip = new OemMenuStrip();
            oemMenuStrip.Manager = oemManager;
            oemMenuStrip.Application = application;
            oemMenuStrip.Title = application.Title;
            oemMenuStrip.Priority = 100;
            oemMenuStrip.IsEnabled = true;
            string str = this.CustomCategory + "\\Strip " + application.ID;
            oemMenuStrip.Category = str;
            return (IMenuStrip)oemMenuStrip;
        }

        protected override IMenuStrip CreateMenuStrip(XmlNode node, IResourceLibrary ehres)
        {
            IMenuStrip menuStrip = (IMenuStrip)null;
            XmlElement xmlElement = node as XmlElement;
            if (xmlElement != null)
            {
                if (xmlElement.Name == "home:NowPlayingApp")
                    menuStrip = (IMenuStrip)new NowPlayingStrip((StartMenuManager)this, xmlElement);
                else if (!(xmlElement.Name == "home:MSOApp"))
                {
                    string attribute = xmlElement.GetAttribute("Uri");
                    if (!string.IsNullOrEmpty(attribute))
                    {
                        XmlReader xmlResource = MediaCenterUtil.GetXmlResource(ehres, attribute, 23);
                        if (xmlResource != null)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(xmlResource);
                            menuStrip = (IMenuStrip)new DefaultMenuStrip((StartMenuManager)this, doc, xmlElement, attribute);
                        }
                        else
                            Trace.TraceWarning("Strip {0} points to null resource: {1}", (object)xmlElement.Name, (object)attribute);
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
