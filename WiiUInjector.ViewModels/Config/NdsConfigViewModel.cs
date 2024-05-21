using WiiUInjector.GitTools.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public class NdsConfigViewModel : ConfigViewModel<Config>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NdsConfigViewModel"/> class.
        /// </summary>
        public NdsConfigViewModel(Config config, IMetadataService metadataService, IDialogService fileDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel)
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService)
        {
            fileDialogArgs = "Nintendo DS Rom (*.nds;*.srl) | *.nds;*.srl";
        }
    }
}