using System.Threading.Tasks;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// A view model that handles background activity.
    /// </summary>
    public interface IBackgroundTaskViewModel : IViewModel
    {
        /// <summary>
        /// True if something is happening in the background.
        /// </summary>
        bool IsWorking { get; }

        /// <summary>
        /// Message to accompany the current running task.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Add a background task
        /// </summary>
        void Enqueue(Task task, string message);
    }
}