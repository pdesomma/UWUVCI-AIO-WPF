using System.Windows.Data;
using System.Globalization;
using System;
using System.Windows.Media.Imaging;
using System.IO;

namespace UWUVCI_AIO_WPF.UI.Converters
{
    public sealed class BytesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bytes = (byte[])value;
            if (bytes is null) return null;

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                BitmapImage returnImage = new BitmapImage();
                returnImage.BeginInit();
                returnImage.CacheOption = BitmapCacheOption.OnLoad;
                stream.Seek(0, SeekOrigin.Begin);
                returnImage.StreamSource = stream;
                returnImage.EndInit();
                
                return returnImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}