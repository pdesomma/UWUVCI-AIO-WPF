using System.Collections.Generic;
using System.Threading.Tasks;

namespace WiiUInjector.Services
{
    /// <summary>
    /// Service that generates Bitmaps to later user for tga files.
    /// </summary>
    public interface IMetadataService
    {
        byte[] GamePadPreview { get; }
        byte[] IconPreview { get; }
        byte[] LogoPreview { get; }
        byte[] TvPreview { get; }
        byte[] BootSoundPreview { get; }

        /// <summary>
        /// Clears the saved boot sound file on the disk.
        /// </summary>
        void ClearBootSound();

        /// <summary>
        /// Clears the preview and final gamepad tga from the disk.
        /// </summary>
        void ClearGamePad();

        /// <summary>
        /// Clears the preview and final icon tga from the disk.
        /// </summary>
        void ClearIcon();

        /// <summary>
        /// Clears the preview and final logo tga from the disk.
        /// </summary>
        void ClearLogo();

        /// <summary>
        /// Clears the preview and final tv tga from the disk.
        /// </summary>
        void ClearTv();

        /// <summary>
        /// Generates a game pad image preview.
        /// </summary>
        /// <returns></returns>
        Task GenerateGamePadPreviewAsync(string source, string overlay, string name1, string name2, int year, int players);

        /// <summary>
        /// Generates an icon image preview.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="overlay"></param>
        Task GenerateIconPreviewAsync(string source, string overlay);

        /// <summary>
        /// Generates a logo image preview.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        Task GenerateLogoPreviewAsync(string overlay, string text, float fontSize);

        /// <summary>
        /// Generates a tv image preview.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        Task GenerateTvPreviewAsync(string source, string overlay, string name1, string name2, int year, int players);

        /// <summary>
        /// Creates a trimmed preview of the boot sound
        /// </summary>
        /// <param name="sourceFilePath"></param>
        Task GenerateBootSoundPreviewAsync(string sourceFilePath);

        /// <summary>
        /// Gets a list of available icon template overlays.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        List<string> GetIconTemplates(GameConsole console);

        /// <summary>
        /// Gets a list of available icon template overlays.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        List<string> GetTvTemplates(GameConsole console);

        /// <summary>
        /// Gets the logo template overlay.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        string GetLogoTemplate();

        /// <summary>
        /// Does final setup before an injection.
        /// </summary>
        /// <returns></returns>
        Task FinalizeAsync();

        /// <summary>
        /// Saves the latest generated bootsound to the disk.
        /// </summary>
        string SaveBootSound();

        /// <summary>
        /// Saves the latest generated gamepad image to the disk.
        /// </summary>
        Task<string> SaveGamePadTgaAsync();

        /// <summary>
        /// Saves the latest generated icon image to the disk.
        /// </summary>
        Task<string> SaveIconTgaAsync();

        /// <summary>
        /// Saves the latest generated logo image to the disk as a tga.
        /// </summary>
        Task<string> SaveLogoTgaAsync();

        /// <summary>
        /// Saves the latest generated tv image to the disk.
        /// </summary>
        Task<string> SaveTvTgaAsync();
    }
}
