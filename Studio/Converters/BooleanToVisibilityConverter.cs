using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }

        public Visibility FalseValue { get; set; }

        public BooleanToVisibilityConverter()
        {
            this.TrueValue = Visibility.Visible;
            this.FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)(Visibility)((bool)value ? (int)this.TrueValue : (int)this.FalseValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (object)((Visibility)value == this.TrueValue);
        }
    }
}
