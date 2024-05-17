using System;
using WiiUInjector;

namespace UWUVCI_AIO_WPF
{
    [Serializable]
    public class GameConfig : ICloneable
    {
        public GameConfig Clone()
        {
            return this.MemberwiseClone() as GameConfig;
        }
        object ICloneable.Clone() => this.Clone();

        public GameConsole Console { get; set; }
        public BaseRom BaseRom { get; set; }
        public string CBasePath { get; set; }

        public byte[] bootsound;
        public string extension = "";
        public bool fourbythree = false;
        public bool disgamepad = false;
        public bool donttrim = false;
        public bool lr = false;
        public bool motepass = false;
        public bool jppatch = false;
        public bool pokepatch = false;

        public bool tgcd = false;

        public int Index;

        public bool pixelperfect = false;
        public string GameName { get; set; }

        public bool vm = false;
        public bool vmtopal = false;

        public bool rf = false;
        public bool rfus = false;
        public bool rfjp = false;
    }
}
