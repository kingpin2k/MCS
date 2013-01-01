


using Advent.MediaCenter.StartMenu;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Windows7
{
    internal abstract class Windows7QuickLinkBase : XmlQuickLink
    {
        public override XmlElement LinkElement
        {
            get
            {
                return Windows7QuickLinkBase.GetLinkElement(this.XmlElement);
            }
        }

        public Windows7QuickLinkBase(StartMenuManager smm, XmlElement element)
            : base(smm, element)
        {
        }

        internal static XmlElement GetLinkElement(XmlElement constrainedElement)
        {
            if (constrainedElement.Name == "home:ConstrainedQuicklink" || constrainedElement.Name == "constraints:ConstrainedItem")
                return (XmlElement)constrainedElement.GetElementsByTagName("Value")[0].FirstChild;
            else
                return constrainedElement;
        }
    }
}
