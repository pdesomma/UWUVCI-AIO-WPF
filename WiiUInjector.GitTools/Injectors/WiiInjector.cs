using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WiiUInjector.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class WiiInjector : Injector<WiiConfig>
    {
        private readonly ITool _isoTool = ToolBox.Tools.WitTool;

        /// <summary>
        /// Creates a new instance of the <see cref="WiiInjector"/> class.
        /// </summary>
        public WiiInjector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for Gamecube roms.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(WiiConfig config, BaseRom injectionBase, string directory, bool force)
        {
            switch (Path.GetExtension(config.RomPath.ToLower())
            {
                case "dol":
                    await RunHomebrewAsync(config);
                    break;
                case "wad":
                    await RunForwardAsync(config);
                    break;
                default:
                    await RunWiiAsync(config);
                    break;
            }
        }

        /// <summary>
        /// Do normal wii stuff.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task RunWiiAsync(WiiConfig config)
        {
            string savedir = Directory.GetCurrentDirectory();

            // get an ISO to use.
            if (config.NKit)
            {
                await _isoTool.UseAsync($"copy --source \"{config.RomPath}\" --dest \"{Path.Combine(tempPath, "pre.iso")}\" -I");
                if (!File.Exists(Path.Combine(ToolBox.ToolsDirectory, "out.iso"))) throw new Exception("nkit");               
                File.Move(Path.Combine(ToolBox.ToolsDirectory, "out.iso"), Path.Combine(ToolBox.ToolsDirectory, "pre.iso"));
            }
            else
            {
                if (new FileInfo(config.RomPath).Extension.Contains("wbfs"))
                {
                    await _isoTool.UseAsync($"copy --source \"{config.RomPath}\" --dest \"{Path.Combine(tempPath, "pre.iso")}\" -I");
                }
                else if (new FileInfo(config.RomPath).Extension.Contains("iso"))
                {
                    File.Copy(config.RomPath, Path.Combine(tempPath, "pre.iso"));
                }
            }
            //GET ROMCODE and change it
            //mvm.Msg = "Trying to change the Manual...";
            //READ FIRST 4 BYTES
            byte[] chars = new byte[4];
            FileStream fstrm = new FileStream(Path.Combine(tempPath, "pre.iso"), FileMode.Open);
            fstrm.Read(chars, 0, 4);
            fstrm.Close();
            string procod = ByteArrayToString(chars);
            string neededformanual = procod.ToHex();
            string metaXml = Path.Combine(baseRomPath, "meta", "meta.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(metaXml);
            doc.SelectSingleNode("menu/reserved_flag2").InnerText = neededformanual;
            doc.Save(metaXml);
            //edit emta.xml

            if (!mvm.donttrim)
            {
                if (mvm.regionfrii)
                {
                    if (mvm.regionfriius)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(tempPath, "pre.iso"), FileMode.Open))
                        {
                            fs.Seek(0x4E003, SeekOrigin.Begin);
                            fs.Write(new byte[] { 0x01 }, 0, 1);
                            fs.Seek(0x4E010, SeekOrigin.Begin);
                            fs.Write(new byte[] { 0x80, 0x06, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }, 0, 16);
                            fs.Close();

                        }
                    }
                    else if (mvm.regionfriijp)
                    {
                        using (FileStream fs = new FileStream(Path.Combine(tempPath, "pre.iso"), FileMode.Open))
                        {
                            fs.Seek(0x4E003, SeekOrigin.Begin);
                            fs.Write(new byte[] { 0x00 }, 0, 1);
                            fs.Seek(0x4E010, SeekOrigin.Begin);
                            fs.Write(new byte[] { 0x00, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }, 0, 16);
                            fs.Close();

                        }
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(Path.Combine(tempPath, "pre.iso"), FileMode.Open))
                        {
                            fs.Seek(0x4E003, SeekOrigin.Begin);
                            fs.Write(new byte[] { 0x02 }, 0, 1);
                            fs.Seek(0x4E010, SeekOrigin.Begin);
                            fs.Write(new byte[] { 0x80, 0x80, 0x80, 0x00, 0x03, 0x03, 0x04, 0x03, 0x00, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80 }, 0, 16);
                            fs.Close();

                        }
                    }
                }
                using (Process trimm = new Process())
                {
                    //mvm.Msg = "Trimming ROM...";
                    trimm.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                    trimm.StartInfo.Arguments = $"extract \"{Path.Combine(tempPath, "pre.iso")}\" --DEST \"{Path.Combine(tempPath, "TEMP")}\" --psel data -vv1";
                    trimm.Start();
                    trimm.WaitForExit();
                }
                if (mvm.Index == 4)
                {
                    //mvvm.Msg = "Patching ROM (Force CC)...";
                    Console.WriteLine("Patching the ROM to force Classic Controller input");
                    using (Process tik = new Process())
                    {
                        tik.StartInfo.FileName = Path.Combine(toolsPath, "GetExtTypePatcher.exe");
                        tik.StartInfo.Arguments = $"\"{Path.Combine(tempPath, "TEMP", "sys", "main.dol")}\" -nc";
                        tik.StartInfo.UseShellExecute = false;
                        tik.StartInfo.CreateNoWindow = true;
                        tik.StartInfo.RedirectStandardOutput = true;
                        tik.StartInfo.RedirectStandardInput = true;
                        tik.Start();
                        Thread.Sleep(2000);
                        tik.StandardInput.WriteLine();
                        tik.WaitForExit();
                    }

                }
                if (mvm.jppatch)
                {

                    //mvm.Msg = "Language Patching ROM...";
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
                if (mvm.Patch)
                {
                    //mvm.Msg = "Video Patching ROM...";
                    using (Process vmc = new Process())
                    {
                        File.Copy(Path.Combine(toolsPath, "wii-vmc.exe"), Path.Combine(tempPath, "TEMP", "sys", "wii-vmc.exe"));

                        Directory.SetCurrentDirectory(Path.Combine(tempPath, "TEMP", "sys"));
                        vmc.StartInfo.FileName = "wii-vmc.exe";
                        vmc.StartInfo.Arguments = "main.dol";
                        vmc.StartInfo.UseShellExecute = false;
                        vmc.StartInfo.CreateNoWindow = true;
                        vmc.StartInfo.RedirectStandardOutput = true;
                        vmc.StartInfo.RedirectStandardInput = true;

                        vmc.Start();
                        Thread.Sleep(1000);
                        vmc.StandardInput.WriteLine("a");
                        Thread.Sleep(2000);
                        if (mvm.toPal) vmc.StandardInput.WriteLine("1");
                        else vmc.StandardInput.WriteLine("2");
                        Thread.Sleep(2000);
                        vmc.StandardInput.WriteLine();
                        vmc.WaitForExit();
                        File.Delete("wii-vmc.exe");


                        Directory.SetCurrentDirectory(savedir);
                    }

                }
                //mvm.Msg = "Creating ISO from trimmed ROM...";
                using (Process repack = new Process())
                {
                    repack.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                    repack.StartInfo.Arguments = $"copy \"{Path.Combine(tempPath, "TEMP")}\" --DEST \"{Path.Combine(tempPath, "game.iso")}\" -ovv --links --iso";
                    repack.Start();
                    repack.WaitForExit();
                    Directory.Delete(Path.Combine(tempPath, "TEMP"), true);
                    File.Delete(Path.Combine(tempPath, "pre.iso"));
                }
            }
            else
            {
                if (mvm.Index == 4 || mvm.Patch)
                {
                    using (Process trimm = new Process())
                    {
                        if (!mvm.DebugMode)
                        {

                            trimm.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        }
                        //mvm.Msg = "Trimming ROM...";
                        trimm.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                        trimm.StartInfo.Arguments = $"extract \"{Path.Combine(tempPath, "pre.iso")}\" --DEST \"{Path.Combine(tempPath, "TEMP")}\" --psel WHOLE -vv1";
                        trimm.Start();
                        trimm.WaitForExit();
                    }
                    if (mvm.Index == 4)
                    {
                        //mvvm.Msg = "Patching ROM (Force CC)...";
                        Console.WriteLine("Patching the ROM to force Classic Controller input");
                        using (Process tik = new Process())
                        {
                            tik.StartInfo.FileName = Path.Combine(toolsPath, "GetExtTypePatcher.exe");
                            tik.StartInfo.Arguments = $"\"{Path.Combine(tempPath, "TEMP", "DATA", "sys", "main.dol")}\" -nc";
                            tik.StartInfo.UseShellExecute = false;
                            tik.StartInfo.CreateNoWindow = true;
                            tik.StartInfo.RedirectStandardOutput = true;
                            tik.StartInfo.RedirectStandardInput = true;
                            tik.Start();
                            Thread.Sleep(2000);
                            tik.StandardInput.WriteLine();
                            tik.WaitForExit();
                        }

                    }
                    if (mvm.Patch)
                    {
                        //mvm.Msg = "Video Patching ROM...";
                        using (Process vmc = new Process())
                        {

                            File.Copy(Path.Combine(toolsPath, "wii-vmc.exe"), Path.Combine(tempPath, "TEMP", "DATA", "sys", "wii-vmc.exe"));

                            Directory.SetCurrentDirectory(Path.Combine(tempPath, "TEMP", "DATA", "sys"));
                            vmc.StartInfo.FileName = "wii-vmc.exe";
                            vmc.StartInfo.Arguments = "main.dol";
                            vmc.StartInfo.UseShellExecute = false;
                            vmc.StartInfo.CreateNoWindow = true;
                            vmc.StartInfo.RedirectStandardOutput = true;
                            vmc.StartInfo.RedirectStandardInput = true;

                            vmc.Start();
                            Thread.Sleep(1000);
                            vmc.StandardInput.WriteLine("a");
                            Thread.Sleep(2000);
                            if (mvm.toPal) vmc.StandardInput.WriteLine("1");
                            else vmc.StandardInput.WriteLine("2");
                            Thread.Sleep(2000);
                            vmc.StandardInput.WriteLine();
                            vmc.WaitForExit();
                            File.Delete("wii-vmc.exe");

                            Directory.SetCurrentDirectory(savedir);
                        }

                    }
                    //mvm.Msg = "Creating ISO from patched ROM...";
                    using (Process repack = new Process())
                    {
                        if (!mvm.DebugMode)
                        {

                            repack.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        }
                        repack.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                        repack.StartInfo.Arguments = $"copy \"{Path.Combine(tempPath, "TEMP")}\" --DEST \"{Path.Combine(tempPath, "game.iso")}\" -ovv --psel WHOLE --iso";
                        repack.Start();
                        repack.WaitForExit();
                        Directory.Delete(Path.Combine(tempPath, "TEMP"), true);
                        File.Delete(Path.Combine(tempPath, "pre.iso"));
                    }
                }
                else
                {
                    File.Move(Path.Combine(tempPath, "pre.iso"), Path.Combine(tempPath, "game.iso"));
                }

            }

            //mvm.Msg = "Replacing TIK and TMD...";
            using (Process extract = new Process())
            {
                if (!mvm.DebugMode)
                {

                    extract.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                }
                extract.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                extract.StartInfo.Arguments = $"extract \"{Path.Combine(tempPath, "game.iso")}\" --psel data --files +tmd.bin --files +ticket.bin --DEST \"{Path.Combine(tempPath, "TIKTMD")}\" -vv1";
                extract.Start();
                extract.WaitForExit();
                foreach (string sFile in Directory.GetFiles(Path.Combine(baseRomPath, "code"), "rvlt.*"))
                {
                    File.Delete(sFile);
                }
                File.Copy(Path.Combine(tempPath, "TIKTMD", "tmd.bin"), Path.Combine(baseRomPath, "code", "rvlt.tmd"));
                File.Copy(Path.Combine(tempPath, "TIKTMD", "ticket.bin"), Path.Combine(baseRomPath, "code", "rvlt.tik"));
                Directory.Delete(Path.Combine(tempPath, "TIKTMD"), true);
            }

            //mvm.Msg = "Injecting ROM...";
            foreach (string sFile in Directory.GetFiles(Path.Combine(baseRomPath, "content"), "*.nfs"))
            {
                File.Delete(sFile);
            }
            File.Move(Path.Combine(tempPath, "game.iso"), Path.Combine(baseRomPath, "content", "game.iso"));
            File.Copy(Path.Combine(toolsPath, "nfs2iso2nfs.exe"), Path.Combine(baseRomPath, "content", "nfs2iso2nfs.exe"));
            Directory.SetCurrentDirectory(Path.Combine(baseRomPath, "content"));
            using (Process iso2nfs = new Process())
            {
                if (!mvm.DebugMode)
                {

                    iso2nfs.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                }
                iso2nfs.StartInfo.FileName = "nfs2iso2nfs.exe";
                string extra = "";
                if (mvm.Index == 2)
                {
                    extra = "-horizontal ";
                }
                if (mvm.Index == 3) { extra = "-wiimote "; }
                if (mvm.Index == 4) { extra = "-instantcc "; }
                if (mvm.Index == 5) { extra = "-nocc "; }
                if (mvm.LR) { extra += "-lrpatch "; }
                iso2nfs.StartInfo.Arguments = $"-enc {extra}-iso game.iso";
                iso2nfs.Start();
                iso2nfs.WaitForExit();
                File.Delete("nfs2iso2nfs.exe");
                File.Delete("game.iso");
            }
            Directory.SetCurrentDirectory(savedir);
        }
    }
}
