namespace WiiUInjector.GitTools.Configs
{
    public sealed class WiiConfig : Config
    {
        public WiiConfig() : base(GameConsole.WII) { }

        public bool NKit { get; set; }
        public bool DontTrim { get; set; }
        public bool PatchLanguage { get; set; }
        public bool PatchVideo { get; set; }
        public bool RegionFriiUs { get; set; }
        public bool RegionFriiJp { get; set; }
        public bool RegionFriiEu { get; set; }
        public WiiUseGamePadAs UseGamepadAs { get; set; } = WiiUseGamePadAs.None;
        public bool LR { get; set; }
        public bool ToPal { get; set; }
    }
}
