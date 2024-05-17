using System.Collections.Generic;
using System.Threading.Tasks;

namespace WiiUInjector.Repos
{
    public interface IBaseRomDefinitionRepo
    {
        /// <summary>
        /// Gets all <see cref="BaseRom"/>s for a console.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<BaseRom>> GetAsync(GameConsole console);

        /// <summary>
        /// Gets all <see cref="BaseRom"/>s for a console.
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync(BaseRom rom);
    }
}
