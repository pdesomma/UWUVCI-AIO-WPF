using UWUVCI_AIO_WPF.UI.Windows;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Shows custom message box windows.
    /// </summary>
    public class ToadDialogService : IDialogService
    {
        public ToadDialogService() { }

        public object ShowDialog(object sender, string title, string message)
        {
            var window = new ToadMessageDialogWindow(message)
            {
                DataContext = sender,
                Owner = App.Current.MainWindow
            };
            window.ShowDialog();
            return null;
        }
    }

}
