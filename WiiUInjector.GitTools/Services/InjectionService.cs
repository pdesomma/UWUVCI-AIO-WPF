using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WiiUInjector.GitTools.Configs;
using WiiUInjector.Services;

namespace WiiUInjector.GitTools.Services
{
    public sealed class InjectionService : IInjectionService
    {
        private static readonly string s_injectDirectory = Directory.GetCurrentDirectory() + "\\injection\\";
        private static readonly string s_packDirectory = Directory.GetCurrentDirectory() + "\\packed\\";
        private readonly ITool _packTool = ToolBox.Tools.WupPackTool;

        /// <summary>
        /// Creates a new instance of the <see cref="InjectionService"/> class.
        /// </summary>
        public InjectionService() { }

        /// <summary>
        /// Create an injection for a specific console type.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<Injection> InjectAsync(GameConsole console, BaseRom baseRom, Metadata metadata)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Create a GameBoy injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectGameBoyAsync(GbaConfig config, BaseRom baseRom, Metadata metadata, bool force) => await new GbaInjector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create a GameBoy injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectGameCubeAsync(GcConfig config, BaseRom baseRom, Metadata metadata, bool force) => await new GcInjector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create an MSX injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectMsxAsync(MsxConfig config, BaseRom baseRom, Metadata metadata, bool force) => await new MsxInjector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create an N64 injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectNintendo64Async(N64Config config, BaseRom baseRom, Metadata metadata, bool force) => await new N64Injector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create an N64 injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectNintendoDsAsync(NdsConfig config, BaseRom baseRom, Metadata metadata, bool force) => await new NdsInjector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create an NES injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectNesAsync(NesConfig config, BaseRom baseRom, Metadata metadata, bool force) => await new NesInjector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create an SNES injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectSuperNintendoAsync(SnesConfig config, BaseRom baseRom, Metadata metadata, bool force) => await new SnesInjector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Create a TG16 injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public async Task<Injection> InjectTurboGrafx16Async(Tg16Config config, BaseRom baseRom, Metadata metadata, bool force) => await new Tg16Injector(s_injectDirectory + baseRom.TitleId).InjectAsync(config, baseRom, metadata, force);

        /// <summary>
        /// Runs a wup packing tool (cnus) on an injection.
        /// </summary>
        /// <param name="injection"></param>
        /// <param name="commonKey"></param>
        /// <returns></returns>
        public async Task<string> PackWupAsync(Injection injection, string commonKey)
        {
            if (!Directory.Exists(injection.Path)) return null;

            string gameName = string.IsNullOrEmpty(injection.Name) ? "NoName" : injection.Name;
            Regex reg = new Regex("[^a-zA-Z0-9 -]");
            string outputPath = Path.Combine(s_packDirectory, $"[WUP]{reg.Replace(gameName, "").Replace("|", " ")}");
            int i = 0;
            while (Directory.Exists(outputPath))
            {
                outputPath = Path.Combine(s_packDirectory, $"[WUP]{reg.Replace(gameName, "").Replace("|", " ")}_{++i}");
            }
            Directory.CreateDirectory(outputPath);

            await _packTool.UseAsync($"-in \"{injection.Path}\" -out \"{outputPath}\" -encryptKeyWith {commonKey}");
            return outputPath;
        }

        /// <summary>
        /// Makes a loadiine pack. Basically just copies files over into another output directory.
        /// </summary>
        /// <param name="injection"></param>
        /// <returns></returns>
        public async Task<string> PackLoadiineAsync(Injection injection)
        {
            if (!Directory.Exists(injection.Path)) return null;

            string gameName = string.IsNullOrEmpty(injection.Name) ? "NoName" : injection.Name;         
            Regex reg = new Regex("[^a-zA-Z0-9 é -]");

            string outputPath = Path.Combine(injection.Path, $"[LOADIINE]{reg.Replace(gameName, "")} [{injection.ProdCode}]");
            int i = 0;
            while (Directory.Exists(outputPath))
            {
                outputPath = Path.Combine(injection.Path, $"[LOADIINE]{reg.Replace(gameName, "")} [{injection.ProdCode}]_{i++}");
            }
            await Task.Run(() => DirectoryCopy(injection.Path, outputPath));
            return outputPath;
        }

        /// <summary>
        /// Deep copies a source directory to a destination directory. Creates the destination directory.
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        private static void DirectoryCopy(string sourcePath, string targetPath)
        {
            //create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
