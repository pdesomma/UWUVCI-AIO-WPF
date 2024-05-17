using System.Windows.Input;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// Base dialog view model implementation.
    /// </summary>
    public abstract class DialogViewModel : ViewModel, IDialogViewModel
    {
        protected readonly IDialogService dialogService;

        /// <summary>
        /// Opens a dialog window
        /// </summary>
        public ICommand OpenDialogCommand { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="DialogViewModel"/> class.
        /// </summary>
        /// <param name="exceptionViewModel"></param>
        public DialogViewModel(IDialogService service, ExceptionViewModel exceptionViewModel) : base(exceptionViewModel)
        {
            dialogService = service;
            OpenDialogCommand = new BasicCommand(ExecuteOpenDialogCommand, (x) => true);
        }

        /// <summary>
        /// Executes <see cref="OpenDialogCommand"/>.
        /// </summary>
        /// <param name="param"></param>
        protected abstract void ExecuteOpenDialogCommand(object param);
    }
}