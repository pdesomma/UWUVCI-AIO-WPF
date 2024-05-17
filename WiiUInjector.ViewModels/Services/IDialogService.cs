namespace WiiUInjector.ViewModels.Services
{
    /// <summary>
    /// Service that raises dialog windows.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a dialog window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        object ShowDialog(object sender, string title, string message);
    }
}