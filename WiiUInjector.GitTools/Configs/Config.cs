namespace WiiUInjector.GitTools.Configs
{
    public class Config
    {
        public GameConsole Console { get; private set; }
        public string Name { get; set; }
        public string RomPath { get; set; }

        public Config(GameConsole console) { Console = console; }
    }
}
