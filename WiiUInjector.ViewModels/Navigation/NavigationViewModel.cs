using System.Windows.Input;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;
using WiiUInjector.ViewModels.Services;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// Class that helps with navigation between elements in an injection application.
    /// </summary>
    public sealed class NavigationViewModel : ViewModel
    {
        private readonly INavigationService _service;

        public string CurrentPage => _service.CurrentPage;
        public ICommand NavigateCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ForwardCommand { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="NavigationViewModel"/> class.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="gameBaseManager"></param>
        public NavigationViewModel(INavigationService service, ExceptionViewModel exceptionViewModel) : base(exceptionViewModel)
        {
            _service = service;
            this.BackCommand = new BasicCommand(ExecuteBackCommand, (x) => true);
            this.ForwardCommand = new BasicCommand(ExecuteForwardCommand, (x) => true);
            this.NavigateCommand = new BasicCommand(ExecuteNavigationCommand, (x) => true);

            // listen for some messages from other view models.
            Messenger.Register<BaseSelectedNotification>(n =>
            {
                _service.GoToConfig(n.Selection.Console);
                RaisePropertyChange(nameof(CurrentPage));
            });
            Messenger.Register<ConfigReadyNotification>(n =>
            {
                _service.GoToInjection();
                RaisePropertyChange(nameof(CurrentPage));
            });
            Messenger.Register<FileProcessingStartNotification>(n => NavigateCommand.Execute(n.FileProcessingPage));
            Messenger.Register<FileProcessingCancelNotification>(n => BackCommand.Execute(null));
            Messenger.Register<FileProcessingSavedNotification>(n => BackCommand.Execute(null));
            Messenger.Register<InjectionConfirmedNotification>(n => GoHome());
            Messenger.Register<NavigateBackNotification>(n => BackCommand.Execute(null));
        }

        /// <summary>
        /// Navigates all the way back to the home page.
        /// </summary>
        private void GoHome()
        {
            string page = null;
            while(page != _service.CurrentPage)
            {
                page = _service.CurrentPage;
                _service.Back();
            }
            RaisePropertyChange(nameof(CurrentPage));
        }

        /// <summary>
        /// Execute the go back command.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteBackCommand(object param)
        {
            if (param != null && int.TryParse(param.ToString(), out int i))
            {
                for (int x = 0; x < i; x++)
                {
                    _service.Back();
                }
            }
            else
            {
                _service.Back();
            }
            RaisePropertyChange(nameof(CurrentPage));
        }

        /// <summary>
        /// Execute the go forward command.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteForwardCommand(object param)
        {
            if (int.TryParse(param.ToString(), out int i))
            {
                for (int x = 0; x < i; x++)
                {
                    _service.Forward();
                }
            }
            else
            {
                _service.Forward();
            }
            RaisePropertyChange(nameof(CurrentPage));
        }

        /// <summary>
        /// Execute the navgiation command.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteNavigationCommand(object param)
        {
            _service.Navigate(param?.ToString());
            RaisePropertyChange(nameof(CurrentPage));
        }
    }
}
