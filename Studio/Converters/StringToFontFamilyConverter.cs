using Advent.Common.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Advent.VmcStudio.Converters
{
    internal class StringToFontFamilyConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FontFamiliesProperty = DependencyProperty.Register("FontFamilies", typeof(IEnumerable<FontFamily>), typeof(StringToFontFamilyConverter), new PropertyMetadata((object)new List<FontFamily>()));

        public IEnumerable<FontFamily> FontFamilies
        {
            get
            {
                return (IEnumerable<FontFamily>)this.GetValue(StringToFontFamilyConverter.FontFamiliesProperty);
            }
            set
            {
                this.SetValue(StringToFontFamilyConverter.FontFamiliesProperty, (object)value);
            }
        }

        static StringToFontFamilyConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fontFamilyName = (string)value;
            XmlLanguage lang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            return (object)(Enumerable.FirstOrDefault<FontFamily>(this.FontFamilies, (Func<FontFamily, bool>)(o =>
            {
                if (o.FamilyNames.ContainsKey(lang))
                    return o.FamilyNames[lang] == fontFamilyName;
                else
                    return false;
            })) ?? Enumerable.FirstOrDefault<FontFamily>(this.FontFamilies, (Func<FontFamily, bool>)(o => FontUtil.GetName(o) == fontFamilyName)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fontFamily = (FontFamily)value;
            XmlLanguage language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            string name;
            if (!fontFamily.FamilyNames.TryGetValue(language, out name))
                name = FontUtil.GetName(fontFamily);
            return (object)name;
        }
    }
}
