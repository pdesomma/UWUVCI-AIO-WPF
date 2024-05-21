using System;
using System.IO;
using System.Threading.Tasks;
using WiiUInjector.GitTools.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class NesInjector : Injector<NesConfig>
    {
        private readonly ITool _nesInjectorTool = ToolBox.Tools.NesInjectorTool;
        private readonly ITool _aspectRatioTool = ToolBox.Tools.RpxAspectRatioTool;
        private readonly ITool _compressorTool = ToolBox.Tools.RpxCompressorTool;

        /// <summary>
        /// Creates a new instance of the <see cref="NesInjector"/> class.
        /// </summary>
        public NesInjector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for an NES rom.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(NesConfig config, BaseRom injectionBase, string directory, bool force)
        {
            string rpxFile = Directory.GetFiles(Path.Combine(directory, "code"), "*.rpx")[0]; //To get the RPX path where the NES/SNES rom needs to be Injected in            
            SetStatus("Decompressing RPX...");
            await DecompressAsync(rpxFile);

            if (config.PixelPerfect)
            {
                SetStatus("Applying Pixel Perfect Patches...");
                await ChangeAspectRatioAsync(rpxFile);
            }

            SetStatus("Injecting ROM...");
            _nesInjectorTool.OnCompletion += NesInjectorTool_OnCompletion;
            await _nesInjectorTool.UseAsync($"\"{rpxFile}\" \"{config.RomPath}\" \"{rpxFile}\"");
            _nesInjectorTool.OnCompletion -= NesInjectorTool_OnCompletion;

            await CompressAsync(rpxFile);
            SetStatus("");
        }

        /// <summary>
        /// Changes the aspect ratio on a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException" />
        private async Task ChangeAspectRatioAsync(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            await _aspectRatioTool.UseAsync($"\"{path}\"");
        }

        /// <summary>
        /// Compresses rpx.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException" />
        private async Task CompressAsync(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            var res = await _compressorTool.UseAsync($"-c \"{path}\"");
        }

        /// <summary>
        /// Decompresses rpx.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException" />
        private async Task DecompressAsync(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            var res = await _compressorTool.UseAsync("-d \"" + path + "\"");
        }

        /// <summary>
        /// Check for largeness.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="events"></param>
        /// <exception cref="Exception"></exception>
        private void NesInjectorTool_OnCompletion(ToolResponse response)
        {
            if (response.Error.Contains("is too large") || response.Output.Contains("is too large")) throw new Exception("Too large");
        }
    }
}
