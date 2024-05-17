namespace WiiUInjector.ViewModels.Notifications
{
    /// <summary>
    /// The game base definition has been selected
    /// </summary>
    public class BaseSelectedNotification 
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BaseSelectedNotification"/>
        /// </summary>
        public BaseSelectedNotification(BaseViewModel selectedBase)
        {
            Selection = selectedBase;
        }

        /// <summary>
        /// The selected base rom definition.
        /// </summary>
        public BaseViewModel Selection { get; private set; }
    }
}