using WiiUInjector.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public sealed class NesConfigViewModel : ConfigViewModel<NesConfig>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NesConfigViewModel"/> class.
        /// </summary>
        public NesConfigViewModel(NesConfig config, IMetadataService metadataService, IDialogService fileDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel) 
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService) 
        {
            fileDialogArgs = "Nintendo Entertainment System ROM (*.nes) | *.nes";
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