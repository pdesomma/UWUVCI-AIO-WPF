using UWUVCI_AIO_WPF.UI.Windows;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Shows common key entry dialog windows.
    /// </summary>
    public class CommonKeyDialogService : IDialogService
    {
        public CommonKeyDialogService() { }

        public object ShowDialog(object sender, string title, string message)
        {
            CommonKeyDialogWindow window = new CommonKeyDialogWindow
            {
                Owner = App.Current.MainWindow,
                DataContext = sender
            };
            window.ShowDialog();
            return null;
        }
    }

}
