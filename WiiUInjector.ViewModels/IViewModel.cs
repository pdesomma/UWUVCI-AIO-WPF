using System.ComponentModel;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// A basic view model.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Exception handling view model.
        /// </summary>
        ExceptionViewModel ExceptionViewModel { get; }
    }
}