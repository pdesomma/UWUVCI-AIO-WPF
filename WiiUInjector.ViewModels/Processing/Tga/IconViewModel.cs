using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WiiUInjector;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Tga
{
    public sealed class IconViewModel : TgaViewModel
    {
        private readonly GameConsole _console;
        private bool _overlay = true;
        private string _selectedStyle;

        /// <summary>
        /// Creates a new instance of the <see cref="IconViewModel"/>
        /// </summary>
        public IconViewModel(GameConsole console, IDialogService fileDialogService, IDialogService previewDialogService, IMetadataService metadataService, ExceptionViewModel exceptionViewModel) 
            : base(fileDialogService, previewDialogService, metadataService, exceptionViewModel)
        {
            _console = console;
        }

        /// <summary>
        /// Use the overlay or not.
        /// </summary>
        public bool Overlay
        {
            get => _overlay;
            set
            {
                SetProperty(ref _overlay, value);
                _ = GeneratePreviewAsync();
            }
        }

        /// <summary>
        /// Available overlay style locations.
        /// </summary>
        public ObservableCollection<string> Styles
        {
            get => new ObservableCollection<string>(metadataService.GetIconTemplates(_console));
        }

        /// <summary>
        /// The selected overlay style.
        /// </summary>
        public string SelectedStyle
        {
            get => _selectedStyle;
            set
            {
                SetProperty(ref _selectedStyle, value);
                _ = GeneratePreviewAsync();
            }
        }

        /// <summary>
        /// Get the preview from the service.
        /// </summary>
        /// <returns></returns>
        protected override async Task<byte[]> DoGeneratePreviewWorkAsync()
        {
            await metadataService.GenerateIconPreviewAsync(SourceFileLocation, _overlay ? _selectedStyle : null);
            return metadataService.IconPreview;
        }

        /// <summary>
        /// Save the tga image
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> DoSaveProcessedFileWorkAsync() => await metadataService.SaveIconTgaAsync();

        /// <summary>
        /// Clears the processed tga image.
        /// </summary>
        /// <returns></returns>
        protected override void ExecuteClearProcessedFileCommand(object param)
        {
            base.ExecuteClearProcessedFileCommand(param);
            metadataService.ClearIcon();
        }
    }
}