using Advent.MediaCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Advent.MediaCenter.Mcml
{
    public class PropertiesElement : McmlElement
    {
        internal PropertiesElement(XElement element, McmlDocument document)
            : base(element, document)
        {
        }

        public IEnumerable<PropertyElement> ColorProperties()
        {
            return Enumerable.Select<XElement, PropertyElement>(Enumerable.Where<XElement>(this.Xml.Elements(), (Func<XElement, bool>)(o => o.Name == this.Document.DefaultNamespace + "Color")), (Func<XElement, PropertyElement>)(o => this.CreatePropertyElement(o)));
        }

        public IEnumerable<FontElement> FontProperties()
        {
            return Enumerable.Select<XElement, FontElement>(Enumerable.Where<XElement>(this.Xml.Elements(), (Func<XElement, bool>)(o => o.Name == this.Document.DefaultNamespace + "Font")), (Func<XElement, FontElement>)(o => new FontElement(o, this.Document)));
        }

        public PropertyElement GetProperty(string propertyName)
        {
            XElement propertyElement = Enumerable.FirstOrDefault<XElement>(this.Xml.Elements(), (Func<XElement, bool>)(o => MediaCenterUtil.AttributeValue(o, (XName)"Name") == propertyName));
            if (propertyElement != null)
                return this.CreatePropertyElement(propertyElement);
            else
                return (PropertyElement)null;
        }

        private PropertyElement CreatePropertyElement(XElement propertyElement)
        {
            if (propertyElement.Name == this.Document.DefaultNamespace + "Font")
                return (PropertyElement)new FontElement(propertyElement, this.Document);
            else
                return new PropertyElement(propertyElement, this.Document);
        }
    }
}
