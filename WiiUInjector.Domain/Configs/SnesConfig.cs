namespace WiiUInjector.Configs
{
    /// <summary>
    /// SNES config
    /// </summary>
    public sealed class SnesConfig : Config
    {
        /// <summary>
        /// Creates a new instance of <see cref="SnesConfig"/> class.
        /// </summary>
        /// <param name="snes"></param>
        public SnesConfig() : base(GameConsole.SNES) { }

        /// <summary>
        /// Apply pixel perfect patch.
        /// </summary>
        public bool PixelPerfect { get; set; } = false;

    }
}
