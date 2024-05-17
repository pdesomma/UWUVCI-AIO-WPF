using System.Windows.Input;
using WiiUInjector;

namespace WiiUInjector.ViewModels
{
    public sealed class ConsoleViewModel : ViewModel
    {
        public GameConsole GameConsole { get; private set; }
        public ICommand SelectConsoleCommand { get; private set; }
        public string ImagePath => "pack://application:,,,/UI/Images/consoles/" + GameConsole.ToString().ToLower() + ".png";
        public string Description { get; private set; }
        public NavigationViewModel Navigation { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleViewModel"/> class.
        /// </summary>
        /// <param name="console"></param>
        public ConsoleViewModel(GameConsole console, string desc, NavigationViewModel navigationViewModel, ICommand selectConsoleCommand, ExceptionViewModel exceptionViewModel)
            : base(exceptionViewModel)
        {
            GameConsole = console;
            Description = desc;
            Navigation = navigationViewModel;
            SelectConsoleCommand = selectConsoleCommand;
        }
    }
}