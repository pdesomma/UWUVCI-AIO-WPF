using System;

namespace WiiUInjector.Exceptions
{
    public class CommonKeyException : Exception
    {
        public string CommonKey { get; private set; }

        public CommonKeyException(string key) :
            base("Key '" + key + "' is not valid")
        {
            CommonKey = key;
        }
        public CommonKeyException(string key, Exception inner) :
            base("Key '" + key + "' is not valid", inner)
        {
            CommonKey = key;
        }
    }
}
