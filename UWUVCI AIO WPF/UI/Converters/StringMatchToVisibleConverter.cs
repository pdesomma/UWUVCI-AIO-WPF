using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class StringMatchToVisibleConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = value[0]?.ToString();
            var element = value[1]?.ToString();
            if (name == element) return Visibility.Visible;
            return Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}