


using Advent.MediaCenter.StartMenu;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Default
{
    internal class DefaultPackageQuickLink : PackageQuickLink
    {
        public DefaultPackageQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }

        protected override string GetTitleId()
        {
            string str = base.GetTitleId();
            if (string.IsNullOrEmpty(str))
            {
                str = this.XmlElement.GetAttribute("DisabledTitleId");
                this.IsEnabled = false;
            }
            else
                this.IsEnabled = true;
            return str;
        }

        protected override void SetTitleId(string titleId)
        {
            if (this.IsEnabled)
            {
                this.XmlElement.RemoveAttribute("DisabledTitleId");
                this.XmlElement.SetAttribute("TitleId", titleId);
            }
            else
            {
                this.XmlElement.RemoveAttribute("TitleId");
                this.XmlElement.SetAttribute("DisabledTitleId", titleId);
            }
        }
    }
}
