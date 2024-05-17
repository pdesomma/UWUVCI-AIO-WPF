using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WiiUInjector;
using WiiUInjector.Services;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model for working with available <see cref="BaseRom"/>s.
    /// </summary>
    public sealed class BasesViewModel : GoBackViewModel
    {
        private readonly IBaseRomService _gameBaseManager;
        private readonly ICommonKeyViewModel _commonKeyViewModel;
        private readonly BackgroundViewModel _backgroundTaskViewModel;
        private BaseViewModel _selection;
        private ObservableCollection<BaseViewModel> _gamesBases;

        /// <summary>
        /// Creates a new instance of the <see cref="BasesViewModel"/> class.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="baseService"></param>
        public BasesViewModel(GameConsole console, IBaseRomService baseService, ICommonKeyViewModel commonKeyViewModel, BackgroundViewModel backgroundViewModel, ExceptionViewModel exceptionViewModel) 
            : base(exceptionViewModel)
        {
            this._backgroundTaskViewModel = backgroundViewModel;
            this._gameBaseManager = baseService;
            this._commonKeyViewModel = commonKeyViewModel;

            _backgroundTaskViewModel.Enqueue(GetAsyncData(console), "Getting game base definitions for " + console.ToString());
            ConfirmBaseCommand = new BasicCommand(ExecuteConfirmBaseCommand, x => true);
        }

        /// <summary>
        /// Confirms that the base selected and ready for injection.
        /// </summary>
        public ICommand ConfirmBaseCommand { get; private set; }

        /// <summary>
        /// Observable collection of base definitions.
        /// </summary>
        public ObservableCollection<BaseViewModel> GameBases
        {
            get => _gamesBases;
            set => SetProperty(ref _gamesBases, value);
        }

        /// <summary>
        /// The current selected base.
        /// </summary>
        public BaseViewModel SelectedGameBase
        {
            get => _selection;
            set
            {
                SetProperty(ref _selection, value);
            }
        }

        /// <summary>
        /// Gets data for the constructor asynchronously
        /// </summary>
        /// <returns></returns>
        private async Task GetAsyncData(GameConsole console)
        {
            // this can be one-lined with Linq but for clarity doing it like this
            var bases = new ObservableCollection<BaseViewModel>();
            foreach (var gb in await _gameBaseManager.GetBaseRomDefinitionsAsync(console))
            {
                bases.Add(new BaseViewModel(gb, _gameBaseManager, _backgroundTaskViewModel, ExceptionViewModel));
            }
            GameBases = bases;
        }

        /// <summary>
        /// Executes the <see cref="ConfirmBaseCommand"/>.
        /// </summary>
        /// <returns></returns>
        private void ExecuteConfirmBaseCommand(object parameter)
        {
            if (SelectedGameBase is null)
            {
                ExceptionViewModel.HandleExceptionCommand.Execute(new Exception("Select a game base"));
                return;
            }
            if (!SelectedGameBase.IsDownloaded)
            {
                _backgroundTaskViewModel.Enqueue(_gameBaseManager.DownloadBinaryAsync(_commonKeyViewModel.CommonKey, SelectedGameBase.BaseRom), "Downloading " + SelectedGameBase.Name);
            }
            Messenger.Send(new BaseSelectedNotification(_selection));
        }
    }
}