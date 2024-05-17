using System.Windows.Input;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels
{
    public sealed class ToadViewModel : DialogViewModel
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ToadViewModel"/> class.
        /// </summary>
        public ToadViewModel(IDialogService dialogService, ExceptionViewModel exceptionViewModel) : base(dialogService, exceptionViewModel) { }

        /// <summary>
        /// Executes OpenDialogCommand.
        /// </summary>
        /// <param name="param"></param>
        protected override void ExecuteOpenDialogCommand(object param) => dialogService?.ShowDialog(this, "", param.ToString());
    }
}