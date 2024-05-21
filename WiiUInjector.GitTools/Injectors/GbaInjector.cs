using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WiiUInjector.GitTools.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class GbaInjector : Injector<GbaConfig>
    {
        private readonly ITool _goombaTool = ToolBox.Tools.GoombaTool;
        private readonly ITool _psbTool = ToolBox.Tools.PsbTool;
        private string _actualRomPath;
        private string _tempDirectory;

        /// <summary>
        /// Creates a new instance of the <see cref="GbaInjector"/> class.
        /// </summary>
        public GbaInjector(string workingDirectory) : base(workingDirectory) { _ = ToolBox.AddAsync(_goombaTool.Name); }

        /// <summary>
        /// Do extra work before we get started if it's a gameboy rom.
        /// </summary>
        /// <param name="directory"></param>
        protected override void Preprocess(GbaConfig config, BaseRom injectionBase) { }

        /// <summary>
        /// Do extra work after we get started if it's a gameboy rom.
        /// </summary>
        /// <param name="directory"></param>
        protected override void Postprocess(GbaConfig config, BaseRom injectionBase)
        {
            if (_actualRomPath != null && File.Exists(_actualRomPath) && config.RomPath != _actualRomPath) File.Delete(_actualRomPath);
            if (Directory.Exists(_tempDirectory)) Directory.Delete(_tempDirectory, true);
        }

        /// <summary>
        /// Does injection work for a gameboy rom.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(GbaConfig config, BaseRom injectionBase, string directory, bool force)
        {
            _actualRomPath = config.RomPath;
            _tempDirectory = Path.Combine(directory, "temp");
            string goombaPath = ToolBox.ToolsDirectory + _goombaTool.Name;
            string tempGoombaPath = Path.Combine(_tempDirectory, _goombaTool.Name);
            string paddedGoombaPath = Path.Combine(_tempDirectory, "padded_" + _goombaTool.Name);
            string tempRomPath = Path.Combine(_tempDirectory, "temp_rom.gba");

            if (!Path.GetExtension(_actualRomPath).Contains("gba"))
            {
                if (!Directory.Exists(_tempDirectory)) Directory.CreateDirectory(_tempDirectory);
                using (Process goomba = new Process())
                {
                    goomba.StartInfo.UseShellExecute = false;
                    goomba.StartInfo.CreateNoWindow = true;
                    goomba.StartInfo.FileName = "cmd.exe";
                    goomba.StartInfo.Arguments = $"/c copy /b \"{goombaPath}\"+\"{config.RomPath}\" \"{tempGoombaPath}\"";
                    goomba.Start();
                    goomba.WaitForExit();
                }

                // pad it
                byte[] rom = new byte[33554432];
                using (var fileStream = new FileStream(tempGoombaPath, FileMode.Open))
                {
                    fileStream.Read(rom, 0, (int)fileStream.Length);
                    fileStream.Close();
                }
                File.WriteAllBytes(paddedGoombaPath, rom);
                _actualRomPath = paddedGoombaPath;
            }

            if (config.PokePatch)
            {
                // copy the rom to a temp spot and apply poke patch
                File.Copy(_actualRomPath, tempRomPath, true);
                _actualRomPath = tempRomPath;
                ApplyPokePatch(_actualRomPath);
            }

            var res = await _psbTool.UseAsync($"\"{Path.Combine(injectionBase.Path, "content", "alldata.psb.m")}\" \"{_actualRomPath}\" \"{Path.Combine(directory, "content", "alldata.psb.m")}\"");

            if (File.Exists(paddedGoombaPath) && _actualRomPath != paddedGoombaPath) File.Delete(paddedGoombaPath);
            if (File.Exists(tempRomPath) && _actualRomPath != tempRomPath) File.Delete(tempRomPath);
        }

        /// <summary>
        /// Applies poke patch to a rom.
        /// </summary>
        /// <param name="romPath"></param>
        private static void ApplyPokePatch(string romPath)
        {
            byte[] pattern = { 0xD0, 0x88, 0x8D, 0x83, 0x42 };
            byte[] test = new byte[new FileInfo(romPath).Length];

            using (var romFileStream = new FileStream(romPath, FileMode.Open, FileAccess.ReadWrite))
            {
                romFileStream.Read(test, 0, test.Length - 1);
                var indicesList = FindMatchingIndices(test, pattern, 0);
                byte[] check = new byte[4];

                if(indicesList.Count >= 2)
                {
                    romFileStream.Seek(indicesList[0] + 5, SeekOrigin.Begin);
                    romFileStream.Read(check, 0, 4);

                    romFileStream.Seek(0, SeekOrigin.Begin);
                    if (check[3] != 0x24)
                    {
                        romFileStream.Seek(indicesList[0] + 5, SeekOrigin.Begin);
                        romFileStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4);
                    }
                    else
                    {
                        romFileStream.Seek(indicesList[0] + 5, SeekOrigin.Begin);
                        romFileStream.Write(new byte[] { 0x00, 0x00, 0x00 }, 0, 3);
                    }
                    check = new byte[4];
                    romFileStream.Seek(indicesList[1] + 5, SeekOrigin.Begin);
                    romFileStream.Read(check, 0, 4);
                    romFileStream.Seek(0, SeekOrigin.Begin);
                    if (check[3] != 0x24)
                    {
                        romFileStream.Seek(indicesList[1] + 5, SeekOrigin.Begin);
                        romFileStream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4);
                    }
                    else
                    {
                        romFileStream.Seek(indicesList[1] + 5, SeekOrigin.Begin);
                        romFileStream.Write(new byte[] { 0x00, 0x00, 0x00 }, 0, 3);
                    }
                }
                romFileStream.Close();
            }
        }

        /// <summary>
        /// Returns a list of positions where a byte pattern occurs in a buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="pattern"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static List<int> FindMatchingIndices(byte[] buffer, byte[] pattern, int startIndex)
        {
            List<int> positions = new List<int>();
            int i = Array.IndexOf<byte>(buffer, pattern[0], startIndex);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual<byte>(pattern))
                    positions.Add(i);
                i = Array.IndexOf<byte>(buffer, pattern[0], i + 1);
            }
            return positions;
        }
    }
}
