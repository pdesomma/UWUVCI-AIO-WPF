using System.Windows.Input;
using WiiUInjector.Services;
using WiiUInjector.Exceptions;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels
{
    public sealed class CommonKeyViewModel : DialogViewModel, ICommonKeyViewModel
    {
        private readonly ICommonKeyService _service;

        /// <summary>
        /// Sets the common key.
        /// </summary>
        public ICommand SetKeyCommand { get; private set; }

        /// <summary>
        /// The current common key.
        /// </summary>
        public string CommonKey => _service.CommonKey;

        /// <summary>
        /// Creates a new instance of the <see cref="CommonKeyViewModel"/> class.
        /// </summary>
        public CommonKeyViewModel(ICommonKeyService service, IDialogService dialogService, ExceptionViewModel exceptionViewModel)
            : base(dialogService, exceptionViewModel)
        {
            _service = service;
            SetKeyCommand = new BasicCommand(ExecuteSetKeyCommand, (x) => CommonKey is null);
        }

        /// <summary>
        /// Executes OpenDialogCommand.
        /// </summary>
        /// <param name="param"></param>
        protected override void ExecuteOpenDialogCommand(object param) => dialogService?.ShowDialog(this, "Common Key", "Enter common key");

        /// <summary>
        /// Executes <see cref="SetKeyCommand"/>.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteSetKeyCommand(object param)
        {
            try
            {
                _service.SetKey(param.ToString());
                RaisePropertyChange(nameof(CommonKey));
            }
            catch (CommonKeyException ex) { ExceptionViewModel?.HandleExceptionCommand.Execute(ex); }
        }
    }
}