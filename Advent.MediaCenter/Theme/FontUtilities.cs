using Advent.Common.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;

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
                list.AddRange(theme.Fonts);

            InstallMediaCenterFonts();

            list.AddRange(Fonts.SystemFontFamilies);

            foreach (FontFamily fontFamily in list)
            {
                string name = FontUtil.GetName(fontFamily);
                if (font.StartsWith(name))
                {
                    fontFace.FontFamily = name;
                    FontWeightConverter fontWeightConverter = new FontWeightConverter();
                    string str = font.Substring(name.Length).Trim();
                    char[] chArray = new char[1] { ' ' };
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

        /// <summary>
        /// Install the fonts associated with Media Center by default
        /// I realize I should use a private font collection, but it's a pain to 
        /// use with WPF etc...  So I install and say, not my problem fool
        /// </summary>
        private static void InstallMediaCenterFonts()
        {
            List<FontFamily> local_ttfs = new List<FontFamily>();
            var font_files = Directory.GetFiles(MediaCenterUtil.MediaCenterPath, "*.ttf");

            foreach (var font_file in font_files)
            {
                var result = FontUtil.InstallFont(font_file);

                var error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    Trace.TraceWarning((new Win32Exception(error)).Message);
                }
                else
                {
                    Trace.TraceWarning((result == 0) ? "Font is already installed." :
                                                      "Font installed successfully.");
                }
            }
        }
    }
}
