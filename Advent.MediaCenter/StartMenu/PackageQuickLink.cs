


using Advent.Common.Interop;
using Advent.Common.IO;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Advent.MediaCenter.StartMenu
{
    internal class PackageQuickLink : XmlQuickLink
    {
        private int titleID;
        private string imageBaseName;
        private string resourceFile;
        private string titleId;

        public override bool CanSetEnabled
        {
            get
            {
                return true;
            }
        }

        public override bool CanEditTitle
        {
            get
            {
                return false;
            }
        }

        public PackageQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }

        internal override void Save(IResourceLibrary ehres)
        {
            if (this.IsValid)
                this.SetTitleId(this.titleId);
            base.Save(ehres);
        }

        protected override void Load()
        {
            string attribute = this.LinkElement.GetAttribute("ResourceProviderId");
            this.titleId = this.GetTitleId();
            int.TryParse(this.titleId, out this.titleID);
            this.imageBaseName = this.LinkElement.GetAttribute("ImageBaseName");
            if (attribute == "Spotlight")
                this.resourceFile = PackageQuickLink.GetPackagePath("MCESpotlight", "MCESpotlight", "SpotlightResources.dll");
            else if (attribute == "NetTV")
            {
                this.resourceFile = PackageQuickLink.GetPackagePath("NetTV", "Browse", "NetTVResources.dll");
            }
            else
            {
                if (!(attribute == "NetOpSM"))
                    throw new ArgumentException("Unknown provider: " + attribute);
                this.resourceFile = PackageQuickLink.GetPackagePath("MCEClientUX", "NetOp", "SMResources.dll");
            }
            if (File.Exists(this.resourceFile))
            {
                using (UnmanagedLibrary unmanagedLibrary = new UnmanagedLibrary(this.resourceFile))
                {
                    this.Title = ResourceExtensions.GetStringResource((IResourceLibrary)unmanagedLibrary, this.titleID);
                    byte[] bytes1 = ResourceExtensions.GetBytes(unmanagedLibrary.GetResource(this.imageBaseName + ".NoFocus.png", (object)10));
                    if (bytes1 != null)
                    {
                        BitmapDecoder bitmapDecoder = BitmapDecoder.Create((Stream)new MemoryStream(bytes1), BitmapCreateOptions.None, BitmapCacheOption.Default);
                        this.SetValue(XmlQuickLink.NonFocusImageProperty, (object)bitmapDecoder.Frames[0]);
                    }
                    byte[] bytes2 = ResourceExtensions.GetBytes(unmanagedLibrary.GetResource(this.imageBaseName + ".Focus.png", (object)10));
                    if (bytes2 != null)
                    {
                        BitmapDecoder bitmapDecoder = BitmapDecoder.Create((Stream)new MemoryStream(bytes2), BitmapCreateOptions.None, BitmapCacheOption.Default);
                        this.SetValue(XmlQuickLink.ImageProperty, (object)bitmapDecoder.Frames[0]);
                    }
                }
            }
            //TODO original (object)(bool)(value ? 1 : 0)
            this.SetValue(XmlQuickLink.IsValidPropertyKey, (object)(string.IsNullOrEmpty(this.Title) ? false : (this.Image != null ? true : false)));
        }

        protected virtual string GetTitleId()
        {
            return this.LinkElement.GetAttribute("TitleId");
        }

        protected virtual void SetTitleId(string titleIDParam)
        {
            this.LinkElement.SetAttribute("TitleId", titleIDParam);
        }

        private static string GetPackagePath(string packageName, string attachmentName, string resourceFileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("Microsoft\\eHome\\Packages\\{0}\\{1}\\{2}", (object)packageName, (object)attachmentName, (object)resourceFileName));
        }
    }
}
