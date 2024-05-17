using System;

namespace WiiUInjector.Exceptions
{
    public class BaseRomException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="BaseRomException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public BaseRomException(string message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the <see cref="BaseRomException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public BaseRomException(BaseRom rom, string message) : this(message) { BaseRom = rom; }

        public BaseRom BaseRom { get; private set; }
    }
}
