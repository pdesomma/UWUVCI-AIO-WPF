namespace WiiUInjector.Configs
{
    public sealed class GbaConfig : Config
    {
        public bool PokePatch { get; set; }

        public GbaConfig() : base(GameConsole.GBA) { }
    }
}
