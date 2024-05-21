using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WiiUInjector.GitTools
{
    internal delegate void ToolCompletedEventHandler(ToolResponse response);

    /// <summary>
    /// Class that runs external tools asynchronously to do injection work.
    /// </summary>
    internal sealed class Tool : ITool
    {
        /// <summary>
        /// List of required support tools/libraries.
        /// </summary>
        private readonly List<string> _support = new List<string>();

        /// <summary>
        /// Creates a new instance of the <see cref="Tool"/> class.
        /// </summary>
        /// <param name="name"></param>
        public Tool(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Tool"/> class.
        /// </summary>
        /// <param name="name"></param>
        public Tool(string name, params string[] support) : this(name)
        {
            foreach (string s in support)
            {
                _support.Add(s);
            }
        }

        /// <summary>
        /// Copy the tool to a new location on the disk.
        /// </summary>
        /// <param name="path"></param>
        public ITool Copy(string path)
        {
            if (Location != null)
            {
                File.Copy(Path.Combine(Location, Name), Path.Combine(path, Name));
                foreach (var s in _support)
                {
                    File.Copy(Path.Combine(Location, s), Path.Combine(path, s));
                }
                Location = path;
            }
            var copyTool = new Tool(this.Name, this._support.ToArray()) { Location = path };
            return copyTool;
        }

        /// <summary>
        /// Delete itself from the disk.
        /// </summary>
        public void Delete()
        {
            if (Location != null)
            {
                File.Delete(Path.Combine(Location, Name));
                foreach (var s in _support)
                {
                    File.Delete(Path.Combine(Location, s));
                }
            }
        }

        /// <summary>
        /// File location of the tool.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Tool's file name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Subscribe to the completion event of the process.
        /// </summary>
        public event ToolCompletedEventHandler OnCompletion;

        /// <summary>
        /// Move the tool to a new location on the disk.
        /// </summary>
        /// <param name="path"></param>
        public void Move(string path)
        {
            if (Location != null)
            {
                File.Move(Path.Combine(Location, Name), Path.Combine(path, Name));
                foreach(var s in _support)
                {
                    File.Move(Path.Combine(Location, s), Path.Combine(path, s));    
                }
                Location = path;
            }
        }

        /// <summary>
        /// Use the tool asynchronously (run the process)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<ToolResponse> UseAsync(string args = null)
        {
            if (Location is null)
            {
                await ToolBox.AddAsync(Name, null);
                Location = ToolBox.ToolsDirectory;
            }

            foreach (var s in _support)
            {
                if (!ToolBox.Contains(s)) await ToolBox.AddAsync(s, null);
            }

            var taskCompletionSource = new TaskCompletionSource<ToolResponse>();
            string error = null;
            string output = null;
            using (var process = new Process()
            {
                StartInfo =
                {
                    FileName = ToolBox.ToolsDirectory + Name,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            })
            {
                process.ErrorDataReceived += (s, e) => error += e.Data;
                process.OutputDataReceived += (s, e) => output += e.Data;

                if (!string.IsNullOrWhiteSpace(args)) process.StartInfo.Arguments = args;
                process.Exited += (sender, a) =>
                {
                    var response = new ToolResponse() { Error = error, Output = output };
                    taskCompletionSource.SetResult(response);
                    OnCompletion?.Invoke(response);
                };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                var result = await taskCompletionSource.Task;
                return result;
            }
        }
    }
}
