using System;
using System.ComponentModel;
using System.Windows.Input;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model that shows a dialog window with exception information.
    /// </summary>
    public sealed class ExceptionViewModel
    {
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Raises property changed events.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;        

        /// <summary>
        /// Current <see cref="Exception"/>.
        /// </summary>
        public Exception Exception {get; private set;}

        /// <summary>
        /// Command to handle the <see cref="Exception"/>
        /// </summary>
        public ICommand HandleExceptionCommand { get; private set; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="NavigationViewModel"/> class.
        /// </summary>
        public ExceptionViewModel(IDialogService dialogService) 
        {
            _dialogService = dialogService;
            this.HandleExceptionCommand = new BasicCommand(ExecuteHandleExceptionCommand, (x) => true);
        }

        /// <summary>
        /// Execute the exception command.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteHandleExceptionCommand(object param)
        {
            Exception = param as Exception;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
            _dialogService?.ShowDialog(this, "", Exception?.Message);
        }
    }
}