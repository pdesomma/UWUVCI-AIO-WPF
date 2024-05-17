using System.Windows;
using System.Globalization;
using System;
using System.Windows.Data;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value1 = (double)values[0];
            double value2 = (double)values[1];
            return value1 * value2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}