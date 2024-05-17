using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using WiiUInjector.Configs;

namespace WiiUInjector.GitTools
{
    internal sealed class NdsInjector : Injector<NdsConfig>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NdsInjector"/> class.
        /// </summary>
        public NdsInjector(string workingDirectory) : base(workingDirectory) { }

        /*
         * GameConsole: Can either be NDS, N64, GBA, NES, SNES or TG16
         * baseRom = Name of the BaseRom, which is the folder name too (example: Super Metroid EU will be saved at the BaseRom path under the folder SMetroidEU, so the BaseRom is in this case SMetroidEU).
         * customBasePath = Path to the custom Base. Is null if no custom base is used.
         * injectRomPath = Path to the Rom to be injected into the Base Game.
         * gameName = The name of the final game to be entered into the .xml files.
         * iniPath = Only used for N64. Path to the INI configuration. If "blank", a blank ini will be used.
         * darkRemoval = Only used for N64. Indicates whether the dark filter should be removed.
         */

        /// <summary>
        /// Does injection work for a NintndoDS rom.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(NdsConfig config, BaseRom injectionBase, string directory, bool force)
        {
            string zipRomName = string.Empty;            
            var zipLocation = Path.Combine(directory, "content", "0010", "rom.zip");
            using (var zip = ZipFile.Open(zipLocation, ZipArchiveMode.Read))
            {
                foreach (var file in zip.Entries)
                {
                    if (file.Name.Contains("WUP"))
                    {
                        zipRomName = file.Name;
                        break;
                    }
                }   
            }
                
            File.Delete(zipLocation);
            await Task.Run(() =>
            {
                using (var stream = new FileStream(zipLocation, FileMode.Create))
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        archive.CreateEntryFromFile(config.RomPath, Path.GetFileName(zipRomName));
                    }
                }
            });
        }
    }
}
