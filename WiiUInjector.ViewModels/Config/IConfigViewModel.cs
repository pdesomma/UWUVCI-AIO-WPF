using System.Windows.Input;
using WiiUInjector.GitTools.Configs;
using WiiUInjector.ViewModels.BootSound;
using WiiUInjector.ViewModels.Tga;

namespace WiiUInjector.ViewModels.Configs
{
    public interface IConfigViewModel
    {
        /// <summary>
        /// Builds bootsound.
        /// </summary>
        BootsoundViewModel BootsoundViewModel { get; }

        /// <summary>
        /// Underlying config object.
        /// </summary>
        Config Config { get; }

        /// <summary>
        /// Confirms the config is ready.
        /// </summary>
        ICommand ConfirmConfigCommand { get; }

        /// <summary>
        /// Builds gamepad tgas.
        /// </summary>
        TgaViewModel GamepadViewModel { get; }

        /// <summary>
        /// Builds icon tgas.
        /// </summary>
        TgaViewModel IconViewModel { get; }

        /// <summary>
        /// Builds logo tgas.
        /// </summary>
        TgaViewModel LogoViewModel { get; }

        /// <summary>
        /// Config game name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Go to a file processing element.
        /// </summary>
        ICommand NavigateToFileProcessingCommand { get; }

        /// <summary>
        /// Opens a file dialog to get the rom path.
        /// </summary>
        ICommand OpenRomPathFileDialogCommand { get; }

        /// <summary>
        /// Config file location of rom to inject.
        /// </summary>
        string RomPath { get; set; }

        /// <summary>
        /// Builds tv tgas.
        /// </summary>
        TgaViewModel TvViewModel { get; }
    }
}