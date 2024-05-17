using System.Windows.Input;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model that generates previews and final results of processing a source file.
    /// </summary>
    public interface IFileProcessingViewModel : IDialogViewModel
    {
        /// <summary>
        /// Activate the generated preview bytes.
        /// </summary>
        ICommand ActivatePreviewCommand { get; }

        /// <summary>
        /// Canceling the operation.
        /// </summary>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Clears the files that were processed and saved.
        /// </summary>
        ICommand ClearProcessedFileCommand { get; }

        /// <summary>
        /// Clears the source file.
        /// </summary>
        ICommand ClearSourceFileCommand { get; }

        /// <summary>
        /// Gets the generated preview.
        /// </summary>
        byte[] Preview { get; }

        /// <summary>
        /// The saved file resulting from processing the source.
        /// </summary>
        string ProcessedFileLocation { get; }

        /// <summary>
        /// Saves the preview into a processed file.
        /// </summary>
        ICommand SaveProcessedFileCommand { get; }

        /// <summary>
        /// Full location of a source file.
        /// </summary>
        string SourceFileLocation { get; }
    }
}