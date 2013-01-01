using Advent.MediaCenter;
using System.Xml.Linq;

namespace Advent.MediaCenter.Mcml
{
    public class UIElement : McmlElement
    {
        public string Name
        {
            get
            {
                return MediaCenterUtil.AttributeValue(this.Xml, (XName)"Name");
            }
        }

        public UIElement(XElement element, McmlDocument document)
            : base(element, document)
        {
        }

        public PropertiesElement Properties()
        {
            XElement element = this.Xml.Element(this.Document.DefaultNamespace + "Properties");
            if (element != null)
                return new PropertiesElement(element, this.Document);
            else
                return (PropertiesElement)null;
        }
    }
}
