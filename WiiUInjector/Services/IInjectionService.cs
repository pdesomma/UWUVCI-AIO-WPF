using System.Threading.Tasks;

namespace WiiUInjector.Services
{
    /// <summary>
    /// Creates <see cref="Injection"/>s from <see cref="BaseRom"/>s and <see cref="Metadata"/>
    /// </summary>
    public interface IInjectionService
    {
        /// <summary>
        /// Create an injection for a specific console type.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="baseRom"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        Task<Injection> InjectAsync(GameConsole console, BaseRom baseRom, Metadata metadata);

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
