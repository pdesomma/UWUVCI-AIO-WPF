using System;

namespace WiiUInjector.Exceptions
{
    public class InjectionException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InjectionException"/> class.
        /// </summary>
        /// <param name="message"></param>
        public InjectionException(string message) : base(message) { }

        public Injection Injection { get; private set; }
    }
}
