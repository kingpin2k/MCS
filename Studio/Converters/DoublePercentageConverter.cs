using System;
using System.Globalization;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class DoublePercentageConverter : IValueConverter
    {
        public double Percentage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)((double)value * this.Percentage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
