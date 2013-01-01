using System.Xml.Linq;

namespace Advent.MediaCenter.Mcml
{
    public class McmlElement
    {
        public McmlDocument Document { get; private set; }

        public XElement Xml { get; private set; }

        internal McmlElement(XElement element, McmlDocument document)
        {
            this.Xml = element;
            this.Document = document;
        }
    }
}
