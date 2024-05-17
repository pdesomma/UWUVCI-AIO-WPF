namespace WiiUInjector.Configs
{
    /// <summary>
    /// Turbo Grafx16 config
    /// </summary>
    public sealed class Tg16Config : Config
    {
        /// <summary>
        /// Creates a new instance of <see cref="Tg16Config"/> class.
        /// </summary>
        /// <param name="snes"></param>
        public Tg16Config() : base(GameConsole.TG16) { }

        /// <summary>
        /// True if we're injecting a TGCD game.
        /// </summary>
        public bool Cd { get; set; } = false;

    }
}
