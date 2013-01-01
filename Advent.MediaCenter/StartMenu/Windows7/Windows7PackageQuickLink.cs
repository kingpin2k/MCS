


using Advent.MediaCenter.StartMenu;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal class Windows7PackageQuickLink : PackageQuickLink
    {
        public override XmlElement LinkElement
        {
            get
            {
                return Windows7QuickLinkBase.GetLinkElement(this.XmlElement);
            }
        }

        public Windows7PackageQuickLink(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }
    }
}
