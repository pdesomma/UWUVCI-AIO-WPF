using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UWUVCI_AIO_WPF.UI.Windows
{
    /// <summary>
    /// Called to preview an image that was generated for the injection.
    /// </summary>
    public partial class ImagePreviewDialogWindow : Window
    {
        /// <summary>
        /// Make a new instance of the <see cref="ImagePreviewDialogWindow"/> class.
        /// </summary>
        public ImagePreviewDialogWindow()
        {
            InitializeComponent();

            var prop = DependencyPropertyDescriptor.FromProperty(System.Windows.Controls.Image.SourceProperty, typeof(System.Windows.Controls.Image));
            prop.AddValueChanged(img, SourceChangedHandler);
            SizeChanged += ShowImagePreviewDialogWindow_SizeChanged;
        }

        /// <summary>
        /// Recenters after accidental movement from resizing the image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImagePreviewDialogWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double parentLeft = Owner.Left;
            double parentTop = Owner.Top;
            double parentWidth = Owner.ActualWidth;
            double parentHeight = Owner.ActualHeight;
            double childWidth = ActualWidth;
            double childHeight = ActualHeight;
            Left = parentLeft + (parentWidth - childWidth) / 2;
            Top = parentTop + (parentHeight / 2) - (childHeight / 2);
        }

        /// <summary>
        /// Resize the image control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceChangedHandler(object sender, EventArgs e)
        {
            // this is hacky and dumb but it works. i'm sure there's a proper way to do this via binding but i can't figure it out.
            if (img?.Source != null)
            {
                img.Height = (img.Source as ImageSource).Height;
                img.Width = (img.Source as ImageSource).Width;
            }
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
