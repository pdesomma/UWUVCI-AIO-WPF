using System.Collections.Generic;
using WiiUInjector;
using WiiUInjector.ViewModels.Services;

namespace UWUVCI_AIO_WPF
{
    /// <summary>
    /// Magic strings that represent page names.
    /// </summary>
    public static class PageNames
    {
        public static readonly string BaseRom = "BaseRomPage";
        public static readonly string Bootsound = "BootsoundPage";
        public static readonly string Gamepad = "GamepadPage";
        public static readonly string GbaConfig = "GbaConfigPage";
        public static readonly string Icon = "IconPage";
        public static readonly string Injection = "InjectionPage";
        public static readonly string Logo = "LogoPage";
        public static readonly string MsxConfig = "MsxConfigPage";
        public static readonly string N64Config = "N64Page";
        public static readonly string NdsConfig = "NdsConfigPage";
        public static readonly string NesConfig = "NesConfigPage";
        public static readonly string Start = "StartPage";
        public static readonly string Tg16 = "Tg16Page";
        public static readonly string Tv = "TvPage";
    }

    /// <summary>
    /// Navigates from page to page.
    /// </summary>
    public class PageNavigationService : INavigationService
    {
        private readonly Stack<string> _stack = new Stack<string>();
        private readonly Stack<string> _forwardStack = new Stack<string>();
        
        /// <summary>
        /// Creates a new instance of the <see cref="PageNavigationService"/> class.
        /// </summary>
        public PageNavigationService() { }

        /// <summary>
        /// The name of the current page.
        /// </summary>
        public string CurrentPage => _stack.Count > 0 ? _stack.Peek() : null;

        /// <summary>
        /// Navigate back.
        /// </summary>
        public void Back()
        {
            if(_stack.Count > 1) _stack.Pop();
        }

        /// <summary>
        /// Navigate forward.
        /// </summary>
        public void Forward()
        {
            if(_forwardStack.Count >= 1) _stack.Push(_forwardStack.Pop());;
        }

        /// <summary>
        /// Navigates to the correct config page.
        /// </summary>
        public void GoToConfig(GameConsole console)
        {
            switch(console)
            {
                case (GameConsole.NDS):
                    Navigate(PageNames.NdsConfig);
                    break;
                case (GameConsole.SNES):
                case (GameConsole.NES):
                    Navigate(PageNames.NesConfig);
                    break;
                case (GameConsole.GBA):
                    Navigate(PageNames.GbaConfig);
                    break;
                case (GameConsole.MSX):
                    Navigate(PageNames.MsxConfig);
                    break;
                case (GameConsole.TG16):
                    Navigate(PageNames.Tg16);
                    break;
                case (GameConsole.N64):
                    Navigate(PageNames.N64Config);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Navigates to the injection page.
        /// </summary>
        public void GoToInjection() => this.Navigate(PageNames.Injection);

        /// <summary>
        /// Navigate to a specific page.
        /// </summary>
        /// <param name="destination"></param>
        public void Navigate(string destination)
        {
            _stack.Push(destination);
            _forwardStack.Clear();
        }
    }

}
