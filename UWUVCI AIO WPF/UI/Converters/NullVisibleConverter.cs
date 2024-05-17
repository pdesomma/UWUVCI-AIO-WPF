using System.Windows;
using System.Globalization;
using System;
using System.Windows.Data;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class NullVisibileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}