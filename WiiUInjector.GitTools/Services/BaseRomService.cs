using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WiiUInjector.Repos;
using WiiUInjector.Services;

namespace WiiUInjector.GitTools.Services
{
    public sealed class BaseRomService : IBaseRomService
    {
        private static readonly string s_directory = Path.Combine(Directory.GetCurrentDirectory(), "roms");
        private static readonly string s_temp = Path.Combine(s_directory, "temp");
        private readonly ITool _romDownloadTool = ToolBox.Tools.RomDownloaderTool;
        private readonly ITool _decryptTool = ToolBox.Tools.DecryptTool;
        private readonly IBaseRomDefinitionRepo _repo;      

        /// <summary>
        /// Creates a new instance of the <see cref="BaseRomService"/> class.
        /// </summary>
        public BaseRomService(IBaseRomDefinitionRepo repo)
        {
            _repo = repo;
            if (!Directory.Exists(s_directory)) Directory.CreateDirectory(s_directory);
        }

        /// <summary>
        /// Downloads base rom binaries.
        /// </summary>
        /// <param name="baseRomDef"></param>
        /// <returns></returns>
        public async Task<string> DownloadBinaryAsync(string commonKey, BaseRom baseRomDef)
        {
            if (!Directory.Exists(s_temp)) Directory.CreateDirectory(s_temp);

            await _romDownloadTool.UseAsync($"{baseRomDef.TitleId} {baseRomDef.TitleKey} \"{Path.Combine(s_temp, "download")}\"");

            string dest = Path.Combine(s_directory, baseRomDef.Console.ToString(), baseRomDef.TitleId);
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            var decrpytArgs = $"{commonKey} \"{Path.Combine(s_temp, "download")}\" \"{dest}\"";
            await _decryptTool.UseAsync(decrpytArgs);

            // extra processing for wii/gamecube
            if (baseRomDef.Console == GameConsole.WII || baseRomDef.Console == GameConsole.GCN)
            {
                foreach (var file in Directory.GetFiles(Path.Combine(dest, "content"), "*.nfs")) File.Delete(file);
            }
            Directory.Delete(s_temp, true);
            baseRomDef.Path = dest;
            await UpdateAsync(baseRomDef);
            return dest;
        }

        /// <summary>
        /// Gets <see cref="BaseRom"/>s for a <see cref="GameConsole"/>.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BaseRom>> GetBaseRomDefinitionsAsync(GameConsole console) => await _repo.GetAsync(console);

        /// <summary>
        /// Updates a <see cref="BaseRom"/> in  the system.
        /// </summary>
        /// <param name="rom"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BaseRom rom) => await _repo.UpdateAsync(rom);
    }
}
