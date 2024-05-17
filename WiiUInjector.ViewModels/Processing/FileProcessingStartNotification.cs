namespace WiiUInjector.ViewModels.Notifications
{
    /// <summary>
    /// Staring to process a file for metadata.
    /// </summary>
    public class FileProcessingStartNotification
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FileProcessingStartNotification"/> class.
        /// </summary>
        /// <param name="fileProcessingPage"></param>
        public FileProcessingStartNotification(string fileProcessingPage)
        {
            FileProcessingPage = fileProcessingPage;
        }

        /// <summary>
        /// Page we need to see to process the file.
        /// </summary>
        public string FileProcessingPage { get; private set; }
    }
}