using Advent.MediaCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Advent.MediaCenter.Mcml
{
    public class McmlDocument
    {
        internal static readonly XNamespace SystemCoreNamespace = (XNamespace)"assembly://MsCorLib/System";

        public XDocument Xml { get; private set; }

        internal XNamespace DefaultNamespace
        {
            get
            {
                return MediaCenterUtil.GetNamespace(this.Xml, "xmlns");
            }
        }

        static McmlDocument()
        {
        }

        public McmlDocument(XDocument doc)
        {
            this.Xml = doc;
        }

        public PropertiesElement Properties()
        {
            return new PropertiesElement(this.Xml.Root, this);
        }

        public PropertiesElement GetUIProperties(string name)
        {
            UIElement uiElement = this.UIElement(name);
            if (uiElement != null)
                return uiElement.Properties();
            else
                return (PropertiesElement)null;
        }

        public UIElement UIElement(string name)
        {
            return Enumerable.FirstOrDefault<UIElement>(this.UIElements(), (Func<UIElement, bool>)(o => o.Name == name));
        }

        public IEnumerable<UIElement> UIElements()
        {
            return Enumerable.Select<XElement, UIElement>(this.Xml.Root.Elements(this.DefaultNamespace + "UI"), (Func<XElement, UIElement>)(o => new UIElement(o, this)));
        }
    }
}
