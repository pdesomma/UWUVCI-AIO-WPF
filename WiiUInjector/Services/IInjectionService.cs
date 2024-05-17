using System.Threading.Tasks;
using WiiUInjector.Configs;

namespace WiiUInjector.Services
{
    /// <summary>
    /// Creates <see cref="Injection"/>s from <see cref="BaseRom"/>s and <see cref="Config"/>s.
    /// </summary>
    public interface IInjectionService
    {
        /// <summary>
        /// Create a GameBoy injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectGameBoyAsync(GbaConfig config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create a GameCube3 injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectGameCubeAsync(GcConfig config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create an MSX injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectMsxAsync(MsxConfig config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create an N64 injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectNintendo64Async(N64Config config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create a NintendoDS injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectNintendoDsAsync(NdsConfig config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create an NES injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectNesAsync(NesConfig config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create an SNES injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectSuperNintendoAsync(SnesConfig config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Create a TG16 injection.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        Task<Injection> InjectTurboGrafx16Async(Tg16Config config, BaseRom baseRom, Metadata metadata, bool force);

        /// <summary>
        /// Makes a wup from an injection.
        /// </summary>
        /// <returns></returns>
        Task<string> PackWupAsync(Injection injection, string commonKey);

        /// <summary>
        /// Makes a loadiine from an injection.
        /// </summary>
        /// <returns></returns>
        Task<string> PackLoadiineAsync(Injection injection);
    }
}
