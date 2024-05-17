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

        public BaseRom BaseRom { get; private set; }
    }
}
