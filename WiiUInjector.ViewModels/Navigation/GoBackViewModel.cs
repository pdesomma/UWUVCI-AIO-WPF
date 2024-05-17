
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WiiUInjector.Messaging;
using WiiUInjector.ViewModels.Commands;
using WiiUInjector.ViewModels.Notifications;

namespace WiiUInjector.ViewModels
{
    /// <summary>
    /// View model that can navigate backwards.
    /// </summary>
    public abstract class GoBackViewModel : ViewModel
    {
        /// <summary>
        /// Create a new instance of the <see cref="GoBackViewModel"/> class.
        /// </summary>
        /// <param name="ex"></param>
        public GoBackViewModel(ExceptionViewModel ex) : base(ex) 
        {
            BackCommand = new BasicCommand(ExecuteBackCommand, x => true);
        }

        /// <summary>
        /// The go backwards command.
        /// </summary>
        public ICommand BackCommand { get; private set; }

        /// <summary>
        /// Executes the <see cref="BackCommand"/>.
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteBackCommand(object parameter)
        {
            Messenger.Send(new NavigateBackNotification());
        }
    }
}