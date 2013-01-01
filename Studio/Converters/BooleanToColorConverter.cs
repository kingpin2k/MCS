using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Advent.VmcStudio.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public Color TrueValue { get; set; }

        public Color FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)((bool)value ? this.TrueValue : this.FalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
