using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WiiUInjector;
using WiiUInjector.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public sealed class N64ConfigViewModel : ConfigViewModel<N64Config>
    {
        private readonly IDialogService _iniDialogService;

        /// <summary>
        /// Creates a new instance of the <see cref="N64ConfigViewModel"/> class.
        /// </summary>
        public N64ConfigViewModel(N64Config config, IMetadataService metadataService,  IDialogService fileDialogService,  IDialogService iniDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel) 
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService) 
        {
            _iniDialogService = iniDialogService;
            fileDialogArgs = "Nintendo 64 ROM (*.n64; *.v64; *.z64) | *.n64;*.v64;*.z64";

            OpenIniDialogCommand = new BasicCommand(ExecuteOpenIniDialogCommand, (x) => true);
        }

        /// <summary>
        /// Returns all backup sizes for selection purposes.
        /// </summary>
        public IEnumerable<BackupSize> BackupSizes => Enum.GetValues(typeof(BackupSize)).Cast<BackupSize>();

        /// <summary>
        /// Returns all backup types for selection purposes.
        /// </summary>
        public IEnumerable<BackupType> BackupTypes => Enum.GetValues(typeof(BackupType)).Cast<BackupType>();

        /// <summary>
        /// Ini backup type.
        /// </summary>
        public BackupSize BackupSize
        {
            get => Config.BackupSize;
            set
            {
                Config.BackupSize = value;
                RaisePropertyChange(nameof(BackupSize));
            }
        }

        /// <summary>
        /// Ini backup size.
        /// </summary>
        public BackupType BackupType
        {
            get => Config.BackupType;
            set
            {
                Config.BackupType = value;
                RaisePropertyChange(nameof(BackupType));
            }
        }

        /// <summary>
        /// Dark filter flag.
        /// </summary>
        public bool DarkFilter
        {
            get => Config.DarkFilter;
            set
            {
                Config.DarkFilter = value;
                RaisePropertyChange(nameof(DarkFilter));
            }
        }

        /// <summary>
        /// Expansion pak ini flag.
        /// </summary>
        public bool ExpansionPak
        {
            get => Config.ExpansionPak;
            set
            {
                Config.ExpansionPak = value;
                RaisePropertyChange(nameof(ExpansionPak));
            }
        }

        /// <summary>
        /// Ini path.
        /// </summary>
        public string IniPath => Config.IniPath;

        /// <summary>
        /// Memory pak ini flag.
        /// </summary>
        public bool MemPak
        {
            get => Config.MemPak;
            set
            {
                Config.MemPak = value;
                RaisePropertyChange(nameof(Rumble));
                RaisePropertyChange(nameof(MemPak));
            }
        }

        /// <summary>
        /// Command to open a file dialog to find ini files.
        /// </summary>
        public ICommand OpenIniDialogCommand { get; private set; }


        /// <summary>
        /// Rumble pak ini flag.
        /// </summary>
        public bool Rumble
        {
            get => Config.Rumble;
            set
            {
                Config.Rumble = value;
                RaisePropertyChange(nameof(Rumble));
                RaisePropertyChange(nameof(MemPak));
            }
        }

        /// <summary>
        /// True boot ini flag.
        /// </summary>
        public bool TrueBoot
        {
            get => Config.TrueBoot;
            set
            {
                Config.TrueBoot = value;
                RaisePropertyChange(nameof(TrueBoot));
            }
        }

        /// <summary>
        /// Use timer ini flag.
        /// </summary>
        public bool UseTimer
        {
            get => Config.UseTimer;
            set
            {
                Config.UseTimer = value;
                RaisePropertyChange(nameof(UseTimer));
            }
        }

        /// <summary>
        /// Vsync ini flag.
        /// </summary>
        public bool Vsync
        {
            get => Config.RetraceByVsync;
            set
            {
                Config.RetraceByVsync = value;
                RaisePropertyChange(nameof(Vsync));
            }
        }

        /// <summary>
        /// Flag for widescreen.
        /// </summary>
        public bool WideScreen
        {
            get => Config.WideScreen;
            set  
            {
                Config.WideScreen = value;
                RaisePropertyChange(nameof(WideScreen));
            }
        }

        /// <summary>
        /// Executes <see cref="OpenIniDialogCommand"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ExecuteOpenIniDialogCommand(object obj)
        {
            var path = _iniDialogService?.ShowDialog(this, null, "N64 VC Config (*.ini) | *.ini");
            if (path != null) Config.IniPath = path.ToString();
        }
    }
}