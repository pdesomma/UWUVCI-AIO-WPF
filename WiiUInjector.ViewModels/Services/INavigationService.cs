using WiiUInjector;

namespace WiiUInjector.ViewModels.Services
{
    /// <summary>
    /// Service that helps with navigating a mvvm injection app.
    /// </summary>
    public interface INavigationService
    {
        string CurrentPage { get; }
        void Back();
        void Forward();
        void GoToConfig(GameConsole console);
        void GoToInjection();
        void Navigate(string destination);
    }
}