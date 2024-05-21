using System.Threading.Tasks;

namespace WiiUInjector.GitTools
{
    /// <summary>
    /// An external process that can be executed asynchronously to do injection work.
    /// </summary>
    internal interface ITool
    {
        event ToolCompletedEventHandler OnCompletion;

        string Name { get; }
        string Location { get; }

        void Delete();
        ITool Copy(string path);
        void Move(string path);
        Task<ToolResponse> UseAsync(string args = null);
    }
}
