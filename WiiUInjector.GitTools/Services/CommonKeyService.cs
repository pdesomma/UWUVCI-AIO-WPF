using System.IO;
using WiiUInjector.Exceptions;
using WiiUInjector.Services;

namespace WiiUInjector.GitTools.Services
{
    public sealed class CommonKeyService : ICommonKeyService
    {
        private static readonly string s_directory = Directory.GetCurrentDirectory();
        private static readonly string s_path = s_directory + "\\common.key";

        public string CommonKey { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="CommonKeyService"/> class.
        /// </summary>
        public CommonKeyService() { ReadFromDisk(); }

        /// <summary>
        /// Enters the common key into the system.
        /// </summary>
        /// <param name="commonKey"></param>
        /// <exception cref="CommonKeyException"></exception>
        public void SetKey(string commonKey)
        {
            if (1274359530 == commonKey.ToLower().GetHashCode())
            {
                this.CommonKey = commonKey;
                WriteToDisk();
            }
            else throw new CommonKeyException(commonKey);
        }

        /// <summary>
        /// Tries to get the common key from a file
        /// </summary>
        private void ReadFromDisk()
        {
            if(!Directory.Exists(s_directory)) Directory.CreateDirectory(s_directory);
            if(File.Exists(s_path)) CommonKey = File.ReadAllText(s_path);
        }

        /// <summary>
        /// Writes the key to the disk.
        /// </summary>
        private void WriteToDisk()
        {
            if (!Directory.Exists(s_directory)) Directory.CreateDirectory(s_directory);
            File.WriteAllText(s_path, CommonKey);
        }
    }
}
