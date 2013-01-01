


using Advent.MediaCenter.StartMenu;
using System.Collections.Generic;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Fiji
{
    internal class FijiPartnerQuickLink : PartnerQuickLink
    {
        private bool removeIfEmpty;

        public bool RemoveIfEmpty
        {
            get
            {
                return this.removeIfEmpty;
            }
        }

        protected override string[] Categories
        {
            get
            {
                List<string> list = new List<string>();
                string attribute = this.XmlElement.GetAttribute("ExtensibilityCategory");
                if (string.IsNullOrEmpty(attribute))
                {
                    foreach (XmlNode xmlNode1 in this.XmlElement.ChildNodes)
                    {
                        if (xmlNode1.Name == "ExtensibilityCategories")
                        {
                            foreach (XmlNode xmlNode2 in xmlNode1.ChildNodes)
                            {
                                string str = xmlNode2.Attributes["String"].Value;
                                if (!string.IsNullOrEmpty(str))
                                    list.Add(str);
                            }
                        }
                    }
                }
                else
                    list.Add(attribute);
                return list.ToArray();
            }
        }

        public FijiPartnerQuickLink(StartMenuManager manager, XmlElement element)
            : this(manager, element, false)
        {
        }

        public FijiPartnerQuickLink(StartMenuManager manager, XmlElement element, bool removeIfEmpty)
            : base(manager, element)
        {
            this.removeIfEmpty = removeIfEmpty;
        }
    }
}
