using Advent.VmcStudio.Theme.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
namespace Advent.VmcStudio.Converters
{
    public class StringToFontClassConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FontClassesProperty = DependencyProperty.Register("FontClasses", typeof(IEnumerable<FontClassModel>), typeof(StringToFontClassConverter), new PropertyMetadata(new List<FontClassModel>()));
        internal IEnumerable<FontClassModel> FontClasses
        {
            get
            {
                return (IEnumerable<FontClassModel>)base.GetValue(StringToFontClassConverter.FontClassesProperty);
            }
            set
            {
                base.SetValue(StringToFontClassConverter.FontClassesProperty, value);
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (
                from o in this.FontClasses
                where o.Item.Name == (string)value
                select o).FirstOrDefault<FontClassModel>();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((FontClassModel)value).Item.Name;
        }
    }
}
