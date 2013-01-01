using System;
using System.Globalization;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO bool or int?
            return (object)!(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO bool or int?
            return (object)!(bool)value;
        }
    }
}
