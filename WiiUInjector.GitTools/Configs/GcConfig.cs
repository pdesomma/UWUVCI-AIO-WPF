namespace WiiUInjector.GitTools.Configs
{
    public sealed class GcConfig : Config
    {
        public GcConfig() : base(GameConsole.GCN) { }

        public bool DisableGamepad { get; set; }
        public bool DisableTrim { get; set; }
        public bool FourThree { get; set; }
        public string RomPath2 { get; set; }
    }
}
