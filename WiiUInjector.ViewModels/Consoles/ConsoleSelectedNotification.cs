using WiiUInjector;

namespace WiiUInjector.ViewModels.Notifications
{
    /// <summary>
    /// The console has been selected
    /// </summary>
    public class ConsoleSelectedNotification 
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConsoleSelectedNotification"/>
        /// </summary>
        public ConsoleSelectedNotification(GameConsole selection)
        {
            Selection = selection;
        }

        /// <summary>
        /// The selected console. 
        /// </summary>
        public GameConsole Selection { get; private set; }
    }
}