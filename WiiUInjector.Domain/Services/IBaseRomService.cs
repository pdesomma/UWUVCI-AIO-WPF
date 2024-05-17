using System.Collections.Generic;
using System.Threading.Tasks;

namespace WiiUInjector.Services
{
    /// <summary>
    /// Service that manages <see cref="BaseRom"/>s.
    /// </summary>
    public interface IBaseRomService
    {
        /// <summary>
        /// Gets the rom's binaries from a server.
        /// </summary>
        /// <param name="commonKey"></param>
        /// <param name="baseRomDefinition"></param>
        /// <returns></returns>
        Task<string> DownloadBinaryAsync(string commonKey, BaseRom baseRomDefinition);

        /// <summary>
        /// Gets all the <see cref="BaseRom"/> definitions.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<BaseRom>> GetBaseRomDefinitionsAsync(GameConsole console);

        /// <summary>
        /// Updates a <see cref="BaseRom"/>
        /// </summary>
        /// <param name="baseRom"></param>
        /// <returns></returns>
        Task UpdateAsync(BaseRom baseRom);
    }
}
