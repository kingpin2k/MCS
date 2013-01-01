


using Advent.MediaCenter.Mcml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace Advent.MediaCenter.Theme
{
    internal static class McmlUtilities
    {
        public static PropertiesElement GetThemeUIProperties(this McmlDocument document, string ui)
        {
            PropertiesElement uiProperties = document.GetUIProperties(ui);
            if (uiProperties == null)
                throw new ThemeApplicationException(string.Format("Could not find UI {0}.", (object)ui));
            else
                return uiProperties;
        }

        public static PropertyElement GetThemeProperty(this PropertiesElement propertiesElement, string name)
        {
            PropertyElement property = propertiesElement.GetProperty(name);
            if (property == null)
                throw new ThemeApplicationException(string.Format("Could not find property {0}.", (object)name));
            else
                return property;
        }

        public static void UpdateColorElement(this ColorReference colorReference, PropertyElement colorElement, MediaCenterTheme theme)
        {
            if (colorReference == null)
                return;
            if (colorElement == null)
                throw new ThemeApplicationException("Color element not found.");
            Color? color = colorReference.GetColor(theme);
            if (!color.HasValue)
                return;
            colorElement.Value = color.ToString();
        }

        public static void UpdateFontElement(this FontOverride fontOverride, FontElement fontElement, MediaCenterTheme theme)
        {
            if (fontOverride == null)
                return;
            if (fontElement == null)
                throw new ThemeApplicationException("Font element not found.");
            FontFace typeface = (FontFace)null;
            if (fontOverride.FontFace != null)
                typeface = fontOverride.FontFace;
            else if (fontOverride.FontClass != null)
            {
                FontClass fontClass = Enumerable.FirstOrDefault<FontClass>((IEnumerable<FontClass>)theme.FontsItem.FontClasses, (Func<FontClass, bool>)(o => o.Name == fontOverride.FontClass));
                if (fontClass != null)
                    typeface = fontClass.FontFace;
                else
                    Trace.TraceWarning("Could not find font class " + fontOverride.FontClass + ".");
            }
            if (typeface == null)
                return;
            string typefaceName = FontUtilities.GetTypefaceName(theme, typeface);
            fontElement.Value = typefaceName;
            if (fontOverride.Size > 0)
                fontElement.Size = fontOverride.Size.ToString();
            string str = (string)null;
            if (fontOverride.IsBold)
                str = "Bold";
            if (fontOverride.IsItalic)
            {
                if (str != null)
                    str = str + ", ";
                str = str + "Italic";
            }
            fontElement.Style = str;
        }
    }
}
