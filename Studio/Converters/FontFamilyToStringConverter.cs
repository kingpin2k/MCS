using Advent.Common.UI;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Advent.VmcStudio.Converters
{
    public class FontFamilyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontFamily fontFamily = (FontFamily)value;
            XmlLanguage language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            string name;
            if (!fontFamily.FamilyNames.TryGetValue(language, out name))
                name = FontUtil.GetName(fontFamily);
            return (object)name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
