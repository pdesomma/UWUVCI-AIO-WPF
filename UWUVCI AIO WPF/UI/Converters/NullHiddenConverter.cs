using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class NullHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}