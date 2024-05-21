using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WiiUInjector.GitTools.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class SnesInjector : Injector<SnesConfig>
    {
        private readonly ITool _nesInjectorTool = ToolBox.Tools.NesInjectorTool;
        private readonly ITool _aspectRatioTool = ToolBox.Tools.RpxAspectRatioTool;
        private readonly ITool _compressorTool = ToolBox.Tools.RpxCompressorTool;
        private string _temporaryFilePath;

        /// <summary>
        /// Creates a new instance of the <see cref="SnesInjector"/> class.
        /// </summary>
        /// <param name="config"></param>
        public SnesInjector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Do header editing work before we get going.
        /// </summary>
        /// <param name="directory"></param>
        protected override void Preprocess(SnesConfig config, BaseRom injectionBase)
        {
            // logic taken from snesROMUtil
            using (FileStream inStream = new FileStream(config.RomPath, FileMode.Open))
            {
                byte[] header = new byte[512];
                inStream.Read(header, 0, header.Length);
                if (BitConverter.ToString(header, 8, 3) != "AA-BB-04" &&
                    Encoding.ASCII.GetString(header, 0, 11) != "GAME DOCTOR"
                    && BitConverter.ToString(header, 30, 16) != "00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00") return;

                _temporaryFilePath = Path.Combine(Path.GetDirectoryName(config.RomPath), "_" + Path.GetFileName(config.RomPath));
                if (File.Exists(_temporaryFilePath)) File.Delete(_temporaryFilePath);
                using (FileStream outStream = new FileStream(_temporaryFilePath, FileMode.OpenOrCreate))
                {
                    inStream.CopyTo(outStream);
                }
                config.RomPath = _temporaryFilePath;
            }
        }

        /// <summary>
        /// Delete the temp file path
        /// </summary>
        /// <param name="directory"></param>
        protected override void Postprocess(SnesConfig config, BaseRom injectionBase)
        {
            if(File.Exists(_temporaryFilePath)) File.Delete(_temporaryFilePath);
        }

        /// <summary>
        /// Does injection work for a SNES rom.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(SnesConfig config, BaseRom injectionBase, string directory, bool force)
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
