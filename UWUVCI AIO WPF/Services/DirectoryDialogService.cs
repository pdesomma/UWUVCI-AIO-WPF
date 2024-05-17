using Microsoft.WindowsAPICodePack.Dialogs;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Shows directory dialog windows.
    /// </summary>
    public class DirectoryDialogService : IDialogService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DirectoryDialogService"/> class.
        /// </summary>
        public DirectoryDialogService() { }

        /// <summary>
        /// Show the directory dialog window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object ShowDialog(object sender, string title, string message)
        {
            using (var fileDialog = new CommonOpenFileDialog())
            {
                fileDialog.IsFolderPicker = true;
                if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)  return fileDialog.FileName;
            }
            return null;
        }
    }

}
