using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WiiUInjector.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class N64Injector : Injector<N64Config>
    {
        private readonly ITool injectorTool = ToolBox.Tools.N64InjectorTool;

        /// <summary>
        /// Creates a new instance of the <see cref="N64Injector"/> class.
        /// </summary>
        public N64Injector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for an N64 rom.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(N64Config config, BaseRom injectionBase, string directory, bool force)
        {
            string mainRomPath = Directory.GetFiles(Path.Combine(directory, "content", "rom"))[0];
            string mainIniPath = Path.Combine(directory, "content", "config", $"{Path.GetFileName(mainRomPath)}.ini");

            // inject the rom with tool
            await injectorTool.UseAsync($"\"{config.RomPath}\" \"{mainRomPath}\"");

            // apply widescreen and dark filter
            if (config.WideScreen || config.DarkFilter)
            {
                using (var fileStream = File.Open(Path.Combine(directory, "content", "FrameLayout.arc"), FileMode.Open))
                {
                    uint offset = 0;
                    uint size = 0;
                    var offsetB = new byte[4];
                    var sizeB = new byte[4];
                    var nameB = new byte[0x18];
                    var header = new byte[4];

                    byte[] oneOut = BitConverter.GetBytes((float)1);
                    byte[] zeroOut = BitConverter.GetBytes((float)0);

                    byte darkFilter = (byte)(config.DarkFilter ? 0 : 1);
                    byte[] wideScreen = config.WideScreen ? new byte[] { 0x44, 0xF0, 0, 0 } : new byte[] { 0x44, 0xB4, 0, 0 };

                    fileStream.Read(header, 0, 4);
                    if (header[0] == 'S' && header[1] == 'A' && header[2] == 'R' && header[3] == 'C')
                    {
                        fileStream.Position = 0x0C;
                        fileStream.Read(offsetB, 0, 4);

                        offset = (uint)(offsetB[0] << 24 | offsetB[1] << 16 | offsetB[2] << 8 | offsetB[3]);

                        fileStream.Position = 0x38;
                        fileStream.Read(offsetB, 0, 4);
                        offset += (uint)(offsetB[0] << 24 | offsetB[1] << 16 | offsetB[2] << 8 | offsetB[3]);

                        fileStream.Position = offset;
                        fileStream.Read(header, 0, 4);

                        if (header[0] == 'F' && header[1] == 'L' && header[2] == 'Y' && header[3] == 'T')
                        {
                            fileStream.Position = offset + 0x04;
                            fileStream.Read(offsetB, 0, 4);

                            offsetB[0] = 0;
                            offsetB[1] = 0;

                            offset += (uint)(offsetB[0] << 24 | offsetB[1] << 16 | offsetB[2] << 8 | offsetB[3]);

                            fileStream.Position = offset;

                            string name = null;
                            while (name != "power_save_bg")
                            {
                                fileStream.Read(header, 0, 4);
                                fileStream.Read(sizeB, 0, 4);
                                size = (uint)(sizeB[0] << 24 | sizeB[1] << 16 | sizeB[2] << 8 | sizeB[3]);

                                if (header[0] == 'p' && header[1] == 'i' && header[2] == 'c' && header[3] == '1')
                                {
                                    fileStream.Position = offset + 0x0C;
                                    fileStream.Read(nameB, 0, 0x18);
                                    int count = Array.IndexOf(nameB, (byte)0);
                                    name = Encoding.ASCII.GetString(nameB, 0, count);

                                    if (name == "frame")
                                    {
                                        fileStream.Position = offset + 0x2C;
                                        fileStream.WriteByte(zeroOut[3]);
                                        fileStream.WriteByte(zeroOut[2]);
                                        fileStream.WriteByte(zeroOut[1]);
                                        fileStream.WriteByte(zeroOut[0]);

                                        fileStream.Position = offset + 0x30;//TranslationX
                                        fileStream.WriteByte(zeroOut[3]);
                                        fileStream.WriteByte(zeroOut[2]);
                                        fileStream.WriteByte(zeroOut[1]);
                                        fileStream.WriteByte(zeroOut[0]);

                                        fileStream.Position = offset + 0x44;//ScaleX
                                        fileStream.WriteByte(oneOut[3]);
                                        fileStream.WriteByte(oneOut[2]);
                                        fileStream.WriteByte(oneOut[1]);
                                        fileStream.WriteByte(oneOut[0]);

                                        fileStream.Position = offset + 0x48;//ScaleY
                                        fileStream.WriteByte(oneOut[3]);
                                        fileStream.WriteByte(oneOut[2]);
                                        fileStream.WriteByte(oneOut[1]);
                                        fileStream.WriteByte(oneOut[0]);

                                        fileStream.Position = offset + 0x4C;//Widescreen
                                        fileStream.Write(wideScreen, 0, 4);
                                    }
                                    else if (name == "frame_mask")
                                    {
                                        fileStream.Position = offset + 0x08;//Dark filter
                                        fileStream.WriteByte(darkFilter);
                                    }

                                    offset += size;
                                    fileStream.Position = offset;
                                }
                                else if (offset + size < fileStream.Length)
                                {
                                    offset += size;
                                    fileStream.Position = offset;
                                }
                            }
                        }
                    }
                    fileStream.Close();
                }
            }

            // ini
            if (File.Exists(mainIniPath)) File.Delete(mainIniPath);
            if (config.IniPath is null)
            {
                CreateIniFromConfig(config, injectionBase.Region, mainIniPath);
            }
            else
            {
                File.Copy(config.IniPath, mainIniPath);
            }
        }

        /// <summary>
        /// Creates an n64 ini file from a config.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="region"></param>
        /// <param name="iniPath"></param>
        private static void CreateIniFromConfig(N64Config config, Region region, string iniPath)
        {
            char newLine = '\n';
            string iniText = ";" + config.Name + " " + region.ToString() + newLine + "[RomOption]" + newLine;
            iniText += "BackupType = " + config.BackupType + newLine;
            iniText += "BackupSize = " + config.BackupSize.ToString().Replace("_", string.Empty) + newLine;
            if (config.UseTimer) iniText += "UseTimer = 1" + newLine;
            if (config.Rumble && !config.MemPak) iniText += "Rumble = 1" + newLine;
            if (config.RetraceByVsync) iniText += "RetraceByVsync = 1" + newLine;
            if (config.ExpansionPak) iniText += "RamSize = 0x800000" + newLine;
            if (config.TrueBoot) iniText += "TrueBoot = 1" + newLine;
            if (config.MemPak) iniText += "MemPack = 1" + newLine;

            File.WriteAllText(iniPath, iniText);
        }
    }
}
