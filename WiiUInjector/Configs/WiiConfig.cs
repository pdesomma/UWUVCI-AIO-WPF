namespace WiiUInjector.Configs
{
    public sealed class WiiConfig : Config
    {
        public WiiConfig() : base(GameConsole.WII) { }

        public bool NKit { get; set; }
    }
}
