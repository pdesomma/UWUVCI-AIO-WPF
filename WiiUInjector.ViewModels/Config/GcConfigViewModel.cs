using WiiUInjector.GitTools.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public sealed class GcConfigViewModel : ConfigViewModel<GcConfig>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GcConfigViewModel"/> class.
        /// </summary>
        public GcConfigViewModel(GcConfig config, IMetadataService metadataService, IDialogService fileDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel) 
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService) 
        {
            fileDialogArgs = "GCN ROM (*.iso;*.gcm) | *.iso;*.gcm";
        }

        /// <summary>
        /// Disable the gamepad
        /// </summary>
        public bool DisableGamepad
        {
            get => Config.DisableGamepad;
            set
            {
                Config.DisableGamepad = value;
                RaisePropertyChange(nameof(DisableGamepad));
            }
        }

        /// <summary>
        /// Disable trim
        /// </summary>
        public bool DisableTrim
        {
            get => Config.DisableTrim;
            set
            {
                Config.DisableTrim = value;
                RaisePropertyChange(nameof(DisableTrim));
            }
        }

        /// <summary>
        /// Force 4:3
        /// </summary>
        public bool FourThree
        {
            get => Config.FourThree;
            set
            {
                Config.FourThree = value;
                RaisePropertyChange(nameof(FourThree));
            }
        }

        /// <summary>
        /// Second rom path
        /// </summary>
        public string RomPath2
        {
            get => Config.RomPath2;
            set
            {
                Config.RomPath2 = value;
                RaisePropertyChange(nameof(RomPath2));
            }
        }
    }
}