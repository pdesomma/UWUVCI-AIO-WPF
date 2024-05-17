using System.Windows;
using System.Globalization;
using System;
using System.Windows.Data;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class BooleanHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}