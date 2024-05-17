using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WiiUInjector.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class GcInjector : Injector<GcConfig>
    {
        private static readonly string s_tempPath = Path.Combine(Directory.GetCurrentDirectory(), "gcTemp");
        private readonly ITool _baseTool;
        private readonly ITool _isoConvertTool;
        private readonly ITool _nfsTool;
        private readonly ITool _nkitTool;
        private readonly ITool _nintendontTool;
        private readonly ITool _nintendontForceTool;
        private readonly ITool _witTool;

        /// <summary>
        /// Creates a new instance of the <see cref="GcInjector"/> class.
        /// </summary>
        public GcInjector(string workingDirectory) : base(workingDirectory)
        {
            _nintendontTool = ToolBox.Tools.NintendontTool;
            _nintendontForceTool = ToolBox.Tools.NintendontForceTool;
            _witTool = ToolBox.Tools.WitTool;
            _isoConvertTool = ToolBox.Tools.ConvertIsoTool;
            _nkitTool = ToolBox.Tools.ConvertNKitTool;
            _baseTool = ToolBox.Tools.GcBaseTool;
            _nfsTool = ToolBox.Tools.Nfs2IsoTool;
        }

        /// <summary>
        /// Does injection work for Gamecube roms.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(GcConfig config, BaseRom injectionBase, string directory, bool force)
        {
            // download needed resources.
            await Task.WhenAll(
                ToolBox.AddAsync(_nfsTool.Name),
                ToolBox.AddAsync(_baseTool.Name),
                ToolBox.AddAsync(_nintendontTool.Name), 
                ToolBox.AddAsync(_nintendontTool.Name));

            if (Directory.Exists(Path.Combine(s_tempPath, "TempBase"))) Directory.Delete(Path.Combine(s_tempPath, "TempBase"), true);
            Directory.CreateDirectory(Path.Combine(s_tempPath, "TempBase"));
            ZipFile.ExtractToDirectory(Path.Combine(ToolBox.ToolsDirectory, _baseTool.Name), s_tempPath);
            DirectoryCopy(Path.Combine(s_tempPath, "BASE"), Path.Combine(s_tempPath, "TempBase"));

            // dol replacement with nintendont
            File.Copy(Path.Combine(ToolBox.ToolsDirectory, config.FourThree ? "nintendont_force.dol" : "nintendont.dol"), Path.Combine(s_tempPath, "TempBase", "sys", "main.dol"));

            await ProcessRomPaths(config.RomPath, config, "out.iso", "game.iso");
            await ProcessRomPaths(config.RomPath2, config, "out(Disc 1).iso", "disc2.iso");
            await _witTool.UseAsync($"copy \"{Path.Combine(s_tempPath, "TempBase")}\" --DEST \"{Path.Combine(s_tempPath, "game.iso")}\" -ovv --links --iso");

            if (!File.Exists(Path.Combine(s_tempPath, "game.iso")))
            {
                throw new Exception("An error occured while Creating the ISO");
            }

            // GET ROMCODE and change it
            var chars = new byte[4];
            using (var fstrm = new FileStream(Path.Combine(s_tempPath, "TempBase", "files", "game.iso"), FileMode.Open))
            {
                fstrm.Read(chars, 0, 4);
                fstrm.Close();
            }

            // edit meta.xml
            string procod = new ASCIIEncoding().GetString(chars);
            string metaXml = Path.Combine(directory, "meta", "meta.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(metaXml);
            doc.SelectSingleNode("menu/reserved_flag2").InnerText = ToHex(procod);
            doc.Save(metaXml);
            Directory.Delete(Path.Combine(s_tempPath, "TempBase"), true);

            // replace TIK and TMD
            await _witTool.UseAsync($"extract \"{Path.Combine(s_tempPath, "game.iso")}\" --psel data --files +tmd.bin --files +ticket.bin --DEST \"{Path.Combine(s_tempPath, "TIKTMD")}\" -vv1");
            foreach (string sFile in Directory.GetFiles(Path.Combine(directory, "code"), "rvlt.*"))
            {
                File.Delete(sFile);
            }
            File.Copy(Path.Combine(s_tempPath, "TIKTMD", "tmd.bin"), Path.Combine(directory, "code", "rvlt.tmd"));
            File.Copy(Path.Combine(s_tempPath, "TIKTMD", "ticket.bin"), Path.Combine(directory, "code", "rvlt.tik"));
            Directory.Delete(Path.Combine(s_tempPath, "TIKTMD"), true);

            // inject rom
            foreach (string sFile in Directory.GetFiles(Path.Combine(directory, "content"), "*.nfs"))
            {
                File.Delete(sFile);
            }
            File.Move(Path.Combine(s_tempPath, "game.iso"), Path.Combine(directory, "content", "game.iso"));

            _nfsTool.Move(Path.Combine(directory, "content"));
            await _nfsTool.UseAsync($"-enc -homebrew -passthrough -iso game.iso");
            _nfsTool.Move(ToolBox.ToolsDirectory);
        }

        /// <summary>
        /// do some work on the rom files we want to inject.
        /// </summary>
        /// <param name="romPath"></param>
        /// <param name="gcConfig"></param>
        /// <param name="outName"></param>
        /// <param name="isoName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task ProcessRomPaths(string romPath, GcConfig gcConfig, string outName, string isoName)
        {
            if (gcConfig.DisableTrim)
            {
                // if nkit or gcz, run iso tool and clean up
                if (romPath.ToLower().Contains("nkit.iso") || romPath.ToLower().Contains("gcz"))
                {
                    await _isoConvertTool.UseAsync($"\"{romPath}\"");
                    if (!File.Exists(Path.Combine(ToolBox.ToolsDirectory, "out.iso"))) throw new Exception("iso");
                    File.Move(Path.Combine(ToolBox.ToolsDirectory, "out.iso"), Path.Combine(s_tempPath, "TempBase", "files", isoName));
                }
                else
                {
                    File.Copy(romPath, Path.Combine(s_tempPath, "TempBase", "files", isoName));
                }
            }
            else if (gcConfig.RomPath.ToLower().Contains("iso") || gcConfig.RomPath.ToLower().Contains("gcm") || gcConfig.RomPath.ToLower().Contains("gcz"))
            {
                //convert to nkit
                await _nkitTool.UseAsync($"\"{romPath}\"");
                if (!File.Exists(Path.Combine(ToolBox.ToolsDirectory, outName))) throw new Exception("nkit");
                File.Move(Path.Combine(ToolBox.ToolsDirectory, outName), Path.Combine(s_tempPath, "TempBase", "files", isoName));
            }
            else
            {
                File.Copy(romPath, Path.Combine(s_tempPath, "TempBase", "files", isoName));
            }
        }

        /// <summary>
        /// Convert string to hex
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ToHex(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
                sb.AppendFormat("{0:X2}", (int)c);
            return sb.ToString().Trim();
        }
    }
}
