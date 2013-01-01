using System;
using System.Globalization;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter", "The converter parameter must be a string of the same format used by string.Format().");
            string format = parameter as string;
            if (format == null)
                throw new ArgumentException("The converter parameter must be a string of the same format used by string.Format().");
            return (object)string.Format((IFormatProvider)culture, format, new object[1]
      {
        value
      });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
