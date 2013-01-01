using Advent.MediaCenter;
using System.Xml.Linq;

namespace Advent.MediaCenter.Mcml
{
    public class PropertyElement : McmlElement
    {
        public string Name
        {
            get
            {
                return MediaCenterUtil.AttributeValue(this.Xml, (XName)"Name");
            }
        }

        public virtual string Value
        {
            get
            {
                return MediaCenterUtil.AttributeValue(this.Xml, (XName)this.Xml.Name.LocalName);
            }
            set
            {
                this.Xml.SetAttributeValue((XName)this.Xml.Name.LocalName, (object)value);
            }
        }

        internal PropertyElement(XElement element, McmlDocument document)
            : base(element, document)
        {
        }
    }
}
