using System.IO;
using System.Threading.Tasks;
using WiiUInjector.GitTools.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class Tg16Injector : Injector<Tg16Config>
    {
        private readonly ITool _cdTool = ToolBox.Tools.TurboCdTool;
        private readonly ITool _tool = ToolBox.Tools.TurboTool;

        /// <summary>
        /// Creates a new instance of the <see cref="Tg16Injector"/> class.
        /// </summary>
        public Tg16Injector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for an TG16 roms.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(Tg16Config config, BaseRom injectionBase, string directory, bool force)
        {
            ITool tool = config.Cd ? _cdTool : _tool;
            string args = config.Cd ? "$\"test\"" : $"\"{config.RomPath}\"";
            if (config.Cd)
            {
                await Task.Run(() =>
                {
                    Directory.CreateDirectory("test");
                    DirectoryCopy(config.RomPath, "test");
                });
            }
            var  r = await tool.UseAsync(args);

            if (Directory.Exists("test")) Directory.Delete("test", true);

            // swap pce.pkg in injection directory with what we ran.
            File.Delete(Path.Combine(directory, "content", "pceemu", "pce.pkg"));
            File.Copy("pce.pkg", Path.Combine(directory, "content", "pceemu", "pce.pkg"));
            File.Delete("pce.pkg");
        }
    }
}
