using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace UWUVCI_AIO_WPF
{
    public static class Util
    {
        /// <summary>
        /// Checks if we're connected to the internet.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsConnectedToInternetAsync()
        {
            try
            {
                return (await new Ping().SendPingAsync("google.com", 1000)).Status == IPStatus.Success;
            }
            catch { return false; }
        }
    }

}
