using Microsoft.Win32;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Shows file dialog windows.
    /// </summary>
    public class FileDialogService : IDialogService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FileDialogService"/> class.
        /// </summary>
        public FileDialogService() { }

        /// <summary>
        /// Show the file dialog window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object ShowDialog(object sender, string title, string message)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = message
            };
            fileDialog.ShowDialog();
            return fileDialog.FileName;
        }
    }

}
