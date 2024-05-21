using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml;
using WiiUInjector;
using UWUVCI_AIO_WPF.Properties;
using UWUVCI_AIO_WPF.ViewModels;
using UWUVCI_AIO_WPF.UI.Windows;

namespace UWUVCI_AIO_WPF
{
    public static class StringExtensions
    {
        public static string ToHex(this string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
                sb.AppendFormat("{0:X2}", (int)c);
            return sb.ToString().Trim();
        }
    }
    internal static class Injection
    {
        private static readonly string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "temp");
        private static readonly string baseRomPath = Path.Combine(tempPath, "baserom");
        private static readonly string toolsPath = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Tools");
        static MainViewModel mvvm;

        /*
         * GameConsole: Can either be NDS, N64, GBA, NES, SNES or TG16
         * baseRom = Name of the BaseRom, which is the folder name too (example: Super Metroid EU will be saved at the BaseRom path under the folder SMetroidEU, so the BaseRom is in this case SMetroidEU).
         * customBasePath = Path to the custom Base. Is null if no custom base is used.
         * injectRomPath = Path to the Rom to be injected into the Base Game.
         * bootImages = String array containing the paths for
         *              bootTvTex: PNG or TGA (PNG gets converted to TGA using UPNG). Needs to be in the dimensions 1280x720 and have a bit depth of 24. If null, the original BootImage will be used.
         *              bootDrcTex: PNG or TGA (PNG gets converted to TGA using UPNG). Needs to be in the dimensions 854x480 and have a bit depth of 24. If null, the original BootImage will be used.
         *              iconTex: PNG or TGA (PNG gets converted to TGA using UPNG). Needs to be in the dimensions 128x128 and have a bit depth of 32. If null, the original BootImage will be used.
         *              bootLogoTex: PNG or TGA (PNG gets converted to TGA using UPNG). Needs to be in the dimensions 170x42 and have a bit depth of 32. If null, the original BootImage will be used.
         * gameName = The name of the final game to be entered into the .xml files.
         * iniPath = Only used for N64. Path to the INI configuration. If "blank", a blank ini will be used.
         * darkRemoval = Only used for N64. Indicates whether the dark filter should be removed.
         */

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        [STAThread]
        public static bool Inject(GameConfig Configuration, string RomPath, MainViewModel mvm, bool force)
        {
            long freeSpaceInBytes = 0;
            if (!mvm.SaveWorkAround)
            {
                try
                {
                    long gamesize = new FileInfo(RomPath).Length;
                    var drive = new DriveInfo(tempPath);
                    done = true;
                    freeSpaceInBytes = drive.AvailableFreeSpace;
                }
                catch (Exception)
                {
                    mvm.SaveWorkAround = true;
                }
            }
            long neededspace = mvm.GC ? 10000000000 : 15000000000;
            mvvm = mvm;

            Directory.CreateDirectory(tempPath);

            //mvm.Msg = "Copying Base...";
            try
            {
                if (!mvm.SaveWorkAround && (Configuration.Console == GameConsole.WII || Configuration.Console == GameConsole.GCN) && freeSpaceInBytes < neededspace)
                {
                    throw new Exception("12G");
                }

                if (Configuration.BaseRom == null || Configuration.BaseRom.Name == null)
                {
                    throw new Exception("BASE");
                }
                if (Configuration.BaseRom.Name != "Custom")
                {
                    //Normal Base functionality here
                    CopyBase($"{Configuration.BaseRom.Name.Replace(":", "")} [{Configuration.BaseRom.Region}]", null);
                }
                else
                {
                    //Custom Base Functionality here
                    CopyBase($"Custom", Configuration.CBasePath);
                }
                if (!Directory.Exists(Path.Combine(baseRomPath, "code")) || !Directory.Exists(Path.Combine(baseRomPath, "content")) || !Directory.Exists(Path.Combine(baseRomPath, "meta")))
                {
                    throw new Exception("MISSINGF");
                }

                //mvm.Msg = "Injecting ROM...";
                if (mvm.GC)
                {
                    RunSpecificInjection(Configuration, GameConsole.GCN, RomPath, force, mvm);
                }
                else
                {
                    RunSpecificInjection(Configuration, Configuration.Console, RomPath, force, mvm);
                }

                return true;
            }

            catch (Exception e)
            {
                var errorMessage = "Injection Failed due to unknown circumstances, please contact us on the UWUVCI discord";

                if (e.Message == "MISSINGF")
                    errorMessage = "Injection Failed because there are base files missing. \nPlease redownload the base, or redump if you used a custom base!";
                else if (e.Message.Contains("Images"))
                    errorMessage = "Injection Failed due to wrong BitDepth, please check if your Files are in a different bitdepth than 32bit or 24bit\n\nIf the image/s that's being used is automatically grabbed for you, then don't use them." +
                        "\nFAQ: #28";
                else if (e.Message.Contains("Size"))
                    errorMessage = "Injection Failed due to Image Issues.Please check if your Images are made using following Information:\n\niconTex: \nDimensions: 128x128\nBitDepth: 32\n\nbootDrcTex: \nDimensions: 854x480\nBitDepth: 24\n\nbootTvTex: \nDimensions: 1280x720\nBitDepth: 24\n\nbootLogoTex: \nDimensions: 170x42\nBitDepth: 32";
                else if (e.Message.Contains("retro"))
                    errorMessage = "The ROM you want to Inject is to big for selected Base!\nPlease try again with different Base";
                else if (e.Message.Contains("BASE"))
                    errorMessage = "If you import a config you NEED to reselect a base";
                else if (e.Message.Contains("WII"))
                    errorMessage = $"{e.Message.Replace("WII", "")}\nPlease make sure that your ROM isn't flawed and that you have atleast 12 GB of free Storage left.";
                else if (e.Message.Contains("12G"))
                    errorMessage = $" Please make sure to have atleast {FormatBytes(15000000000)} of storage left on the drive where you stored the Injector.";
                else if (e.Message.Contains("nkit"))
                    errorMessage = $"There is an issue with your NKIT.\nPlease try the original ISO, or redump your game and try again with that dump.";
                else if (e.Message.Contains("meta.xml"))
                    errorMessage = "Looks to be your meta.xml file isn't missing from your directory. If you downloaded your base, redownload it, if it's a custom base then the folder selected might be wrong or the layout is messed up.";
                else if (e.Message.Contains("pre.iso"))
                    errorMessage = "Looks to be that there is something about your game that UWUVCI doesn't like, you are most likely injecting with a wbfs or nkit.iso file, this file has data trimmed." +
                        "\nFAQ: #17, #27, #29";
                else if (e.Message.Contains("temp\\temp") || e.Message.Contains("temp/temp"))
                    errorMessage = "Looks to be your images are the problem" +
                        "\nFAQ: #28";

                MessageBox.Show(errorMessage + "\n\nDon't forget that there's an FAQ in the ReadMe.txt file and on the UWUVCI Discord\n\nError Message:\n" + e.Message, "Injection Failed", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            finally
            {
                mvm.Index = -1;
                mvm.LR = false;
            }
        }

        private static bool done = false;
        private static void Tick(object sender, EventArgs e)
        {
            if (!done)
            {
                mvvm.failed = true;
            }
            throw new Exception("Failed this shit");
        }

        private static void RunSpecificInjection(GameConfig cfg, GameConsole console, string RomPath, bool force, MainViewModel mvm)
        {
            switch (console)
            {
                case GameConsole.WII:
                    if (RomPath.ToLower().EndsWith(".dol"))
                    {
                        WiiHomebrew(RomPath, mvm);
                    }
                    else if (RomPath.ToLower().EndsWith(".wad"))
                    {
                        WiiForwarder(RomPath, mvm);
                    }
                    break;
            }
        }
        private static string ByteArrayToString(byte[] arr)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(arr);
        }
        private static void WiiForwarder(string romPath, MainViewModel mvm)
        {
            string savedir = Directory.GetCurrentDirectory();
            //mvvm.Msg = "Extracting Forwarder Base...";
            if (Directory.Exists(Path.Combine(tempPath, "TempBase"))) Directory.Delete(Path.Combine(tempPath, "TempBase"), true);
            Directory.CreateDirectory(Path.Combine(tempPath, "TempBase"));

            var zipLocation = Path.Combine(toolsPath, "BASE.zip");
            ZipFile.ExtractToDirectory(zipLocation, Path.Combine(tempPath));

            DirectoryCopy(Path.Combine(tempPath, "BASE"), Path.Combine(tempPath, "TempBase"), true);
            //mvvm.Msg = "Setting up Forwarder...";
            byte[] test = new byte[4];
            using (FileStream fs = new FileStream(romPath, FileMode.Open))
            {
                fs.Seek(0xC20, SeekOrigin.Begin);
                fs.Read(test, 0, 4);
                fs.Close();

            }

            string[] id = { ByteArrayToString(test) };
            File.WriteAllLines(Path.Combine(tempPath, "TempBase", "files", "title.txt"), id);
            //mvm.Msg = "Copying Forwarder...";
            File.Copy(Path.Combine(toolsPath, "forwarder.dol"), Path.Combine(tempPath, "TempBase", "sys", "main.dol"));
            //mvvm.Msg = "Creating Injectable file...";
            using (Process wit = new Process())
            {
                if (!mvm.DebugMode)
                {

                    wit.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }
                wit.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                wit.StartInfo.Arguments = $"copy \"{Path.Combine(tempPath, "TempBase")}\" --DEST \"{Path.Combine(tempPath, "game.iso")}\" -ovv --links --iso";
                wit.Start();
                wit.WaitForExit();
            }

            Thread.Sleep(6000);
            if (!File.Exists(Path.Combine(tempPath, "game.iso")))
            {
                Console.Clear();

                throw new Exception("WIIAn error occured while Creating the ISO");
            }
            Directory.Delete(Path.Combine(tempPath, "TempBase"), true);

            //mvm.Msg = "Replacing TIK and TMD...";
            using (Process extract = new Process())
            {
                if (!mvm.DebugMode)
                {
                    extract.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
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
                if (mvm.Index == 2) { extra = "-horizontal "; }
                if (mvm.Index == 3) { extra = "-wiimote "; }
                if (mvm.Index == 4) { extra = "-instantcc "; }
                if (mvm.Index == 5) { extra = "-nocc "; }
                if (mvm.LR) { extra += "-lrpatch "; }
                iso2nfs.StartInfo.Arguments = $"-enc -homebrew {extra}-iso game.iso";
                iso2nfs.Start();
                iso2nfs.WaitForExit();
                File.Delete("nfs2iso2nfs.exe");
                File.Delete("game.iso");
            }
            Directory.SetCurrentDirectory(savedir);
        }

        private static void WiiHomebrew(string romPath, MainViewModel mvm)
        {
            string savedir = Directory.GetCurrentDirectory();
            //mvvm.Msg = "Extracting Homebrew Base...";

            if (Directory.Exists(Path.Combine(tempPath, "TempBase")))
                Directory.Delete(Path.Combine(tempPath, "TempBase"), true);

            Directory.CreateDirectory(Path.Combine(tempPath, "TempBase"));

            ZipFile.ExtractToDirectory(Path.Combine(toolsPath, "BASE.zip"), Path.Combine(tempPath));

            DirectoryCopy(Path.Combine(tempPath, "BASE"), Path.Combine(tempPath, "TempBase"), true);
            //mvvm.Msg = "Injecting DOL...";

            File.Copy(romPath, Path.Combine(tempPath, "TempBase", "sys", "main.dol"));
            //mvvm.Msg = "Creating Injectable file...";
            using (Process wit = new Process())
            {
                if (!mvm.DebugMode)
                {

                    wit.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                }
                wit.StartInfo.FileName = Path.Combine(toolsPath, "wit.exe");
                wit.StartInfo.Arguments = $"copy \"{Path.Combine(tempPath, "TempBase")}\" --DEST \"{Path.Combine(tempPath, "game.iso")}\" -ovv --links --iso";
                wit.Start();
                wit.WaitForExit();
            }

            Thread.Sleep(6000);
            if (!File.Exists(Path.Combine(tempPath, "game.iso")))
            {
                Console.Clear();

                throw new Exception("WIIAn error occured while Creating the ISO");
            }
            Directory.Delete(Path.Combine(tempPath, "TempBase"), true);

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
                string pass = "-passthrough ";
                if (mvm.passtrough != true)
                {
                    pass = "";
                }
                iso2nfs.StartInfo.Arguments = $"-enc -homebrew {pass}-iso game.iso";
                iso2nfs.Start();
                iso2nfs.WaitForExit();
                File.Delete("nfs2iso2nfs.exe");
                File.Delete("game.iso");
            }
            Directory.SetCurrentDirectory(savedir);
        }


        [STAThread]
        public static void Loadiine(string gameName)
        {
            if (gameName == null || gameName == string.Empty) gameName = "NoName";
            gameName = gameName.Replace("|", " ");
            Regex reg = new Regex("[^a-zA-Z0-9 é -]");
            //string outputPath = Path.Combine(Properties.Settings.Default.InjectionPath, gameName);
            string outputPath = Path.Combine(Properties.Settings.Default.OutPath, $"[LOADIINE]{reg.Replace(gameName, "")} [{mvvm.ProdCode}]");
            mvvm.foldername = $"[LOADIINE]{reg.Replace(gameName, "")} [{mvvm.ProdCode}]";
            int i = 0;
            while (Directory.Exists(outputPath))
            {
                outputPath = Path.Combine(Properties.Settings.Default.OutPath, $"[LOADIINE]{reg.Replace(gameName, "")} [{mvvm.ProdCode}]_{i}");
                mvvm.foldername = $"[LOADIINE]{reg.Replace(gameName, "")} [{mvvm.ProdCode}]_{i}";
                i++;
            }

            DirectoryCopy(baseRomPath, outputPath, true);

            Custom_Message cm = new Custom_Message("Injection Complete", $"To Open the Location of the Inject press Open Folder.\nIf you want the inject to be put on your SD now, press Copy to SD.", Settings.Default.OutPath);            
            cm.ShowDialog();
        }

        [STAThread]
        public static void Packing(string gameName, MainViewModel mvm)
        {
            //mvm.Msg = "Checking Tools...";
            //mvm.InjcttoolCheck();            
            //mvm.Msg = "Creating Outputfolder...";
            Regex reg = new Regex("[^a-zA-Z0-9 -]");
            if (gameName == null || gameName == string.Empty) gameName = "NoName";

            //string outputPath = Path.Combine(Properties.Settings.Default.InjectionPath, gameName);
            string outputPath = Path.Combine(Properties.Settings.Default.OutPath, $"[WUP]{reg.Replace(gameName, "").Replace("|", " ")}");
            outputPath = outputPath.Replace("|", " ");
            mvvm.foldername = $"[WUP]{reg.Replace(gameName, "").Replace("|", " ")}";
            int i = 0;
            while (Directory.Exists(outputPath))
            {
                outputPath = Path.Combine(Properties.Settings.Default.OutPath, $"[WUP]{reg.Replace(gameName, "").Replace("|", " ")}_{i}");
                mvvm.foldername = $"[WUP]{reg.Replace(gameName, "").Replace("|", " ")}_{i}";
                i++;
            }
            var oldpath = Directory.GetCurrentDirectory();
            //mvm.Msg = "Packing...";
            try
            {
                Directory.Delete(Environment.GetEnvironmentVariable("LocalAppData") + @"\temp\.net\CNUSPACKER", true);
            }
            catch { }
            using (Process cnuspacker = new Process())
            {
                if (!mvm.DebugMode)
                {
                    cnuspacker.StartInfo.UseShellExecute = false;
                    cnuspacker.StartInfo.CreateNoWindow = true;
                }
                if (Environment.Is64BitOperatingSystem)
                {
                    cnuspacker.StartInfo.FileName = Path.Combine(toolsPath, "CNUSPACKER.exe");
                    cnuspacker.StartInfo.Arguments = $"-in \"{baseRomPath}\" -out \"{outputPath}\" -encryptKeyWith {Properties.Settings.Default.Ckey}";
                }
                else
                {
                    cnuspacker.StartInfo.FileName = "java";
                    cnuspacker.StartInfo.Arguments = $"-jar \"{Path.Combine(toolsPath, "NUSPacker.jar")}\" -in \"{baseRomPath}\" -out \"{outputPath}\" -encryptKeyWith {Properties.Settings.Default.Ckey}";
                }
                cnuspacker.Start();
                cnuspacker.WaitForExit();
                Directory.SetCurrentDirectory(oldpath);
            }
        }

        public static void Download(MainViewModel mvm)
        {
            //mvm.InjcttoolCheck();
            //BaseRomDefinition b = mvm.GetBasefromName(mvm.SelectedBaseAsString);
            BaseRom b = null;

            //GetKeyOfBase
            if (mvm.GameConfiguration.Console == GameConsole.WII || mvm.GameConfiguration.Console == GameConsole.GCN)
            {
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
                Directory.CreateDirectory(tempPath);


                using (Process download = new Process())
                {
                    if (!mvm.DebugMode)
                    {
                        download.StartInfo.UseShellExecute = false;
                        download.StartInfo.CreateNoWindow = true;
                    }

                    download.StartInfo.FileName = Path.Combine(toolsPath, "WiiUDownloader.exe");
                    download.StartInfo.Arguments = $"{b.TitleId} {b.TitleKey} \"{Path.Combine(tempPath, "download")}\"";

                    download.Start();
                    download.WaitForExit();
                }

                using (Process decrypt = new Process())
                {
                    if (!mvm.DebugMode)
                    {
                        decrypt.StartInfo.UseShellExecute = false;
                        decrypt.StartInfo.CreateNoWindow = true;
                    }

                    decrypt.StartInfo.FileName = Path.Combine(toolsPath, "Cdecrypt.exe");
                    decrypt.StartInfo.Arguments = $"{Properties.Settings.Default.Ckey} \"{Path.Combine(tempPath, "download")}\" \"{Path.Combine(Properties.Settings.Default.BasePath, $"{b.Name.Replace(":", "")} [{b.Region}]")}\"";

                    decrypt.Start();
                    decrypt.WaitForExit();
                }
                foreach (string sFile in Directory.GetFiles(Path.Combine(Properties.Settings.Default.BasePath, $"{b.Name.Replace(":", "")} [{b.Region}]", "content"), "*.nfs"))
                    File.Delete(sFile);
            }
            else
            {
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);

                Directory.CreateDirectory(tempPath);
                using (Process download = new Process())
                {
                    if (!mvm.DebugMode)
                    {
                        download.StartInfo.UseShellExecute = false;
                        download.StartInfo.CreateNoWindow = true;
                    }

                    download.StartInfo.FileName = Path.Combine(toolsPath, "WiiUDownloader.exe");
                    download.StartInfo.Arguments = $"{b.TitleId} {b.TitleKey} \"{Path.Combine(tempPath, "download")}\"";
                    download.Start();
                    download.WaitForExit();
                }

                using (Process decrypt = new Process())
                {
                    if (!mvm.DebugMode)
                    {
                        decrypt.StartInfo.UseShellExecute = false;
                        decrypt.StartInfo.CreateNoWindow = true;
                    }
                    decrypt.StartInfo.FileName = Path.Combine(toolsPath, "Cdecrypt.exe");
                    decrypt.StartInfo.Arguments = $"{Properties.Settings.Default.Ckey} \"{Path.Combine(tempPath, "download")}\" \"{Path.Combine(Properties.Settings.Default.BasePath, $"{b.Name.Replace(":", "")} [{b.Region}]")}\"";
                    decrypt.Start();
                    decrypt.WaitForExit();
                }
            }
            //GetCurrentSelectedBase
        }

        public static string ExtractBase(string path, GameConsole console)
        {
            if (!Directory.Exists(Path.Combine(Properties.Settings.Default.BasePath, "CustomBases")))
            {
                Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.BasePath, "CustomBases"));
            }
            string outputPath = Path.Combine(Properties.Settings.Default.BasePath, "CustomBases", $"[{console}] Custom");
            int i = 0;
            while (Directory.Exists(outputPath))
            {
                outputPath = Path.Combine(Properties.Settings.Default.BasePath, $"[{console}] Custom_{i}");
                i++;
            }
            using (Process decrypt = new Process())
            {
                decrypt.StartInfo.UseShellExecute = false;
                decrypt.StartInfo.CreateNoWindow = true;
                decrypt.StartInfo.FileName = Path.Combine(toolsPath, "Cdecrypt.exe");
                decrypt.StartInfo.Arguments = $"{Properties.Settings.Default.Ckey} \"{path}\" \"{outputPath}";
                decrypt.Start();
                decrypt.WaitForExit();
            }
            return outputPath;
        }

        // This function changes TitleID, ProductCode and GameName in app.xml (ID) and meta.xml (ID, ProductCode, Name)
        //private static void EditXML(string gameNameOr, int index)

        //This function copies the custom or normal Base to the working directory
        private static void CopyBase(string baserom, string customPath)
        {
            if (Directory.Exists(baseRomPath)) // sanity check
            {
                Directory.Delete(baseRomPath, true);
            }
            if (baserom == "Custom")
            {
                DirectoryCopy(customPath, baseRomPath, true);
            }
            else
            {
                DirectoryCopy(Path.Combine(Properties.Settings.Default.BasePath, baserom), baseRomPath, true);
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            foreach (FileInfo file in dir.EnumerateFiles())
            {
                file.CopyTo(Path.Combine(destDirName, file.Name), false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dir.EnumerateDirectories())
                {
                    DirectoryCopy(subdir.FullName, Path.Combine(destDirName, subdir.Name), copySubDirs);
                }
            }
        }
    }
}