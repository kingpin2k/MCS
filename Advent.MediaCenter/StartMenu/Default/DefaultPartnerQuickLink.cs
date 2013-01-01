


using Advent.MediaCenter.StartMenu;
using System.Xml;

namespace Advent.MediaCenter.StartMenu.Default
{
    internal class DefaultPartnerQuickLink : PartnerQuickLink
    {
        protected override string[] Categories
        {
            get
            {
                switch (this.XmlElement.GetAttribute("EntryPointId").ToUpper())
                {
                    case "[MUSICPARTNEREP]":
                        return new string[2]
            {
              "Services\\Audio",
              "Services\\Radio"
            };
                    case "[PICTURESPARTNEREP]":
                        return new string[1]
            {
              "Services\\Pictures"
            };
                    case "[TVPARTNEREP]":
                        return new string[2]
            {
              "Services\\TV",
              "Services\\Movies"
            };
                    case "[ACTIVITIESPARTNEREP]":
                        return new string[1]
            {
              "Services\\Activities"
            };
                    case "[SPORTSPARTNEREP]":
                        return new string[2]
            {
              "Services\\Sports",
              "Services\\News"
            };
                    case "[SPOTLIGHTPROMO1EP]":
                        return new string[1]
            {
              "Spotlight1"
            };
                    default:
                        return new string[0];
                }
            }
        }

        public DefaultPartnerQuickLink(StartMenuManager manager, XmlElement element)
            : base(manager, element)
        {
        }
    }
}
