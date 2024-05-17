using System.Windows.Input;

namespace WiiUInjector.ViewModels
{
    public interface IViewModelWithNavigation
    {
        ICommand GoToNextCommand { get; }
        NavigationViewModel NavigationViewModel { get; }
    }
}