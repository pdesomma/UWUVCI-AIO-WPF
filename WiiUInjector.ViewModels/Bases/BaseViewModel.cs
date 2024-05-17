using WiiUInjector.Services;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// Game base definition view model.
    /// </summary>
    public sealed class BaseViewModel : ViewModel
    {
        private readonly IBaseRomService _gameBaseManager;
        private readonly IBackgroundTaskViewModel _backgroundTaskViewModel;

        /// <summary>
        /// Creates a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="gameBaseManager"></param>
        public BaseViewModel(BaseRom baseRom, IBaseRomService gameBaseManager, BackgroundViewModel backgroundViewModel, ExceptionViewModel exceptionViewModel) : base(exceptionViewModel)
        {
            _gameBaseManager = gameBaseManager;
            _backgroundTaskViewModel = backgroundViewModel;
            BaseRom = baseRom;
        }

        public BaseRom BaseRom { get; private set; }
        public GameConsole Console => BaseRom.Console;
        public bool IsDownloaded => BaseRom.IsDownloaded;
        public bool IsKeyValid => BaseRom.IsKeyValid;
        public int KeyHash => BaseRom.KeyHash;
        public string Name => BaseRom.Name;
        public string Path => BaseRom.Path;
        public string Region => BaseRom.Region.ToString();
        public string TitleId => BaseRom.TitleId;

        public string TitleKey
        {
            get => BaseRom.TitleKey;
            set
            {
                if (!BaseRom.IsKeyValid)
                {
                    BaseRom.TitleKey = value;
                    if (BaseRom.IsKeyValid) _backgroundTaskViewModel.Enqueue(_gameBaseManager.UpdateAsync(BaseRom), "Updating base definition with title key");
                    RaisePropertyChange(nameof(TitleKey));
                    RaisePropertyChange(nameof(IsKeyValid));
                }
            }
        }
    }
}