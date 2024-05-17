using WiiUInjector.Exceptions;

namespace WiiUInjector.Services
{
    public interface ICommonKeyService
    {
        /// <summary>
        /// The valid common key.
        /// </summary>
        string CommonKey { get; }

        /// <summary>
        /// Sets the common key.
        /// </summary
        /// <param name="commonKey"></param>
        /// <exception cref="CommonKeyException" />
        void SetKey(string commonKey);
    }
}
