


using Advent.Common.IO;
using Advent.MediaCenter.StartMenu;
using Advent.MediaCenter.StartMenu.OEM;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class Windows7PartnerQuickLink : PartnerQuickLinkBase, IPartnerQuickLink, IQuickLink, ISupportInitialize
    {
        private XmlElement launchElement;

        public override XmlElement LinkElement
        {
            get
            {
                return Windows7QuickLinkBase.GetLinkElement(this.XmlElement);
            }
        }

        public Windows7PartnerQuickLink(StartMenuManager manager, XmlElement element)
            : base(manager, element)
        {
        }

        protected override void Load()
        {
            base.Load();
            this.launchElement = (XmlElement)Enumerable.First<XmlElement>(Enumerable.OfType<XmlElement>((IEnumerable)this.LinkElement.ChildNodes), (Func<XmlElement, bool>)(o => o.Name == "Command")).FirstChild;
            string attribute1 = this.launchElement.GetAttribute("AppId");
            string attribute2 = this.launchElement.GetAttribute("EntryPointId");
            if (string.IsNullOrEmpty(attribute2))
                return;
            EntryPoint entryPoint = this.Manager.OemManager.EntryPoints[attribute2];
            if (entryPoint != null)
            {
                OemQuickLink oemQuickLink = new OemQuickLink(this.Manager.OemManager);
                oemQuickLink.BeginInit();
                oemQuickLink.ApplicationID = attribute1;
                oemQuickLink.EntryPoint = entryPoint;
                oemQuickLink.EndInit();
                this.OemQuickLink = oemQuickLink;
            }
            else
                Trace.TraceWarning("Entry point " + attribute2 + " not found.");
        }

        internal override void Save(IResourceLibrary ehres)
        {
            if (this.OemQuickLink != null)
            {
                OemQuickLink oemQuickLink = this.OemQuickLink;
                this.launchElement.SetAttribute("AppId", oemQuickLink.ApplicationID);
                this.launchElement.SetAttribute("EntryPointId", oemQuickLink.EntryPointID);
                this.LinkElement.SetAttribute("Description", this.OemQuickLink.Title);
                this.LinkElement.SetAttribute("EntryPointId", oemQuickLink.EntryPointID);
                XmlElement xmlElement = (XmlElement)Enumerable.First<XmlElement>(Enumerable.OfType<XmlElement>((IEnumerable)this.LinkElement), (Func<XmlElement, bool>)(o => o.Name == "Image")).FirstChild.FirstChild;
                Enumerable.First<XmlElement>(Enumerable.OfType<XmlElement>((IEnumerable)xmlElement.ChildNodes), (Func<XmlElement, bool>)(o => o.GetAttribute("Name") == "Focus")).SetAttribute("Source", Windows7PartnerQuickLink.GetFileUrl(oemQuickLink.EntryPoint.ImageUrl));
                Enumerable.First<XmlElement>(Enumerable.OfType<XmlElement>((IEnumerable)xmlElement.ChildNodes), (Func<XmlElement, bool>)(o => o.GetAttribute("Name") == "Default")).SetAttribute("Source", Windows7PartnerQuickLink.GetFileUrl(oemQuickLink.EntryPoint.InactiveImageUrl ?? oemQuickLink.EntryPoint.ImageUrl));
            }
            base.Save(ehres);
        }

        private static string GetFileUrl(string file)
        {
            if (file.IndexOf("://") == -1)
                return "file://" + file;
            else
                return file;
        }
    }
}
