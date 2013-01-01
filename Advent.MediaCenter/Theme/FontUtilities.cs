


using Advent.Common.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Advent.MediaCenter.Theme
{
    internal static class FontUtilities
    {
        internal static string GetTypefaceName(Typeface typeface)
        {
            XmlLanguage language = XmlLanguage.GetLanguage("en-us");
            Typeface typeface1 = new Typeface(typeface.FontFamily, FontStyles.Normal, typeface.Weight, FontStretches.Normal);
            string str = typeface1.FontFamily.FamilyNames[language];
            if (typeface1.Weight == FontWeights.Bold)
                str = str + " Bold";
            else if (typeface1.Weight == FontWeights.SemiBold)
                str = str + " Semibold";
            else if (typeface1.Weight == FontWeights.Light)
                str = str + " Light";
            return str;
        }

        internal static string GetTypefaceName(MediaCenterTheme theme, FontFace typeface)
        {
            return FontUtilities.GetTypefaceName(FontUtilities.GetTypeface(theme, typeface));
        }

        internal static Typeface GetTypeface(MediaCenterTheme theme, FontFace info)
        {
            FontFamily fontFamily = theme.GetFontFamily(info.FontFamily);
            if (fontFamily == null)
                return (Typeface)null;
            else
                return new Typeface(fontFamily, FontStyles.Normal, info.FontWeight, FontStretches.Normal);
        }

        public static FontFace GetFontFaceInfo(string font, MediaCenterTheme theme)
        {
            FontFace fontFace = new FontFace();
            List<FontFamily> list = new List<FontFamily>();
            if (theme != null)
                list.AddRange((IEnumerable<FontFamily>)theme.Fonts);
            list.AddRange((IEnumerable<FontFamily>)Fonts.SystemFontFamilies);
            list.Reverse();
            foreach (FontFamily fontFamily in list)
            {
                string name = FontUtil.GetName(fontFamily);
                if (font.StartsWith(name))
                {
                    fontFace.FontFamily = name;
                    FontWeightConverter fontWeightConverter = new FontWeightConverter();
                    string str = font.Substring(name.Length).Trim();
                    char[] chArray = new char[1]
          {
            ' '
          };
                    foreach (string text in str.Split(chArray))
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            try
                            {
                                fontFace.FontWeight = (FontWeight)fontWeightConverter.ConvertFromString(text);
                            }
                            catch (FormatException)
                            {
                            }
                        }
                    }
                    break;
                }
            }
            if (fontFace.FontFamily == null)
                return (FontFace)null;
            else
                return fontFace;
        }
    }
}
