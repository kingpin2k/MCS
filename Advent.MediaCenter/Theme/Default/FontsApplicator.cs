// Type: Advent.MediaCenter.Theme.Default.FontsApplicator



using Advent.Common.IO;
using Advent.MediaCenter;
using Advent.MediaCenter.Mcml;
using Advent.MediaCenter.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Advent.MediaCenter.Theme.Default
{
    internal class FontsApplicator : FontsThemeItem.IFontsThemeItemApplicator, IThemeItemApplicator
    {
        internal const string FontClassRefPrefix = "me";

        protected virtual int DocumentResourceType
        {
            get
            {
                return 10;
            }
        }

        public void Apply(IThemeItem themeItem, MediaCenterLibraryCache readCache, MediaCenterLibraryCache writeCache)
        {
            FontsThemeItem fontsThemeItem = (FontsThemeItem)themeItem;
            IResourceLibrary resourceLibrary1 = readCache["Microsoft.MediaCenter.Shell.dll"];
            IResourceLibrary resourceLibrary2 = writeCache["Microsoft.MediaCenter.Shell.dll"];
            McmlDocument mcml1 = MediaCenterUtil.GetMcml(resourceLibrary1.GetResource("FONTNAMES.MCML", (object)this.DocumentResourceType));
            McmlDocument mcml2 = MediaCenterUtil.GetMcml(resourceLibrary1.GetResource("FONTS.MCML", (object)this.DocumentResourceType));
            foreach (FontClass fontClass in Enumerable.Where<FontClass>((IEnumerable<FontClass>)fontsThemeItem.FontClasses, (Func<FontClass, bool>)(o => o.FontFace != null)))
            {
                string typefaceName = FontUtilities.GetTypefaceName(themeItem.Theme, fontClass.FontFace);
                PropertyElement property = mcml1.Properties().GetProperty(fontClass.Name);
                if (property == null)
                {
                    XElement xelement = new XElement(McmlDocument.SystemCoreNamespace + "String", new object[2]
          {
            (object) new XAttribute((XName) "Name", (object) fontClass.Name),
            (object) new XAttribute((XName) "String", (object) typefaceName)
          });
                    mcml1.Xml.Root.Add((object)xelement);
                }
                else
                    property.Value = typefaceName;
            }
            foreach (XElement xelement in mcml1.Xml.Root.Elements())
                mcml2.Xml.Root.AddFirst((object)xelement);
            MediaCenterUtil.UpdateMcml(resourceLibrary2.GetResource("FONTNAMES.MCML", (object)this.DocumentResourceType), mcml1);
            foreach (FontOverride fontOverride in Enumerable.Where<FontOverride>((IEnumerable<FontOverride>)fontsThemeItem.FontOverrides, (Func<FontOverride, bool>)(o =>
            {
                if (o.FontFace == null)
                    return o.FontClass != null;
                else
                    return true;
            })))
            {
                FontElement fontElement = mcml2.Properties().GetProperty(fontOverride.Name) as FontElement;
                if (fontElement == null)
                {
                    XElement xelement = new XElement(mcml2.DefaultNamespace + "Font", (object)new XAttribute((XName)"Name", (object)fontOverride.Name));
                    mcml2.Xml.Root.Add((object)xelement);
                    fontElement = (FontElement)mcml2.Properties().GetProperty(fontOverride.Name);
                }
                McmlUtilities.UpdateFontElement(fontOverride, fontElement, themeItem.Theme);
            }
            foreach (XNode xnode in mcml2.Xml.Root.Elements(mcml2.DefaultNamespace + "Aggregate"))
                xnode.Remove();
            MediaCenterUtil.UpdateMcml(resourceLibrary2.GetResource("FONTS.MCML", (object)this.DocumentResourceType), mcml2);
        }

        public IEnumerable<FontClass> GetFontClasses(MediaCenterLibraryCache cache)
        {
            XDocument xdocument = XDocument.Load(MediaCenterUtil.GetXml(cache["Microsoft.MediaCenter.Shell.dll"].GetResource("FONTNAMES.MCML", (object)this.DocumentResourceType)));
            List<FontClass> list = new List<FontClass>();
            XNamespace xnamespace = (XNamespace)"assembly://MSCorLib/System";
            foreach (XElement element in xdocument.Root.Elements(xnamespace + "String"))
            {
                FontFace fontFaceInfo = FontUtilities.GetFontFaceInfo(MediaCenterUtil.AttributeValue(element, (XName)"String"), (MediaCenterTheme)null);
                if (fontFaceInfo != null)
                    list.Add(new FontClass()
                    {
                        FontFace = fontFaceInfo,
                        Name = MediaCenterUtil.AttributeValue(element, (XName)"Name")
                    });
            }
            return (IEnumerable<FontClass>)list;
        }

        public IEnumerable<FontOverride> GetFontOverrides(MediaCenterLibraryCache cache, MediaCenterTheme theme)
        {
            XDocument xdocument = XDocument.Load(MediaCenterUtil.GetXml(cache["Microsoft.MediaCenter.Shell.dll"].GetResource("FONTS.MCML", (object)this.DocumentResourceType)));
            string str1 = string.Format("global://{0}:", (object)"me");
            List<FontOverride> list = new List<FontOverride>();
            XNamespace xnamespace = (XNamespace)"http://schemas.microsoft.com/2006/mcml";
            foreach (XElement element in xdocument.Root.Elements(xnamespace + "Font"))
            {
                FontOverride fontOverride = new FontOverride();
                fontOverride.Name = MediaCenterUtil.AttributeValue(element, (XName)"Name");
                string font = MediaCenterUtil.AttributeValue(element, (XName)"FontName");
                if (font.StartsWith(str1))
                    fontOverride.FontClass = font.Substring(str1.Length);
                else
                    fontOverride.FontFace = FontUtilities.GetFontFaceInfo(font, theme);
                if (!string.IsNullOrEmpty(fontOverride.FontClass) || fontOverride.FontFace != null)
                {
                    int result;
                    if (int.TryParse(MediaCenterUtil.AttributeValue(element, (XName)"FontSize"), out result))
                        fontOverride.Size = result;
                    string str2 = MediaCenterUtil.AttributeValue(element, (XName)"FontStyle");
                    if (str2 != null)
                    {
                        fontOverride.IsBold = str2.Contains("Bold");
                        fontOverride.IsItalic = str2.Contains("Italic");
                    }
                    list.Add(fontOverride);
                }
            }
            return (IEnumerable<FontOverride>)list;
        }
    }
}
