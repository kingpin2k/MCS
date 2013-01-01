// Type: Advent.MediaCenter.Theme.Default.ColorsApplicator



using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.Mcml;
using Advent.MediaCenter.Theme;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Xml.Linq;

namespace Advent.MediaCenter.Theme.Default
{
    internal class ColorsApplicator : ColorsThemeItem.IColorsThemeItemApplicator, IThemeItemApplicator
    {
        protected virtual string ResourceLibraryName
        {
            get
            {
                return "Microsoft.MediaCenter.Shell.dll";
            }
        }

        protected virtual string ColorsDocumentName
        {
            get
            {
                return "COLORS.MCML";
            }
        }

        protected virtual int ColorsDocumentResourceType
        {
            get
            {
                //This was 10, but I'm pretty sure it's 23
                return 23;
            }
        }

        public void Apply(IThemeItem themeItem, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            ColorsThemeItem colorsThemeItem = (ColorsThemeItem)themeItem;
            if (colorsThemeItem.DefaultColors.Count <= 0)
                return;
            IResourceLibrary resourceLibrary1 = readCache[this.ResourceLibraryName];
            IResourceLibrary resourceLibrary2 = writeCache[this.ResourceLibraryName];
            McmlDocument mcml = MediaCenterUtil.GetMcml(resourceLibrary1.GetResource(this.ColorsDocumentName, (object)this.ColorsDocumentResourceType));
            foreach (ColorItem colorItem in (Collection<ColorItem>)colorsThemeItem.DefaultColors)
            {
                string str = colorItem.ToString();
                PropertyElement property = mcml.Properties().GetProperty(colorItem.Name);
                if (property == null)
                {
                    XElement xelement = new XElement(mcml.DefaultNamespace + "Color", new object[2]
          {
            (object) new XAttribute((XName) "Name", (object) colorItem.Name),
            (object) new XAttribute((XName) "Color", (object) str)
          });
                    mcml.Xml.Root.Add((object)xelement);
                }
                else
                    property.Value = str;
            }
            MediaCenterUtil.UpdateMcml(resourceLibrary2.GetResource(this.ColorsDocumentName, (object)this.ColorsDocumentResourceType), mcml);
        }

        public IEnumerable<ColorItem> GetColors(MediaCenterLibraryCache cache)
        {
            McmlDocument mcml = MediaCenterUtil.GetMcml(cache[this.ResourceLibraryName].GetResource(this.ColorsDocumentName, (object)this.ColorsDocumentResourceType));
            List<ColorItem> list = new List<ColorItem>();
            foreach (PropertyElement propertyElement in mcml.Properties().ColorProperties())
            {
                string text = propertyElement.Value;
                Color color;
                if (MediaCenterUtil.TryParseColor(text, out color))
                    list.Add(new ColorItem()
                    {
                        Color = color,
                        Name = propertyElement.Name
                    });
                else
                    Trace.TraceWarning("Could not parse color \"{0}\" from COLORS.MCML.", new object[1]
          {
            (object) text
          });
            }
            return (IEnumerable<ColorItem>)list;
        }
    }
}
