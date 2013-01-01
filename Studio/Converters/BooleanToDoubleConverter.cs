using System;
using System.Globalization;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class BooleanToDoubleConverter : IValueConverter
    {
        public double TrueValue { get; set; }

        public double FalseValue { get; set; }

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
