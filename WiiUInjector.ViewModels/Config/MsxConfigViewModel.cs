using WiiUInjector.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public class MsxConfigViewModel : ConfigViewModel<Config>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MsxConfigViewModel"/> class.
        /// </summary>
        public MsxConfigViewModel(Config config, IMetadataService metadataService, IDialogService fileDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel)
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService)
        {
            fileDialogArgs = "MSX/MSX2 ROM (*.rom;*.mx2) | *.rom;*.mx2";
        }
    }
}