
namespace WiiUInjector
{
    /// <summary>
    /// Defines a class that creates an <see cref="Injection"/>s 
    /// </summary>
    public abstract class Injector<TConfig> : IInjector where TConfig : Config
    {
        /// <summary>
        /// Create an <see cref="Injection"/>
        /// </summary>
        /// <returns></returns>
        Injection CreateInjection();
    }
}
