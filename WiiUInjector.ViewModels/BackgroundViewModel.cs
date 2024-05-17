using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model that handles background tasks.
    /// </summary>
    public sealed class BackgroundViewModel: ViewModel, IBackgroundTaskViewModel
    {
        private readonly Queue<Task> _queue = new Queue<Task>();
        private readonly Queue<string> _messages = new Queue<string>();
        private bool _running = false;

        /// <summary>
        /// Creates a new instance of the <see cref="BackgroundViewModel"/> class.
        /// </summary>
        /// <param name="navigationViewModel"></param>
        /// <param name="exceptionViewModel"></param>
        public BackgroundViewModel(ExceptionViewModel exceptionViewModel) : base(exceptionViewModel) { }

        public bool IsWorking => _queue.Count > 0;
        public string Message { get; private set; }

        /// <summary>
        /// Adds something to be done in the background.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="message"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Enqueue(Task task, string message)
        {
            _queue.Enqueue(task);
            _messages.Enqueue(message);

            if (!_running) _ = RunAsync();
        }

        /// <summary>
        /// Run the enqueued tasks.
        /// </summary>
        /// <returns></returns>
        private async Task RunAsync()
        {
            _running = true;
            RaisePropertyChange(nameof(IsWorking));
            while(_queue.Count > 0)
            {
                try
                {
                    Message = _messages.Dequeue();
                    RaisePropertyChange(nameof(Message));
                    await _queue.Dequeue();
                }
                catch (Exception ex)
                {
                    ExceptionViewModel.HandleExceptionCommand.Execute(ex);
                }
            }
            _running = false;
            RaisePropertyChange(nameof(IsWorking));
            Message = null;
            RaisePropertyChange(nameof(Message));
        }
    }
}