using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.BootSound
{
    public sealed class BootsoundViewModel : FileProcessingViewModel
    {
        private static readonly string s_fileTypes = "Sounds (*.mp3; *.wav) | *.mp3;*.wav";

        private DateTime _playTime = DateTime.Now;
        private readonly IMetadataService _metadataService;
        
        /// <summary>
        /// Creates a new instance of the <see cref="BootsoundViewModel"/> class.
        /// </summary>
        /// <param name="navViewModel"></param>
        /// <param name="exceptionViewModel"></param>
        public BootsoundViewModel(IDialogService fileDialogService, IMetadataService metadataService, ExceptionViewModel exceptionViewModel) 
            : base(s_fileTypes, fileDialogService, exceptionViewModel)
        {
            _metadataService = metadataService;
        }

        /// <summary>
        /// Generate preview sound bytes.
        /// </summary>
        /// <returns></returns>
        protected override async Task<byte[]> DoGeneratePreviewWorkAsync()
        {
            await _metadataService.GenerateBootSoundPreviewAsync(SourceFileLocation);
            return _metadataService.BootSoundPreview;
        }

        /// <summary>
        /// Create and save a processed bootsound file.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> DoSaveProcessedFileWorkAsync()
        {
            return await Task.Run(() => _metadataService.SaveBootSound());
        }

        /// <summary>
        /// Plays the preview sound.
        /// </summary>
        /// <param name="obj"></param>
        protected override void ExecuteActivatePreviewCommand(object obj)
        {
            if((DateTime.Now - _playTime).TotalSeconds > 7 && Preview != null)
            {
                _playTime = DateTime.Now;
                using (var stream = new MemoryStream(Preview))
                using (var player = new SoundPlayer(stream))
                {
                    player.Play();
                }
            }
        }

        /// <summary>
        /// Executes the clear sound command.
        /// </summary>
        /// <param name="parameter"></param>
        protected override void ExecuteClearProcessedFileCommand(object parameter)
        {
            _metadataService.ClearBootSound();
            base.ExecuteClearProcessedFileCommand(parameter);
        }
    }
}
