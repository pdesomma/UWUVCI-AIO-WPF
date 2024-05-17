
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// Base view model implementation.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event when properties change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Exception handling view model.
        /// </summary>
        public ExceptionViewModel ExceptionViewModel { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        /// <param name="exceptionViewModel"></param>
        public ViewModel(ExceptionViewModel exceptionViewModel)
        {
            ExceptionViewModel = exceptionViewModel;
        }

        /// <summary>
        /// Changes property value and invokes <see cref="PropertyChanged"/> event handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns>'true' if the value is changed, 'false' otherwise</returns>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue)) return false;

            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        /// <summary>
        /// Raises the property changed event
        /// </summary>
        protected void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}