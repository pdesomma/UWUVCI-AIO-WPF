using System.IO;
using System.Threading.Tasks;
using WiiUInjector.GitTools.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class MsxInjector : Injector<MsxConfig>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MsxInjector"/> class.
        /// </summary>
        /// <param name="config"></param>
        public MsxInjector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for Msx roms.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(MsxConfig config, BaseRom injectionBase, string directory, bool force)
        {
            await Task.Run(() =>
            {
                // read header from copied base.
                byte[] header = new byte[0x580B3];
                using (var originalPkg = new FileStream(Path.Combine(directory, "content", "msx", "msx.pkg"), FileMode.Open, FileAccess.ReadWrite))
                {
                    originalPkg.Read(header, 0, header.Length);
                    originalPkg.Close();
                }
                File.Delete(Path.Combine(directory, "content", "msx", "msx.pkg"));

                // write header to a new PKG
                using (var newPkg = new FileStream(Path.Combine(directory, "content", "msx", "msx.pkg"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    newPkg.Write(header, 0, header.Length);
                    newPkg.Close();
                }

                // write rom bytes to new pkg
                byte[] romBytes = File.ReadAllBytes(config.RomPath);
                using (var finalPkg = new FileStream(Path.Combine(directory, "content", "msx", "msx.pkg"), FileMode.Append))
                {
                    finalPkg.Write(romBytes, 0, romBytes.Length);
                    finalPkg.Close();
                }
            });
        }
    }
}
