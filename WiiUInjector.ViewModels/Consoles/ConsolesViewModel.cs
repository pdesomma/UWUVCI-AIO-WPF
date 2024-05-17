using System.Windows.Input;
using WiiUInjector;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;

namespace WiiUInjector.ViewModels
{
    public sealed class ConsolesViewModel : ViewModel
    {
        private GameConsole _selectedConsole;
        public ICommand SelectConsoleCommand { get; private set; }

        public GameConsole SelectedConsole
        {
            get => _selectedConsole;
            set
            {
                if (_selectedConsole != value)
                {
                    SetProperty(ref _selectedConsole, value);
                }
            }
        }

        public NavigationViewModel NavigationViewModel { get; private set; }
        public ConsoleViewModel NDS { get; private set; }       
        public ConsoleViewModel NES { get; private set; }
        public ConsoleViewModel SNES { get; private set; }
        public ConsoleViewModel GBA { get; private set; }
        public ConsoleViewModel TG16 { get; private set; }
        public ConsoleViewModel MSX { get; private set; }
        public ConsoleViewModel N64 { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsolesViewModel"/> class.
        /// </summary>
        public ConsolesViewModel(NavigationViewModel nav, ExceptionViewModel exceptionViewModel)
            : base(exceptionViewModel)
        {
            NavigationViewModel = nav;
            SelectConsoleCommand = new BasicCommand(ExecuteCommand, (x) => true);
            NDS = new ConsoleViewModel(GameConsole.NDS, "Nintendo DS", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
            NES = new ConsoleViewModel(GameConsole.NES, "Nintendo Entertainment System", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
            SNES = new ConsoleViewModel(GameConsole.SNES, "Super Nintendo Entertainment System", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
            GBA = new ConsoleViewModel(GameConsole.GBA, "Game Boy", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
            TG16 = new ConsoleViewModel(GameConsole.TG16, "TurboGrafx16/CD", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
            MSX = new ConsoleViewModel(GameConsole.MSX, "MSX/2", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
            N64 = new ConsoleViewModel(GameConsole.N64, "Nintendo 64", NavigationViewModel, SelectConsoleCommand, exceptionViewModel);
        }

        /// <summary>
        /// Execute the selection command.
        /// </summary>
        /// <param name="param"></param>
        private void ExecuteCommand(object param)
        {
            SelectedConsole = (GameConsole)param;
            Messenger.Send(new ConsoleSelectedNotification(SelectedConsole));
        }
    }
}