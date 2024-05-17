using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WiiUInjector.GitTools
{
    /// <summary>
    /// Maintains a collection of tools (external processes).
    /// </summary>
    internal static class ToolBox
    {
        /// <summary>
        /// Big collection of known tools.
        /// </summary>
        public static class Tools
        {
            public static ITool BmpTgaTool = new Tool("bmp2tga.exe");
            public static ITool ConvertIsoTool = new Tool("ConvertToISO.exe");
            public static ITool ConvertNKitTool = new Tool("ConvertToNKit.exe");
            public static ITool DecryptTool = new Tool("CDecrypt.exe");
            public static ITool GcBaseTool = new Tool("BASE.zip");
            public static ITool GoombaTool = new Tool("goomba.gba");
            public static ITool JpgTgaTool = new Tool("jpg2tga.exe");
            public static ITool MArchiveBatchTool = new Tool("MArchiveBatchTool.exe");
            public static ITool N64InjectorTool = new Tool("N64Converter.exe");
            public static ITool NesInjectorTool = new Tool("RetroInject.exe");
            public static ITool NintendontForceTool = new Tool("nintendont_force.dol");
            public static ITool NintendontTool = new Tool("nintendont.dol");
            public static ITool Nfs2IsoTool = new Tool("nfs2iso2nfs.exe");
            public static ITool PngTgaTool = new Tool("png2tga.exe", "FreeImage.dll");
            public static ITool PsbTool = new Tool("psb.exe");
            public static ITool RomDownloaderTool = new Tool("WiiUDownloader.exe");
            public static ITool RpxAspectRatioTool = new Tool("ChangeAspectRatio.exe");
            public static ITool RpxCompressorTool = new Tool("wiiurpxtool.exe");
            public static ITool SoxTrimTool = new Tool("sox.exe");
            public static ITool TgaVerifierTool = new Tool("tga_verify.exe");
            public static ITool TurboCdTool = new Tool("BuildTurboCdPcePkg.exe");
            public static ITool TurboTool = new Tool("BuildPcePkg.exe");
            public static ITool WitTool = new Tool("wit.exe");
            public static ITool WupPackTool = new Tool("CNUSPACKER.exe");
        }

        internal static string ToolsDirectory => Directory.GetCurrentDirectory() + "\\tools\\";
        //private static readonly string s_toolDownloadAddress = "https://github.com/Hotbrawl20/UWUVCI-Tools/raw/master/";
        private static readonly string s_toolDownloadAddress = "https://raw.githubusercontent.com/NicoAICP/UWUVCI-Tools/master/";

        /// <summary>
        /// Gets if a tool exists in the tools folder.
        /// </summary>
        /// <param name="toolName"></param>
        /// <returns></returns>
        public static bool Contains(string toolName)
        {
            if (!Directory.Exists(ToolsDirectory)) Directory.CreateDirectory(ToolsDirectory);

            return File.Exists(ToolsDirectory + toolName);
        }

        /// <summary>
        /// Downloads a tool and adds it to the toolbox.
        /// </summary>
        /// <param name="toolName"></param>
        /// <param name="postDownloadProcessing"></param>
        /// <returns></returns>
        public static async Task AddAsync(string toolName, Action postDownloadProcessing = null, bool force = false)
        {
            if (!Directory.Exists(ToolsDirectory)) Directory.CreateDirectory(ToolsDirectory);

            if (File.Exists(ToolsDirectory + toolName) && force) File.Delete(ToolsDirectory + toolName);
            else if (File.Exists(ToolsDirectory + toolName)) return;

            using (var webClient = new WebClient())
            {
                await webClient.DownloadFileTaskAsync(s_toolDownloadAddress + toolName, ToolsDirectory + toolName);
            }
            postDownloadProcessing?.Invoke();
        }
    }
}
