using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using Note.Source.MVVM.Models;

namespace Note.Source.Converters
{
    class MultiParamsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
