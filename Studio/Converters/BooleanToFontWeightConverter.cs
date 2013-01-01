using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)((bool)value ? FontWeights.Bold : FontWeights.Normal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
