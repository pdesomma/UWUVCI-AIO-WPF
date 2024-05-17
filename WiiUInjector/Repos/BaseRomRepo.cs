using GameBaseClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using WiiUInjector.Exceptions;

namespace WiiUInjector.Repos
{
    public sealed class BaseRomRepo : IBaseRomRepo
    {
        private static readonly string s_directory = Path.Combine(Directory.GetCurrentDirectory(), "basedefs");
        private static readonly string s_directory_remote = Path.Combine(s_directory, "remote");

        private readonly Dictionary<GameConsole, Dictionary<string, BaseRom>> _definitionLookup = new Dictionary<GameConsole, Dictionary<string, BaseRom>>();
        private readonly string _downloadLink;        

        /// <summary>
        /// Creates a new instance of the <see cref="BaseRomRepo"/> class.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="filePrefix"></param>
        public BaseRomRepo(string baseUrl, string filePrefix) 
        {
            _downloadLink =  Path.Combine(baseUrl, filePrefix);
            if (_downloadLink.Length <= 0) throw new ArgumentException(nameof(baseUrl) + " and " + nameof(filePrefix) + " are invalid.");
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BaseRomRepo"/> class.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <exception cref="ArgumentException"></exception>
        public BaseRomRepo(string baseUrl)
        {
            _downloadLink = baseUrl;
            if (_downloadLink.Length <= 0) throw new ArgumentException(nameof(baseUrl));
            if (_downloadLink[_downloadLink.Length - 1] != '/') _downloadLink += '/';
        }

        /// <summary>
        /// Get all <see cref="BaseRom"/>s for a <see cref="GameConsole"/>.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BaseRom>> GetAsync(GameConsole console)
        {
            if (!Directory.Exists(s_directory)) Directory.CreateDirectory(s_directory);

            List<BaseRom> definitions = new List<BaseRom>();

            // we don't have a local repo file of bases, need to get from server
            if (!File.Exists(Path.Combine(s_directory, console.ToString().ToLower())))
            {
                if (!Directory.Exists(s_directory_remote)) Directory.CreateDirectory(s_directory_remote);

                await DownloadBaseDefinitionAsync(console);
                foreach (var item in ReadBasesFromVcb(console))
                {
                    item.Console = console;
                    definitions.Add(item);
                }
                await WriteToFileAsync(definitions, console); // save for later

                Directory.Delete(s_directory_remote, true);
            }
            else
            {
                //just read from the file we already saved.
                definitions = await ReadBasesAsync(console);
            }

            // set up the dictionary for quick lookup later
            if (!_definitionLookup.TryGetValue(console, out var _)) _definitionLookup.Add(console, new Dictionary<string, BaseRom>());
            foreach(var definition in definitions)
            {
                if (!_definitionLookup[console].ContainsKey(definition.TitleId)) _definitionLookup[console].Add(definition.TitleId, definition);
            }

            return definitions;
        }

        /// <summary>
        /// Update a base in the system.
        /// </summary>
        /// <param name="rom"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BaseRom rom)
        {
            if (!_definitionLookup.TryGetValue(rom.Console, out var dict)) throw new BaseRomException(rom, "Not a valid console: " + rom.Console.ToString());
            if (!dict.TryGetValue(rom.TitleId, out var _)) throw new BaseRomException(rom, "Not a valid TitleId: " + rom.TitleId);

            dict[rom.TitleId] = rom;
            await WriteToFileAsync(dict.Values, rom.Console);
        }

        /// <summary>
        /// Downloads a base definition for the <see cref="GameConsole"/> to the disk.
        /// </summary>
        /// <returns></returns>
        private async Task DownloadBaseDefinitionAsync(GameConsole console)
        {
            string consoleName = console.ToString().ToLower();
            Directory.CreateDirectory(s_directory_remote);
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(_downloadLink + "vcb" + consoleName), GetDownloadedFile(console));
            }
        }

        /// <summary>
        /// Writes <see cref="BaseRom"/>s to a file
        /// </summary>
        /// <param name="baseRoms"></param>
        /// <param name="console"></param>
        private async Task WriteToFileAsync(IEnumerable<BaseRom> baseRoms, GameConsole console)
        {
            if (!Directory.Exists(s_directory)) Directory.CreateDirectory(s_directory);

            using (FileStream fileStream = new FileStream(s_directory + "\\" + console.ToString().ToLower(), FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fileStream, baseRoms);
                fileStream.Close();
            }
        }

        /// <summary>
        /// Location on the local disk of the game base files we're saving.
        /// </summary>
        private static string GetFile(GameConsole console) => s_directory + "\\" + console.ToString().ToLower();

        /// <summary>
        /// Location on the local disk of the game base files we got from the server.
        /// </summary>
        private static string GetDownloadedFile(GameConsole console) => s_directory_remote + "\\bases.vcb" + console.ToString().ToLower();

        /// <summary>
        /// Deserializes <see cref="BaseRom"/>s from our file.
        /// </summary>
        /// <returns></returns>
        private async Task<List<BaseRom>> ReadBasesAsync(GameConsole console)
        {
            using (FileStream fs = File.OpenRead(GetFile(console)))
            {
                List<BaseRom> roms = await JsonSerializer.DeserializeAsync<List<BaseRom>>(fs);
                fs.Close();
                return roms;
            }
        }

        /// <summary>
        /// Deserializes <see cref="BaseRom"/>s from a vcb file.
        /// </summary>
        /// <returns></returns>
        private List<BaseRom> ReadBasesFromVcb(GameConsole console)
        {
            using (var serializationStream = new GZipStream(new FileStream(GetDownloadedFile(console), FileMode.Open, FileAccess.Read), CompressionMode.Decompress))
            {
                var result = (List<GameBases>)(new BinaryFormatter()).Deserialize(serializationStream);
                return result.Select(x => new BaseRom { KeyHash = x.KeyHash, Name = x.Name, Path = x.Path, Region = (Region)Enum.Parse(typeof(Region), x.Region.ToString()), TitleId = x.Tid }).ToList();

            }
        }
    }
}
