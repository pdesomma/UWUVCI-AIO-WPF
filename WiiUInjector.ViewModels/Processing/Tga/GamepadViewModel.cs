using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WiiUInjector;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Tga
{
    public sealed class GamepadViewModel : TgaViewModel
    {
        private readonly GameConsole _console;
        private string _name1;
        private string _name2;
        private bool _overlay = true;
        private int _players;
        private string _selectedStyle;
        private int _year;

        /// <summary>
        /// Creates a new instance of the <see cref="GamepadViewModel"/>
        /// </summary>
        public GamepadViewModel(GameConsole console, IDialogService fileDialogService, IDialogService previewDialogService, IMetadataService metadataService, ExceptionViewModel exception)
            : base(fileDialogService, previewDialogService, metadataService, exception)
        {
            _console = console;
        }

        /// <summary>
        /// First name to add to an overlay
        /// </summary>
        public string Name1
        {
            get => _name1;
            set
            {
                SetProperty(ref _name1, value);
                SetTimer();
            }
        }

        /// <summary>
        /// Second name to add to an overlay.
        /// </summary>
        public string Name2
        {
            get => _name2;
            set
            {
                SetProperty(ref _name2, value);
                SetTimer();
            }
        }

        /// <summary>
        /// Release year to add to an overlay.
        /// </summary>
        public int Year
        {
            get => _year;
            set
            {
                SetProperty(ref _year, value);
                SetTimer();
            }
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
        /// Number of players to add to the overlay.
        /// </summary>
        public int Players
        {
            get => _players;
            set
            {
                SetProperty(ref _players, value);
                SetTimer();
            }
        }

        /// <summary>
        /// Available overlay style locations.
        /// </summary>
        public ObservableCollection<string> Styles
        {
            get => new ObservableCollection<string>(metadataService.GetTvTemplates(_console));
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
            await metadataService.GenerateGamePadPreviewAsync(SourceFileLocation, _overlay ? _selectedStyle : null, _name1, _name2, _year, _players);
            return metadataService.GamePadPreview;
        }

        /// <summary>
        /// Save the tga image
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> DoSaveProcessedFileWorkAsync() => await metadataService.SaveGamePadTgaAsync();

        /// <summary>
        /// Clears the processed tga image.
        /// </summary>
        /// <returns></returns>
        protected override void ExecuteClearProcessedFileCommand(object param)
        {
            base.ExecuteClearProcessedFileCommand(param);
            metadataService.ClearGamePad();
        }
    }
}