


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Fiji
{
    internal class FijiQuickLink : XmlQuickLink
    {
        private string originalTitle;

        public override bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public FijiQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }

        internal override void Save(IResourceLibrary ehres)
        {
            if (this.Title != this.originalTitle)
                this.XmlElement.SetAttribute("Description", this.Title);
            base.Save(ehres);
        }

        protected override void Load()
        {
            string attribute = this.XmlElement.GetAttribute("Description");
            this.Title = MediaCenterUtil.GetStringResource(this.Manager.Resources, attribute, false) ?? attribute;
            this.originalTitle = this.Title;
            XmlNode firstChild = this.XmlElement.FirstChild;
            if (firstChild == null || !(firstChild.Name == "Image") || firstChild.FirstChild == null)
                return;
            ImageSet imageSet = ImageSet.FromXml(this.Manager.Resources, firstChild.FirstChild as XmlElement);
            this.SetValue(XmlQuickLink.ImageProperty, (object)imageSet["Focus"]);
            this.SetValue(XmlQuickLink.NonFocusImageProperty, (object)imageSet["Default"]);
        }
    }
}
