using System;

namespace WiiUInjector.ViewModels.Services
{
    /// <summary>
    /// Service that helps with exceptions.
    /// </summary>
    public interface IExceptionService
    {
        void HandleException(Exception ex);
    }
}