
using System.Windows.Input;
using WiiUInjector.ViewModels.Commands;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// Base view model with navigation
    /// </summary>
    public abstract class ViewModelWithNavigation : ViewModel, IViewModelWithNavigation
    {
        public NavigationViewModel NavigationViewModel { get; private set; }

        /// <summary>
        /// Moves to the next view model.
        /// </summary>
        public ICommand GoToNextCommand { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ViewModelWithNavigation"/> class.
        /// </summary>
        /// <param name="navigationViewModel"></param>
        /// <param name="exceptionViewModel"></param>
        public ViewModelWithNavigation(NavigationViewModel navigationViewModel, ExceptionViewModel exceptionViewModel) : base(exceptionViewModel)
        {
            NavigationViewModel = navigationViewModel;
            GoToNextCommand = new BasicCommand(ExecuteGoToNextCommand, (x) => true);
        }

        /// <summary>
        /// Checks if it's ok to move on to the next page.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsOkToProceed()
        {
            return true;
        }

        /// <summary>
        /// Executes the <see cref="GoToNextCommand"/>.
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteGoToNextCommand(object parameter)
        {
            if (IsOkToProceed() && parameter != null) NavigationViewModel.NavigateCommand.Execute(parameter);
        }

    }
}