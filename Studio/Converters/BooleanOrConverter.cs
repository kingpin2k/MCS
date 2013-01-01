using System;
using System.Globalization;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            foreach (bool flag in values)
            {
                if (flag)
                    return (object)true;
            }
            return (object)false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            object[] objArray = new object[targetTypes.Length];
            for (int index = 0; index < targetTypes.Length; ++index)
                //TODO bool or int?
                objArray[index] = (object)((bool)value);
            return objArray;
        }
    }
}
