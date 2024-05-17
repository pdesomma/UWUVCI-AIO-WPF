using UWUVCI_AIO_WPF.UI.Windows;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Shows exception dialog windows.
    /// </summary>
    public class ExceptionDialogService : IDialogService
    {
        public ExceptionDialogService() { }

        public object ShowDialog(object sender, string title, string message)
        {
            ExceptionWindow window = new ExceptionWindow("Oh no!", message)
            {
                Owner = App.Current.MainWindow,
                DataContext = sender
            };
            window.ShowDialog();
            return null;
        }
    }

}
