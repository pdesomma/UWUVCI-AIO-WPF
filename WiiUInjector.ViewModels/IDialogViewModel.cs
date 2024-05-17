using System.Windows.Input;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model that can open up dialog windows.
    /// </summary>
    public interface IDialogViewModel
    {
        /// <summary>
        /// Opens a dialog window.
        /// </summary>
        ICommand OpenDialogCommand { get; }
    }
}