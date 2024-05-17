using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model that processes a source file and generates previews.
    /// </summary>
    public abstract class FileProcessingViewModel : DialogViewModel, IFileProcessingViewModel
    {
        protected string fileTypesForDialog;
        private string _sourceFile;
        private string _processedFile;
        private byte[] _preview;

        /// <summary>
        /// Creates a new instance of the <see cref="FileProcessingViewModel"/> class.
        /// </summary>
        /// <param name="exceptionViewModel"></param>
        public FileProcessingViewModel(string fileTypes, IDialogService openFileDialogService, ExceptionViewModel exceptionViewModel) 
            : base(openFileDialogService, exceptionViewModel)
        {
            fileTypesForDialog = fileTypes;
            ActivatePreviewCommand = new BasicCommand(ExecuteActivatePreviewCommand, (x) => true);
            CancelCommand = new BasicCommand(ExecuteCanceledCommand, (y) => true);
            ClearProcessedFileCommand = new BasicCommand(ExecuteClearProcessedFileCommand, (x) => true);
            ClearSourceFileCommand = new BasicCommand(ExecuteClearSourceFileCommand, (x) => true);
            SaveProcessedFileCommand = new BasicCommand(ExecuteSaveProcessedFileCommand, (x) => true);
        }

        /// <summary>
        /// Activate the generated preview bytes.
        /// </summary>
        public ICommand ActivatePreviewCommand { get; private set; }

        /// <summary>
        /// Canceling the file processing.
        /// </summary>
        public ICommand CancelCommand { get; private set; } 

        /// <summary>
        /// Clears the files that were processed and saved.
        /// </summary>
        public ICommand ClearProcessedFileCommand { get; private set; }

        /// <summary>
        /// Command to clear the source file.
        /// </summary>
        public ICommand ClearSourceFileCommand { get; private set; }

        /// <summary>
        /// Prevew image bytes.
        /// </summary>
        public byte[] Preview
        {
            get => _preview;
            protected set => SetProperty(ref _preview, value);
        }

        /// <summary>
        /// The saved file resulting from processing the source.
        /// </summary>
        public string ProcessedFileLocation
        {
            get => _processedFile;
            protected set => SetProperty(ref _processedFile, value);
        }

        /// <summary>
        /// Saves the preview into a processed file.
        /// </summary>
        public ICommand SaveProcessedFileCommand { get; private set; }

        /// <summary>
        /// Full source file path.
        /// </summary>
        public string SourceFileLocation 
        { 
            get => _sourceFile; 
            private set
            {
                string oldSource = _sourceFile;
                if(SetProperty(ref _sourceFile, value))
                {
                    _ = GeneratePreviewAsync();
                }
            }
        }

        /// <summary>
        /// Child-specific work to generate preview bytes.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<byte[]> DoGeneratePreviewWorkAsync();

        /// <summary>
        /// Child-specific work to do processing the source file to create a processed file.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<string> DoSaveProcessedFileWorkAsync();

        /// <summary>
        /// Child-specific work to do something with the preview bytes.
        /// </summary>
        /// <returns></returns>
        protected abstract void ExecuteActivatePreviewCommand(object param);    

        /// <summary>
        /// Executes <see cref="ClearProcessedFileCommand"/>.
        /// </summary>
        /// <param name="param"></param>
        protected virtual void ExecuteClearProcessedFileCommand(object param)
        {
            ProcessedFileLocation = null;
            Preview = null;
            SourceFileLocation = null;
        }

        /// <summary>
        /// Executes <see cref="OpenDialogCommand"/>.
        /// </summary>
        /// <param name="param"></param>
        protected override void ExecuteOpenDialogCommand(object param)
        {
            var fileLocation = this.dialogService?.ShowDialog(this, "", fileTypesForDialog).ToString();
            if (!string.IsNullOrWhiteSpace(fileLocation))
            {
                SourceFileLocation = fileLocation;
            }
        }

        /// <summary>
        /// Executes <see cref="ClearProcessedFileCommand"/>
        /// </summary>
        /// <param name="param"></param>
        protected async void ExecuteSaveProcessedFileCommand(object param)
        {
            try
            {
                if (Preview != null) ProcessedFileLocation = await DoSaveProcessedFileWorkAsync();
                Messenger.Send(new FileProcessingSavedNotification());
            }
            catch (FileNotFoundException ex)
            {
                ExceptionViewModel.HandleExceptionCommand.Execute(new Exception("Couldn't locate the file " + ex.FileName));
            }
            catch (Exception x)
            {
                ExceptionViewModel.HandleExceptionCommand.Execute(new Exception("Something went horribly, horribly wrong with file processing!\n" + x.Message));
            }
        }

        /// <summary>
        /// Generates the preview bytes.
        /// </summary>
        /// <returns></returns>
        protected async Task GeneratePreviewAsync()
        {
            Preview = await DoGeneratePreviewWorkAsync();
        }

        /// <summary>
        /// Executes <see cref="CancelCommand"/>.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteCanceledCommand(object param) => Messenger.Send(new FileProcessingCancelNotification());

        /// <summary>
        /// Executes <see cref="ClearSourceFileCommand"/>.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteClearSourceFileCommand(object param) => SourceFileLocation = null;
    }
}