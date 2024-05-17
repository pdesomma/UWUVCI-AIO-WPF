using System.Threading.Tasks;

namespace WiiUInjector.GitTools
{
    /// <summary>
    /// An external process that can be executed asynchronously to do injection work.
    /// </summary>
    internal interface ITool
    {
        string Name { get; }
        string Location { get; }
        event ToolCompletedEventHandler OnCompletion;
        void Move(string path);
        Task<ToolResponse> UseAsync(string args = null);
    }
}
