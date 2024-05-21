namespace WiiUInjector.GitTools.Configs
{
    public sealed class N64Config : Config
    {
        private bool _memPak = false;
        private bool _rumblePak = false;

        public N64Config() : base(GameConsole.N64) { }

        public BackupSize BackupSize { get; set; }
        public BackupType BackupType { get; set; }
        public bool DarkFilter { get; set; }
        public bool ExpansionPak { get; set; }
        public string IniPath { get; set; }
        public bool MemPak
        {
            get => _memPak;
            set
            {
                _memPak = value;
                if (_rumblePak) _rumblePak = false;
            }
        }
        public bool RetraceByVsync { get; set; }
        public bool Rumble
        {
            get => _rumblePak;
            set
            {
                _rumblePak = value;
                if (_memPak) _memPak = false;
            }
        }
        public bool TrueBoot { get; set; }
        public bool UseTimer { get; set; }
        public bool WideScreen { get; set; }
    }
}
