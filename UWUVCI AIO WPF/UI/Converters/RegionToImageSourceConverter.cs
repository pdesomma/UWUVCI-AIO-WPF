using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows.Media.Imaging;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class RegionToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            return new BitmapImage(new Uri("pack://application:,,,/UI/Images/regions/icon-" + value.ToString().ToLower() + ".png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}