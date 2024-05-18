using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WiiUInjector.Configs;

namespace WiiUInjector.GitTools
{
    public sealed class WiiInjector : Injector<WiiConfig>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WiiInjector"/> class.
        /// </summary>
        public WiiInjector(string workingDirectory) : base(workingDirectory) { }

        /// <summary>
        /// Does injection work for Gamecube roms.
        /// </summary>
        protected override async Task RunSpecificInjectionAsync(WiiConfig config, BaseRom injectionBase, string directory, bool force)
        { 
        }
    }
}
