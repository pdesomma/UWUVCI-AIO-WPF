namespace WiiUInjector.Configs
{
    /// <summary>
    /// NES config
    /// </summary>
    public sealed class NesConfig : Config
    {
        /// <summary>
        /// Creates a new instance of <see cref="NesConfig"/> class.
        /// </summary>
        /// <param name="snes"></param>
        public NesConfig() : base(GameConsole.NES) { }

        /// <summary>
        /// Apply pixel perfect patch.
        /// </summary>
        public bool PixelPerfect { get; set; } = false;

    }
}
