using System.ComponentModel;

namespace WiiUInjector.GitTools
{
    public enum WiiUseGamePadAs
    {
        [Description("Do not use. WiiMotes only")]
        None = 0,
        [Description("Classic Controller")]
        ClassicController,
        [Description("Horizontal WiiMote")]
        HorizontalWiiMote,
        [Description("Vertical WiiMote")]
        VerticalWiiMote,
        [Description("Force Classic Controller")]
        ForceClassic,
        [Description("Force No Classic Controller")]
        ForceNoClassic
    }
}
