using WiiUInjector.GitTools.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public sealed class SnesConfigViewModel : ConfigViewModel<SnesConfig>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SnesConfigViewModel"/> class.
        /// </summary>
        public SnesConfigViewModel(SnesConfig config, IMetadataService metadataService, IDialogService fileDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel) 
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService) 
        {
            fileDialogArgs = "Super Nintendo Entertainment System ROM (*.sfc; *.smc) | *.sfc;*.smc"; ;
        }

        public bool PixelPerfect
        {
            get => Config.PixelPerfect;
            set
            {
                Config.PixelPerfect = value;
                RaisePropertyChange("PixelPerfect");
            }
        }
    }
}