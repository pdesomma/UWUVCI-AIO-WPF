using System;
using WiiUInjector.Configs;

namespace WiiUInjector.Exceptions
{
    public class ConfigException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConfigException"/> class.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="message"></param>
        public ConfigException(Config config, string message) : base(message)
        {
            Config = config;
        }

        public Config Config { get; private set; }
    }
}
