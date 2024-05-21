using System;
using System.Windows.Input;
using WiiUInjector.GitTools.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public sealed class Tg16ConfigViewModel : ConfigViewModel<Tg16Config>
    {
        private readonly IDialogService _directoryDialogService;

        /// <summary>
        /// Creates a new instance of the <see cref="Tg16ConfigViewModel"/> class.
        /// </summary>
        public Tg16ConfigViewModel(Tg16Config config, IMetadataService metadataService, IDialogService fileDialogService,  IDialogService directoryDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel) 
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService) 
        {
            _directoryDialogService = directoryDialogService;
            fileDialogArgs = "TurboGrafX-16 ROM (*.pce) | *.pce";

            OpenDirectoryDialogCommand = new BasicCommand(ExecuteOpenDirectoryDialogCommand, (x) => true);
        }


        /// <summary>
        /// Flag for whether this is a cd rom or not.
        /// </summary>
        public bool Cd
        {
            get => Config.Cd;
            set
            {
                Config.Cd = value;
                RaisePropertyChange(nameof(Cd));
            }
        }

        /// <summary>
        /// Command to open a directory browser.
        /// </summary>
        public ICommand OpenDirectoryDialogCommand { get; private set; }

        /// <summary>
        /// Executes <see cref="OpenDirectoryDialogCommand"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ExecuteOpenDirectoryDialogCommand(object obj)
        {
            var directory = _directoryDialogService?.ShowDialog(this, null, null);
            if (directory != null) RomPath = directory.ToString();
        }
    }
}