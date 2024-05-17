
namespace WiiUInjector
{
    /// <summary>
    /// Defines a class that creates an <see cref="Injection"/>s 
    /// </summary>
    public interface IInjector
    {
        /// <summary>
        /// Create an <see cref="Injection"/>
        /// </summary>
        /// <returns></returns>
        Injection CreateInjection();
    }
}
