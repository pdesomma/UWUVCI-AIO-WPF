using System;
using System.IO;
using System.Windows.Input;
using WiiUInjector.GitTools.Configs;
using WiiUInjector.Messaging;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.BootSound;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;
using WiiUInjector.ViewModels.Services;
using WiiUInjector.ViewModels.Tga;

namespace WiiUInjector.ViewModels.Configs
{
    public abstract class ConfigViewModel<TConfig> : GoBackViewModel, IConfigViewModel where TConfig : Config
    {
        private readonly IDialogService _dialogService;
        protected string fileDialogArgs;

        /// <summary>
        /// Creates a new instance of the <see cref="ConfigViewModel"/> class.
        /// </summary>
        public ConfigViewModel(TConfig config, IMetadataService metadataService, ExceptionViewModel xViewModel, IDialogService fileDialogService, IDialogService showImagePreviewDialogService)
            : base(xViewModel)
        {
            _dialogService = fileDialogService;
            Config = config;

            // setup view models.
            IconViewModel = new IconViewModel(config.Console, _dialogService, showImagePreviewDialogService, metadataService, ExceptionViewModel);
            TvViewModel = new TvCreatorViewModel(config.Console, _dialogService, showImagePreviewDialogService, metadataService, ExceptionViewModel);
            GamepadViewModel = new GamepadViewModel(config.Console, _dialogService, showImagePreviewDialogService, metadataService, ExceptionViewModel);
            LogoViewModel = new LogoViewModel(showImagePreviewDialogService, metadataService, ExceptionViewModel);
            BootsoundViewModel = new BootsoundViewModel(fileDialogService, metadataService, ExceptionViewModel);

            // setup commands
            ConfirmConfigCommand = new BasicCommand(ExecuteConfirmConfigCommand, (x) => true);
            OpenRomPathFileDialogCommand = new BasicCommand(ExecuteOpenRomPathFileDialogCommand, (x) => true);
        }

        /// <summary>
        /// Explicit interface implementation.
        /// </summary>
        Config IConfigViewModel.Config => Config;

        /// <summary>
        /// Builds bootsound.
        /// </summary>
        public BootsoundViewModel BootsoundViewModel { get; private set; }

        /// <summary>
        /// Injection configuration.
        /// </summary>
        public TConfig Config { get; private set; }

        /// <summary>
        /// Confirms the config is ready.
        /// </summary>
        public ICommand ConfirmConfigCommand { get; private set; }

        /// <summary>
        /// Builds gamepad tgas.
        /// </summary>
        public TgaViewModel GamepadViewModel { get; private set; }

        /// <summary>
        /// Builds icon tgas.
        /// </summary>
        public TgaViewModel IconViewModel { get; private set; }

        /// <summary>
        /// Builds logo tgas.
        /// </summary>
        public TgaViewModel LogoViewModel { get; private set; }

        /// <summary>
        /// Config game name.
        /// </summary>
        public string Name
        {
            get => Config.Name;
            set
            {
                Config.Name = value;
                RaisePropertyChange(nameof(Name));
            }
        }

        /// <summary>
        /// Go to a file processing element.
        /// </summary>
        public ICommand NavigateToFileProcessingCommand { get; private set; } = new BasicCommand(x => Messenger.Send(new FileProcessingStartNotification(x.ToString())), y => true);

        /// <summary>
        /// Opens a file dialog to get the rom path.
        /// </summary>
        public ICommand OpenRomPathFileDialogCommand { get; private set; }

        /// <summary>
        /// Config file location of rom to inject.
        /// </summary>
        public string RomPath
        {
            get => Config.RomPath;
            set
            {
                Config.RomPath = value;
                RaisePropertyChange(nameof(RomPath));
            }
        }

        /// <summary>
        /// Builds tv tgas.
        /// </summary>
        public TgaViewModel TvViewModel { get; private set; }

        /// <summary>
        /// Checks if it's ok to navigate away to the next page.
        /// </summary>
        /// <returns></returns>
        private void ExecuteConfirmConfigCommand(object parameter)
        {
            if (RomPath is null || (!File.Exists(RomPath) && !Directory.Exists(RomPath)))
            {
                ExceptionViewModel.HandleExceptionCommand.Execute(new FileNotFoundException("RomPath is not valid"));
                return;
            }
            if (Name is null)
            {
                ExceptionViewModel.HandleExceptionCommand.Execute(new Exception("Game name is not valid"));
                return;
            }
            //Config.IconPath = IconViewModel.ProcessedFileLocation;
            //Config.GamePadPath = GamepadViewModel.ProcessedFileLocation;
            //Config.LogoPath = LogoViewModel.ProcessedFileLocation;
            //Config.TvPath = TvViewModel.ProcessedFileLocation;
            //Config.BootSoundPath = BootsoundViewModel.ProcessedFileLocation;
            Messenger.Send(new ConfigReadyNotification());
        }

        /// <summary>
        /// Executes the open file dialog command.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteOpenRomPathFileDialogCommand(object param)
        {
            var file = _dialogService?.ShowDialog(this, "", fileDialogArgs);
            if (file != null) RomPath = file.ToString();
        }
    }
}