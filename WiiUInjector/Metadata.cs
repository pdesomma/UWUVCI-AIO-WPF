namespace WiiUInjector.Configs
{
    /// <summary>
    /// Extra metadata to add to the injection.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Path of the boot sound file to inject.
        /// </summary>
        public string BootSoundPath { get; set; }

        /// <summary>
        /// Path of the GamePad image to inject.
        /// </summary>
        public string GamePadPath { get; set; }

        /// <summary>
        /// Path of the Icon image to inject.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Path of the Logo image to inject.
        /// </summary>
        public string LogoPath { get; set; }

        /// <summary>
        /// Path of the TV image to inject.
        /// </summary>
        public string TvPath { get; set; }
    }
}
