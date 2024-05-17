using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WiiUInjector.Services;

namespace WiiUInjector.GitTools.Services
{
    /// <summary>
    /// Generates bitmap byte arrays for use as future tgas.
    /// </summary>
    public sealed class MetadataService : IMetadataService
    {
        private readonly ITool _jpgTool = ToolBox.Tools.JpgTgaTool;
        private readonly ITool _bmpTool = ToolBox.Tools.BmpTgaTool;
        private readonly ITool _pngTool = ToolBox.Tools.PngTgaTool;
        private readonly ITool _verifierTool = ToolBox.Tools.TgaVerifierTool;
        private readonly ITool _trimTool = ToolBox.Tools.SoxTrimTool;

        private static readonly string s_directory = Path.Combine(Directory.GetCurrentDirectory(), "meta");
        private static readonly string s_bootSndTempFileName = "bootSound.wav";
        private static readonly string s_iconTempFileName = "iconTex.png";
        private static readonly string s_tvTempFileName = "bootTvTex.png";
        private static readonly string s_gamePadTempFileName = "bootDrcTex.png";
        private static readonly string s_logoTempFileName = "bootLogoTex.png";

        public byte[] BootSoundPreview => File.Exists(Path.Combine(s_directory, s_bootSndTempFileName)) ? File.ReadAllBytes(Path.Combine(s_directory, s_bootSndTempFileName)) : null;
        public byte[] GamePadPreview => File.Exists(Path.Combine(s_directory, s_gamePadTempFileName)) ? File.ReadAllBytes(Path.Combine(s_directory, s_gamePadTempFileName)) : null;
        public byte[] IconPreview=> File.Exists(Path.Combine(s_directory, s_iconTempFileName)) ? File.ReadAllBytes(Path.Combine(s_directory, s_iconTempFileName)) : null;
        public byte[] LogoPreview => File.Exists(Path.Combine(s_directory, s_logoTempFileName)) ? File.ReadAllBytes(Path.Combine(s_directory, s_logoTempFileName)) : null;
        public byte[] TvPreview => File.Exists(Path.Combine(s_directory, s_tvTempFileName)) ? File.ReadAllBytes(Path.Combine(s_directory, s_tvTempFileName)) : null;
        
        /// <summary>
        /// Creates a new instance of the <see cref="MetadataService"/> class.
        /// </summary>
        public MetadataService()
        {
            if (Directory.Exists(s_directory)) Directory.Delete(s_directory, true);
            Directory.CreateDirectory(s_directory);
        }

        /// <summary>
        /// Clears the generated boot sound.
        /// </summary>
        public void ClearBootSound() => Clear(s_bootSndTempFileName, MetadataFileNames.Bootsound);

        /// <summary>
        /// Clears the latest saved game pad image.
        /// </summary>
        public void ClearGamePad() => Clear(s_gamePadTempFileName, MetadataFileNames.GamePad);

        /// <summary>
        /// Clears the latest saved icon image.
        /// </summary>
        public void ClearIcon() => Clear(s_iconTempFileName, MetadataFileNames.Icon);

        /// <summary>
        /// Clears the latest saved logo image.
        /// </summary>
        public void ClearLogo() => Clear(s_logoTempFileName, MetadataFileNames.Logo);

        /// <summary>
        /// Clears the latest saved tv image.
        /// </summary>
        public void ClearTv() => Clear(s_tvTempFileName, MetadataFileNames.Tv);

        /// <summary>
        /// Final operations before we get going with injections.
        /// </summary>
        /// <returns></returns>
        public async Task FinalizeAsync()
        {
            if(!File.Exists(Path.Combine(s_directory, MetadataFileNames.GamePad)))
            {
                // make a gamepad tga from the tv image.
                if(File.Exists(Path.Combine(s_directory, s_tvTempFileName)))
                {
                    // copy the tv preview png and call the usual creation method.
                    File.Copy(Path.Combine(s_directory, s_tvTempFileName), Path.Combine(s_directory, s_gamePadTempFileName));
                    await SaveGamePadTgaAsync();
                }
            }
            await VerifyAsync();
        }

        /// <summary>
        /// Generates a preview of the boot sound.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public async Task GenerateBootSoundPreviewAsync(string sourceFilePath)
        {
            if (sourceFilePath is null) return;
            if (!File.Exists(sourceFilePath)) throw new FileNotFoundException(sourceFilePath);

            await _trimTool.UseAsync($"\"{sourceFilePath}\" -b 16 \"{Path.Combine(s_directory, s_bootSndTempFileName)}\" channels 2 rate 48k trim 0 6");

            if (!File.Exists(Path.Combine(s_directory, s_bootSndTempFileName))) throw new FileNotFoundException(Path.Combine(s_directory, s_bootSndTempFileName));
        }

        /// <summary>
        /// Generates a game pad image preview.
        /// </summary>
        /// <returns></returns>
        public async Task GenerateGamePadPreviewAsync(string source, string overlay, string name1, string name2, int year, int players)
        {
            await SavePngAsync(s_gamePadTempFileName, new BootPngGenerator(source, overlay, name1, name2, year, players).Create());
        }

        /// <summary>
        /// Generates an icon image preview.
        /// </summary>
        /// <returns></returns>
        public async Task GenerateIconPreviewAsync(string source, string overlay)
        {
            await SavePngAsync(s_iconTempFileName, new IconPngGenerator(source, overlay).Create());
        }

        /// <summary>
        /// Creates a byte array representing a logo image bitmap.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        public async Task GenerateLogoPreviewAsync(string overlay, string text, float fontSize)
        {
            await SavePngAsync(s_logoTempFileName, new LogoPngGenerator(overlay, text, fontSize).Create());
        }
        
        /// <summary>
        /// Creates a byte array representing a tv image bitmap.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        public async Task GenerateTvPreviewAsync(string source, string overlay, string name1, string name2, int year, int players)
        {
            await SavePngAsync(s_tvTempFileName, new BootPngGenerator(source, overlay, name1, name2, year, players).Create());
        }

        /// <summary>
        /// Gets a list of file paths to icon overlays.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        public List<string> GetIconTemplates(GameConsole console)
        {
            var templates = new List<string>();
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Styles", "Icon");

            templates.Add(Path.Combine(directory, "default.png"));
            foreach (var file in Directory.GetFiles(Path.Combine(directory, console.ToString())))
            {
                templates.Add(file);
            }
            return templates;
        }

        /// <summary>
        /// Gets a list of file paths to tv image overlays.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        public List<string> GetTvTemplates(GameConsole console)
        {
            var templates = new List<string>();
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Styles", "Tv");

            foreach (var file in Directory.GetFiles(Path.Combine(directory, console.ToString())))
            {
                templates.Add(file);
            }
            return templates;
        }

        /// <summary>
        /// Gets a string path to the logo image overlay.
        /// </summary>
        /// <returns></returns>
        public string GetLogoTemplate() => Path.Combine(Directory.GetCurrentDirectory(), "Images", "Styles", "logo.png");


        /// <summary>
        /// Save the preview to a boot sound.
        /// </summary>
        /// <returns></returns>
        public string SaveBootSound()
        {
            var bytes = BootSoundPreview;
            if (bytes == null) return null;

            using (FileStream output = new FileStream(Path.Combine(s_directory, MetadataFileNames.Bootsound), FileMode.OpenOrCreate))
            using (BinaryWriter writer = new BinaryWriter(output))
            {
                writer.Write(new byte[] { 0x0, 0x0, 0x0, 0x2, 0x0, 0x0, 0x0, 0x0 });
                for (int i = 0x2C; i < bytes.Length; i += 2)
                {
                    writer.Write(new[] { bytes[i + 1], bytes[i] });
                }
                output.Close();
                writer.Close();
                return Path.Combine(s_directory, MetadataFileNames.Bootsound);
            }
        }

        /// <summary>
        /// Create a gamepad tga file from the latest saved preview image.
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveGamePadTgaAsync()
        {
            if (!File.Exists(Path.Combine(s_directory, s_gamePadTempFileName))) throw new FileNotFoundException(Path.Combine(s_directory, s_gamePadTempFileName));
            await ConvertPngToTgaAsync(Path.Combine(s_directory, s_gamePadTempFileName), s_directory, MetadataFileNames.GamePad, 854, 480, 24, false);
            return Path.Combine(s_directory, MetadataFileNames.GamePad);
        }

        /// <summary>
        /// Create an icon tga file from the latest saved preview image.
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveIconTgaAsync()
        {
            if (!File.Exists(Path.Combine(s_directory, s_iconTempFileName))) throw new FileNotFoundException(Path.Combine(s_directory, s_iconTempFileName));
            await ConvertPngToTgaAsync(Path.Combine(s_directory, s_iconTempFileName), s_directory, MetadataFileNames.Icon, 128, 128, 32, false);
            return Path.Combine(s_directory, MetadataFileNames.Icon);
        }

        /// <summary>
        /// Create a logo tga file from the latest saved preview image.
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveLogoTgaAsync()
        {
            if (!File.Exists(Path.Combine(s_directory, s_logoTempFileName))) throw new FileNotFoundException(Path.Combine(s_directory, s_logoTempFileName));
            await ConvertPngToTgaAsync(Path.Combine(s_directory, s_logoTempFileName), s_directory, MetadataFileNames.Logo, 170, 42, 32, false);
            return Path.Combine(s_directory, MetadataFileNames.Logo);
        }

        /// <summary>
        /// Create a tv tga file from the latest saved preview image.
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveTvTgaAsync()
        {
            if (!File.Exists(Path.Combine(s_directory, s_tvTempFileName))) throw new FileNotFoundException(Path.Combine(s_directory, s_tvTempFileName));
            await ConvertPngToTgaAsync(Path.Combine(s_directory, s_tvTempFileName), s_directory, MetadataFileNames.Tv, 1280, 720, 24, false);
            return Path.Combine(s_directory, MetadataFileNames.Tv);
        }

        /// <summary>
        /// Clears files.
        /// </summary>
        /// <param name="tempPath"></param>
        /// <param name="livePath"></param>
        private static void Clear(string tempPath, string livePath)
        {
            if (File.Exists(Path.Combine(s_directory, livePath))) File.Delete(Path.Combine(s_directory, livePath));
            if (File.Exists(Path.Combine(s_directory, tempPath))) File.Delete(Path.Combine(s_directory, tempPath));
        }

        /// <summary>
        /// Creates a tga file from a source file.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFileDirectory"></param>
        /// <param name="destinationFileName"></param>
        /// <param name="deleteOriginal"></param>
        /// <returns></returns>
        /// <exception cref="BadImageFormatException"></exception>
        private async Task ConvertPngToTgaAsync(string sourceFilePath, string destinationFileDirectory, string destinationFileName, int width, int height, int bit, bool deleteOriginal)
        {
            ITool tool = null;
            var ext = Path.GetExtension(sourceFilePath).ToLower();
            switch (ext)
            {
                case (".jpg"):
                case (".jpeg"):
                    tool = _jpgTool; break;
                case (".png"):
                    tool = _pngTool; break;
                case (".bmp"):
                    tool = _bmpTool; break;
                default:
                    break;
            }
            if (tool is null) throw new BadImageFormatException("File type should be jpg/jpeg/bmp/png. Type " + ext + " is not supported");

            var args = $"-i \"{sourceFilePath}\" -o \"{destinationFileDirectory}\" --width={width} --height={height} --tga-bpp={bit} --tga-compression=none";
            await tool.UseAsync(args);

            var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            if (File.Exists(Path.Combine(destinationFileDirectory, fileName, ".tga")))
            {
                File.Move(Path.Combine(destinationFileDirectory, fileName, ".tga"), Path.Combine(destinationFileDirectory, destinationFileName));
            }
            if (deleteOriginal) File.Delete(sourceFilePath);
        }

        /// <summary>
        /// Saves a byte array to the disk as a png file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task SavePngAsync(string name, byte[] bytes)
        {
            if (bytes is null) return;
            using (var fileStream = new FileStream(Path.Combine(s_directory, name), FileMode.Create))
            {
                await fileStream.WriteAsync(bytes, 0, bytes.Length);
                fileStream.Close();
            }
        }

        /// <summary>
        /// Verify tga files.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> VerifyAsync()
        {
            _verifierTool.OnCompletion += async (response) =>
            {
                if (response.Output.Contains("width") || response.Output.Contains("height") || response.Output.Contains("depth")) throw new Exception("Size");
                if (response.Error.Contains("width") || response.Error.Contains("height") || response.Error.Contains("depth")) throw new Exception("Size");

                if (response.Error.Contains("TRUEVISION") || response.Output.Contains("TRUEVISION"))
                {
                    // guess you have to run it twice
                    await _verifierTool.UseAsync($"\"{s_directory}\"");
                }
            };

            await _verifierTool.UseAsync($"\"{s_directory}\"");
            return true;
        }
    }
}
