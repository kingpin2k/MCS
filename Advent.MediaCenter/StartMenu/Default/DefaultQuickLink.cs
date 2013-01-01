


using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.StartMenu;
using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Default
{
    internal class DefaultQuickLink : XmlQuickLink
    {
        private string originalTitle;
        private ImageSet images;
        private string originalStripID;
        private DefaultMenuStrip originalStrip;

        public override bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public override IMenuStrip OriginalStrip
        {
            get
            {
                if (this.originalStrip == null)
                {
                    foreach (IMenuStrip menuStrip in (Collection<IMenuStrip>)this.Manager.Strips)
                    {
                        DefaultMenuStrip defaultMenuStrip = menuStrip as DefaultMenuStrip;
                        if (defaultMenuStrip != null && defaultMenuStrip.StripID == this.originalStripID)
                            this.originalStrip = defaultMenuStrip;
                    }
                }
                return (IMenuStrip)this.originalStrip;
            }
        }

        public DefaultQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }

        internal override void Save(IResourceLibrary ehres)
        {
            if (this.IsEnabled)
                this.XmlElement.RemoveAttribute("Visible");
            else
                this.XmlElement.SetAttribute("Visible", "False");
            if (this.originalTitle != this.Title)
                this.XmlElement.SetAttribute("Title", this.Title);
            base.Save(ehres);
        }

        protected override void Load()
        {
            string attribute1 = this.XmlElement.GetAttribute("Title");
            int stringResourceId = MediaCenterUtil.GetMagicStringResourceID(attribute1);
            if (stringResourceId >= 0)
            {
                string stringResource = ResourceExtensions.GetStringResource(this.Manager.Resources["ehres.dll"], stringResourceId);
                if (stringResource != null)
                    this.originalTitle = stringResource;
            }
            if (this.originalTitle == null)
                this.originalTitle = attribute1;
            this.Title = this.originalTitle;
            string attribute2 = this.XmlElement.GetAttribute("Icon");
            this.images = ((DefaultStartMenuManager)this.Manager).GetImages(attribute2);
            if (this.images == null)
                throw new ApplicationException("Could not find image set \"" + attribute2 + "\".");
            this.SetValue(XmlQuickLink.ImageProperty, (object)this.images["Focus"]);
            this.SetValue(XmlQuickLink.NonFocusImageProperty, (object)this.images["Default"]);
            string attribute3 = this.XmlElement.GetAttribute("Visible");
            bool result;
            this.IsEnabled = string.IsNullOrEmpty(attribute3) || bool.TryParse(attribute3, out result) && result;
            this.originalStripID = this.XmlElement.GetAttribute("AppId");
            this.originalStrip = (DefaultMenuStrip)null;
        }
    }
}
