using System.IO;

namespace WiiUInjector
{
    public class BaseRom
    {
        public GameConsole Console { get; set; }
        public bool IsDownloaded => Directory.Exists(Path);
        public bool IsKeyValid => !(TitleKey is null) && KeyHash == TitleKey.ToLower().GetHashCode();
        public int KeyHash { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Region Region { get; set; }
        public string TitleId { get; set; }
        public string TitleKey { get; set; }
    }
}
