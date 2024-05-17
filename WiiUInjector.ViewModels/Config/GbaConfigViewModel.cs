using WiiUInjector.Configs;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Configs
{
    public sealed class GbaConfigViewModel : ConfigViewModel<GbaConfig>
    {
        public bool PokePatch
        {
            get => Config.PokePatch;
            set
            {
                Config.PokePatch = value;
                RaisePropertyChange(nameof(PokePatch));
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GbaConfigViewModel"/> class.
        /// </summary>
        public GbaConfigViewModel(GbaConfig config, IMetadataService metadataService, IDialogService fileDialogService, IDialogService showImagePreviewDialogService, ExceptionViewModel xViewModel) 
            : base(config, metadataService, xViewModel, fileDialogService, showImagePreviewDialogService) 
        {
            fileDialogArgs = "GameBoy Series ROM (*.gba;*.gbc;*.gb) | *.gba;*.gbc;*.gb";
        }
    }
}