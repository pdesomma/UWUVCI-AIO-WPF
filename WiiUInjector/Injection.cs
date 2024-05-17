
using System.IO;

namespace WiiUInjector
{
    /// <summary>
    /// Represents a game to be injected into a WiiU.
    /// </summary>
    public class Injection
    {
        public bool Exists => Directory.Exists(Path);
        public string Name { get; set; }
        public string Path { get; set; }
        public string ProdCode { get; set; }
    }
}
