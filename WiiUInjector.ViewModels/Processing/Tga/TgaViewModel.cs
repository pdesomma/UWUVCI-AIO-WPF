using System;
using System.Timers;
using WiiUInjector.Services;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels.Tga
{
    public abstract class TgaViewModel : FileProcessingViewModel, IDisposable
    {
        private static readonly string s_fileDialogString = "Images (*.png; *.jpg; *.bmp; *.tga; *jpeg) | *.png;*.jpg;*.bmp;*.tga;*jpeg";

        protected readonly IMetadataService metadataService;
        protected readonly IDialogService previewDialogService;
        private Timer _timer;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="TgaViewModel"/> class.
        /// </summary>
        /// <param name="navViewModel"></param>
        /// <param name="exceptionViewModel"></param>
        protected TgaViewModel(IDialogService fileDialogService, IDialogService previewDialogService, IMetadataService metadataService, ExceptionViewModel exceptionViewModel) 
            : base(s_fileDialogString, fileDialogService, exceptionViewModel)
        {
            this.metadataService = metadataService;
            this.previewDialogService = previewDialogService;
        }

        /// <summary>
        /// Implement <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes an <see cref="TgaViewModel"/>.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    _timer?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Sets a timer that creates the preview image at the end of it, or adds a half second to the interval if it's already created.
        /// </summary>
        protected void SetTimer()
        {
            if (_timer is null)
            {
                _timer = new Timer();
                _timer.Elapsed += async (s, e) =>
                {
                    _timer.Stop();
                    await GeneratePreviewAsync();
                };
            }

            if (_timer.Enabled)
            {
                _timer.Interval += 50;
            }
            else
            {
                _timer.Interval = 150;
                _timer.Start();
            }
        }

        /// <summary>
        /// Executes the show image command.
        /// </summary>
        /// <param name="parameter"></param>
        protected override void ExecuteActivatePreviewCommand(object parameter)
        {
           if(Preview != null) previewDialogService.ShowDialog(this, "Preview", "");
        }
    }
}
