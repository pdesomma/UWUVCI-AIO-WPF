using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Tga
{
    public sealed class LogoViewModel : TgaViewModel
    {
        private int _fontSize = 18;
        private string _text;

        /// <summary>
        /// Creates a new instance of the <see cref="LogoViewModel"/>
        /// </summary>
        public LogoViewModel(IDialogService previewDialogService, IMetadataService metadataService, ExceptionViewModel exception)
            : base(null, previewDialogService, metadataService, exception)
        {
            _ = GeneratePreviewAsync();
        }

        /// <summary>
        /// Logo text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                SetProperty(ref _text, value);
                SetTimer();
            }
        }

        /// <summary>
        /// Font size of the logo.
        /// </summary>
        public int FontSize
        {
            get => _fontSize;
            set
            {
                SetProperty(ref _fontSize, value);
                SetTimer();
            }
        }

        /// <summary>
        /// The selected overlay style.
        /// </summary>
        public ObservableCollection<string> Styles => new ObservableCollection<string>() { metadataService.GetLogoTemplate() };

        /// <summary>
        /// Get the preview from the service.
        /// </summary>
        /// <returns></returns>
        protected override async Task<byte[]> DoGeneratePreviewWorkAsync()
        {
            await metadataService.GenerateLogoPreviewAsync(metadataService.GetLogoTemplate(), _text, _fontSize);
            return metadataService.LogoPreview;
        }

        /// <summary>
        /// Save the tga image
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> DoSaveProcessedFileWorkAsync() => await metadataService.SaveLogoTgaAsync();

        /// <summary>
        /// Clears the processed tga image.
        /// </summary>
        /// <returns></returns>
        protected override void ExecuteClearProcessedFileCommand(object param)
        {
            base.ExecuteClearProcessedFileCommand(param);
            metadataService.ClearLogo();
        }
    }
}