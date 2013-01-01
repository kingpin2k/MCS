using Advent.MediaCenter;
using System.Xml.Linq;

namespace Advent.MediaCenter.Mcml
{
    public class FontElement : PropertyElement
    {
        public override string Value
        {
            get
            {
                return MediaCenterUtil.AttributeValue(this.Xml, (XName)"FontName");
            }
            set
            {
                this.Xml.SetAttributeValue((XName)"FontName", (object)value);
            }
        }

        public string Size
        {
            get
            {
                return MediaCenterUtil.AttributeValue(this.Xml, (XName)"FontSize");
            }
            set
            {
                this.Xml.SetAttributeValue((XName)"FontSize", (object)value);
            }
        }

        public string Style
        {
            get
            {
                return MediaCenterUtil.AttributeValue(this.Xml, (XName)"FontStyle");
            }
            set
            {
                this.Xml.SetAttributeValue((XName)"FontStyle", (object)value);
            }
        }

        internal FontElement(XElement element, McmlDocument document)
            : base(element, document)
        {
        }
    }
}
