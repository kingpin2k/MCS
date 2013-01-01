


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class Windows7QuickLink : Windows7QuickLinkBase
    {
        private string m_originalTitle;

        public override bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public Windows7QuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }

        protected override void Load()
        {
            string attribute = this.LinkElement.GetAttribute("Description");
            this.Title = MediaCenterUtil.GetStringResource(this.Manager.Resources, attribute, false) ?? attribute;
            this.m_originalTitle = this.Title;
            XmlNode firstChild = this.LinkElement.FirstChild;
            if (firstChild == null || !(firstChild.Name == "Image") || firstChild.FirstChild == null)
                return;
            ImageSet imageSet = ImageSet.FromXml(this.Manager.Resources, firstChild.FirstChild as XmlElement);
            this.SetValue(XmlQuickLink.ImageProperty, (object)imageSet["Focus"]);
            this.SetValue(XmlQuickLink.NonFocusImageProperty, (object)imageSet["Default"]);
        }

        internal override void Save(IResourceLibrary ehres)
        {
            if (this.Title != this.m_originalTitle)
                this.LinkElement.SetAttribute("Description", this.Title);
            base.Save(ehres);
        }
    }
}
