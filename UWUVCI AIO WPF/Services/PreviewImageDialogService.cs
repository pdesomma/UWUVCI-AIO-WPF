using UWUVCI_AIO_WPF.UI.Windows;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Shows image preview dialog windows.
    /// </summary>
    public class PreviewImageDialogService : IDialogService
    {
        public PreviewImageDialogService() { }

        /// <summary>
        /// Show the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object ShowDialog(object sender, string height, string width)
        {
            var win = new ImagePreviewDialogWindow
            {
                DataContext = sender,
                Owner = App.Current.MainWindow
            };
            win.ShowDialog();
            return null;
        }
    }

}
