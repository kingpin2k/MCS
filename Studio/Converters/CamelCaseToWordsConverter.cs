using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Advent.VmcStudio.Converters
{
    public class CamelCaseToWordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return (object)null;
            string str = value.ToString();
            if (string.IsNullOrEmpty(str))
                return (object)str;
            bool flag1 = char.IsUpper(str[0]);
            bool flag2 = char.IsNumber(str[0]);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(str[0]);
            for (int index = 1; index < str.Length; ++index)
            {
                bool flag3 = char.IsUpper(str[index]);
                bool flag4 = char.IsNumber(str[index]);
                if (!flag4 && !flag3 && flag1)
                {
                    if (stringBuilder.Length > 2 && (int)stringBuilder[stringBuilder.Length - 2] != 32)
                        stringBuilder.Insert(stringBuilder.Length - 1, ' ');
                }
                else if (flag3 && !flag1 || flag4 && !flag2 || flag2 && !flag4)
                    stringBuilder.Append(' ');
                flag1 = flag3;
                flag2 = flag4;
                stringBuilder.Append(flag1 ? str[index] : char.ToLower(str[index]));
            }
            return (object)((object)stringBuilder).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
