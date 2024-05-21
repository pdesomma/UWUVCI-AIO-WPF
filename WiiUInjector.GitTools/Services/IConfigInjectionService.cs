using WiiUInjector.GitTools.Configs;
using WiiUInjector.Services;

namespace WiiUInjector.GitTools.Services
{
    public interface IConfigInjectionService : IInjectionService
    {
        NdsConfig DS { get; set; }
        GbaConfig GameBoy { get; set; }
        GcConfig GameCube { get; set; }
        MsxConfig Msx { get; set; }
        N64Config N64 { get; set; }
        NesConfig Nes { get; set; }
        SnesConfig Snes { get; set; }
        Tg16Config Tg16 { get; set; }   
        WiiConfig Wii { get; set; }
    }
}
