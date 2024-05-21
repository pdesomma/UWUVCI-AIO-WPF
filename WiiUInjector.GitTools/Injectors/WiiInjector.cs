using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Management.Instrumentation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WiiUInjector.GitTools.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class WiiInjector : Injector<WiiConfig>
    {
        private readonly ITool _isoTool = ToolBox.Tools.WitTool;
        private readonly ITool _nfsIsoTool = ToolBox.Tools.Nfs2IsoTool;
        private readonly ITool _wiiVmcTool = ToolBox.Tools.WiiVmcTool;
        private readonly ITool _forceClassic = ToolBox.Tools.ForceClassicPatchTool;

        /// <summary>
        /// Creates a new instance of the <see cref="WiiInjector"/> class.
        /// </summary>
        public WiiInjector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for Gamecube roms.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(WiiConfig config, BaseRom baseRom, string directory, bool force)
        {
            switch (Path.GetExtension(config.RomPath.ToLower()))
            {
                case "dol":
                    await RunHomebrewAsync(config);
                    break;
                case "wad":
                    await RunForwardAsync(config);
                    break;
                default:
                    await RunWiiAsync(config, baseRom);
                    break;
            }
        }

        /// <summary>
        /// Do normal wii stuff.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task RunWiiAsync(WiiConfig config, BaseRom baseRom)
        {
            string savedir = Directory.GetCurrentDirectory();
            string preIsoPath = Path.Combine(tempPath, "pre.iso");

            // get an ISO to use.
            if (config.NKit)
            {
                await _isoTool.UseAsync($"copy --source \"{config.RomPath}\" --dest \"{preIsoPath}\" -I");
                if (!File.Exists(Path.Combine(ToolBox.ToolsDirectory, "out.iso"))) throw new Exception("nkit");               
                File.Move(Path.Combine(ToolBox.ToolsDirectory, "out.iso"), Path.Combine(ToolBox.ToolsDirectory, "pre.iso"));
            }
            else
            {
                if (new FileInfo(config.RomPath).Extension.Contains("wbfs"))
                {
                    await _isoTool.UseAsync($"copy --source \"{config.RomPath}\" --dest \"{preIsoPath}\" -I");
                }
                else if (new FileInfo(config.RomPath).Extension.Contains("iso"))
                {
                    File.Copy(config.RomPath, preIsoPath);
                }
            }

            // change the manual
            byte[] chars = new byte[4];
            using (var fstrm = new FileStream(preIsoPath, FileMode.Open))
            {
                fstrm.Read(chars, 0, 4);
                fstrm.Close();
            }
            string romCode = new ASCIIEncoding().GetString(chars);
            string neededForManual = ToHex(romCode);
            string metaXml = Path.Combine(baseRom.Path, "meta", "meta.xml");
            XmlDocument doc = new XmlDocument();            
            doc.Load(metaXml);
            doc.SelectSingleNode("menu/reserved_flag2").InnerText = neededForManual;
            doc.Save(metaXml);

            if (!config.DontTrim)
            {
                // figure out region frii
                if (config.RegionFriiUs)
                {
                    WriteRegionFriiBytes(preIsoPath, new byte[] { 0x01 }, new byte[] { 0x80, 0x06, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 });
                }
                else if (config.RegionFriiJp)
                {
                    WriteRegionFriiBytes(preIsoPath, new byte[] { 0x00 }, new byte[] { 0x00, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 });
                }
                else if (config.RegionFriiEu)
                {
                    WriteRegionFriiBytes(preIsoPath, new byte[] { 0x02 }, new byte[] { 0x80, 0x80, 0x80, 0x00, 0x03, 0x03, 0x04, 0x03, 0x00, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 });
                }

                // trim the rom
                await _isoTool.UseAsync($"extract \"{preIsoPath}\" --DEST \"{Path.Combine(tempPath, "TEMP")}\" --psel data -vv1");

                if (config.UseGamepadAs == WiiUseGamePadAs.ForceClassic) await PatchForceClassicAsync($"\"{Path.Combine(tempPath, "TEMP", "sys", "main.dol")}\" -nc");
                if (config.PatchLanguage)
                {
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(tempPath, "TEMP", "sys", "main.dol"), FileMode.Open)))
                    {
                        byte[] stuff = new byte[] { 0x38, 0x60 };
                        writer.Seek(0x4CBDAC, SeekOrigin.Begin);
                        writer.Write(stuff);
                        writer.Seek(0x4CBDAF, SeekOrigin.Begin);
                        stuff = new byte[] { 0x00 };
                        writer.Write(stuff);
                        writer.Close();
                    }
                }
                if (config.PatchVideo) await PatchVideoAsync(config, Path.Combine(tempPath, "TEMP", "sys"));

                // create iso from trimmed rom.
                _isoTool.UseAsync($"copy \"{Path.Combine(tempPath, "TEMP")}\" --DEST \"{Path.Combine(tempPath, "game.iso")}\" -ovv --links --iso");
                Directory.Delete(Path.Combine(tempPath, "TEMP"), true);
                File.Delete(preIsoPath);
            }
            else
            {
                if (config.UseGamepadAs == WiiUseGamePadAs.ForceClassic || config.PatchVideo)
                {
                    await _isoTool.UseAsync($"extract \"{preIsoPath}\" --DEST \"{Path.Combine(tempPath, "TEMP")}\" --psel WHOLE -vv1");
                    if (config.UseGamepadAs == WiiUseGamePadAs.ForceClassic) await PatchForceClassicAsync($"\"{Path.Combine(tempPath, "TEMP", "DATA", "sys", "main.dol")}\" -nc");
                    if (config.PatchVideo) await PatchVideoAsync(config, Path.Combine(tempPath, "TEMP", "DATA", "sys"));

                    // create iso from trimmed rom.
                    await _isoTool.UseAsync($"copy \"{Path.Combine(tempPath, "TEMP")}\" --DEST \"{Path.Combine(tempPath, "game.iso")}\" -ovv --psel WHOLE --iso");
                    Directory.Delete(Path.Combine(tempPath, "TEMP"), true);
                    File.Delete(preIsoPath);
                }
                else
                {
                    File.Move(preIsoPath, Path.Combine(tempPath, "game.iso"));
                }
            }

            // replace TIK and TMD
            await _isoTool.UseAsync($"extract \"{Path.Combine(tempPath, "game.iso")}\" --psel data --files +tmd.bin --files +ticket.bin --DEST \"{Path.Combine(tempPath, "TIKTMD")}\" -vv1");
            foreach (string sFile in Directory.GetFiles(Path.Combine(baseRom.Path, "code"), "rvlt.*"))
            {
                File.Delete(sFile);
            }
            File.Copy(Path.Combine(tempPath, "TIKTMD", "tmd.bin"), Path.Combine(baseRom.Path, "code", "rvlt.tmd"));
            File.Copy(Path.Combine(tempPath, "TIKTMD", "ticket.bin"), Path.Combine(baseRom.Path, "code", "rvlt.tik"));
            Directory.Delete(Path.Combine(tempPath, "TIKTMD"), true);

            // actually inject
            foreach (string sFile in Directory.GetFiles(Path.Combine(baseRom.Path, "content"), "*.nfs")) { File.Delete(sFile); }
            File.Move(Path.Combine(tempPath, "game.iso"), Path.Combine(baseRom.Path, "content", "game.iso"));
            var nfsIsoTool = _nfsIsoTool.Copy(Path.Combine(baseRom.Path, "content"));
            Directory.SetCurrentDirectory(Path.Combine(baseRom.Path, "content"));
            var extra = 
                config.UseGamepadAs == WiiUseGamePadAs.HorizontalWiiMote ? "-horizontal " :
                config.UseGamepadAs == WiiUseGamePadAs.VerticalWiiMote ? "-wiimote " :
                config.UseGamepadAs == WiiUseGamePadAs.ForceClassic ? "-instantcc " :
                config.UseGamepadAs == WiiUseGamePadAs.ForceNoClassic ? "-nocc" : "";
            extra += config.LR ? "-lrpatch " : "";
            await nfsIsoTool.UseAsync($"-enc {extra}-iso game.iso");
            nfsIsoTool.Delete();
            File.Delete("game.iso");

            Directory.SetCurrentDirectory(savedir);
        }

        /// <summary>
        /// Patch with 
        /// </summary>
        /// <returns></returns>
        private async Task PatchForceClassicAsync(string args)
        {
            File.WriteAllText("input.txt", "a\n");
            await _forceClassic.UseAsync(args + " <input.txt");
            File.Delete("input.txt");
        }

        /// <summary>
        /// Patch video with wii-vmc.
        /// </summary>
        /// <returns></returns>
        private async Task PatchVideoAsync(WiiConfig config, string copyWiiVmcToPath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(copyWiiVmcToPath);
            File.WriteAllLines("input.txt", new string[] { "a", config.ToPal ? "1" : "2" });
            var tool = _wiiVmcTool.Copy(copyWiiVmcToPath);
            await tool.UseAsync("main.dol <input.txt");

            tool.Delete();
            File.Delete("input.txt");
            Directory.SetCurrentDirectory(currentDirectory);
        }
        
        /// <summary>
        /// Convert a string to hex
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ToHex(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input) sb.AppendFormat("{0:X2}", (int)c);
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Overwrite bytes for region free settings.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="firstBytes"></param>
        /// <param name="secondBytes"></param>
        private static void WriteRegionFriiBytes(string path, byte[] firstBytes, byte[] secondBytes)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Seek(0x4E003, SeekOrigin.Begin);
                fs.Write(firstBytes, 0, firstBytes.Length);
                fs.Seek(0x4E010, SeekOrigin.Begin);
                fs.Write(secondBytes, 0, secondBytes.Length);
                fs.Close();
            }
        }
    }
}
