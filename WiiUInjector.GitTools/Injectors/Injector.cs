using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using WiiUInjector.GitTools.Configs;
using WiiUInjector.Exceptions;

namespace WiiUInjector.GitTools
{
    public delegate void StatusChangeEventHandler(object sender, string status);

    public abstract class Injector<TConfig> where TConfig : Config
    {
        private readonly string _workingDirectory;

        /// <summary>
        /// Creates a new instance of the <see cref="Injector"/> class.
        /// </summary>
        public Injector(string workingDirectory) 
        {
            _workingDirectory = workingDirectory;
        }

        public string Status { get; private set; }
        public int Index { get; protected set; } = -1;
        public string ProdCode { get; protected set; }
        public event StatusChangeEventHandler StatusChanged;

        /// <summary>
        /// Creates a WiiU injection from a <see cref="BaseRom"/>.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="ConfigException" />
        /// <exception cref="IOException" />
        public async Task<Injection> InjectAsync(TConfig config, BaseRom baseRom, Metadata metadata, bool force)
        {
            // fail fast
            if (config.RomPath is null || (!File.Exists(config.RomPath) && !Directory.Exists(config.RomPath))) throw new FileNotFoundException("The rom path '" + config.RomPath + "' is invalid or cannot be found. Make sure it exists!");
            if (baseRom is null) throw new BaseRomException("Injection base is null");
            if (baseRom.TitleId is null) throw new BaseRomException("Injection base title id is null");
            if (baseRom.Path is null || !Directory.Exists(baseRom.Path)) throw new DirectoryNotFoundException("Injection base path is invalid");
            if (!Directory.Exists(Path.Combine(baseRom.Path, "code"))) throw new DirectoryNotFoundException("Injection base code folder is missing");
            if (!Directory.Exists(Path.Combine(baseRom.Path, "content"))) throw new DirectoryNotFoundException("Injection base content folder is missing");
            if (!Directory.Exists(Path.Combine(baseRom.Path, "meta"))) throw new DirectoryNotFoundException("Injection base meta folder is missing");
            if (string.IsNullOrWhiteSpace(baseRom.Name)) throw new NullReferenceException("Config game name is null");
            if (metadata.BootSoundPath != null && !File.Exists(metadata.BootSoundPath)) throw new FileNotFoundException("Metadata boot sound is not found");

            // move everything into a temp directory from the base rom path.
            Directory.CreateDirectory(_workingDirectory);
            await Task.Run(() => DirectoryCopy(baseRom.Path, _workingDirectory));

            Preprocess(config, baseRom);
            await RunSpecificInjectionAsync(config, baseRom, _workingDirectory, force);

            Random random = new Random();
            string ID = $"{random.Next(0x3000, 0x10000):X4}{random.Next(0x3000, 0x10000):X4}";
            string ID2 = $"{random.Next(0x3000, 0x10000):X4}";

            EditAppXml(_workingDirectory, ID, ID2);
            EditMetaXML(config.Name, _workingDirectory, ID, ID2);
            Postprocess(config, baseRom);

            MoveFile(metadata.IconPath, Path.Combine(_workingDirectory, "meta"));
            MoveFile(metadata.TvPath, Path.Combine(_workingDirectory, "meta"));
            MoveFile(metadata.LogoPath, Path.Combine(_workingDirectory, "meta"));
            MoveFile(metadata.GamePadPath, Path.Combine(_workingDirectory, "meta"));
            MoveFile(metadata.BootSoundPath, Path.Combine(_workingDirectory, "meta"));

            return new Injection() { ProdCode = ID2, Path = _workingDirectory, Name = config.Name };
        }

        /// <summary>
        /// Deep copies a source directory to a destination directory. Creates the destination directory.
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        protected static void DirectoryCopy(string sourcePath, string targetPath)
        {
            //create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        /// <summary>
        /// Do some sort of preprocessing to the files.
        /// </summary>
        /// <param name="config"></param>
        protected virtual void Preprocess(TConfig config, BaseRom injectionBase) { }

        /// <summary>
        /// Do some sort of postprocessing to the files.
        /// </summary>
        /// <param name="config"></param>
        protected virtual void Postprocess(TConfig config, BaseRom injectionBase) { }

        /// <summary>
        /// Does specific injection work depending on the config/console
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        protected abstract Task RunSpecificInjectionAsync(TConfig config, BaseRom injectionBase, string directory, bool force);

        /// <summary>
        /// Sets the status and raises the status changed event.
        /// </summary>
        /// <param name="status"></param>
        protected void SetStatus(string status) => StatusChanged?.Invoke(this, Status = status);

        /// <summary>
        /// Edit the newly creeated meta.xml file in the meta folder of the injection.
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="directory"></param>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        private void EditMetaXML(string gameName, string directory, string id1, string id2)
        {
            gameName = gameName.Replace('|', ',');
            string xml = Path.Combine(directory, "meta", "meta.xml");

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xml);
                doc.SelectSingleNode("menu/longname_ja").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_en").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_fr").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_de").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_it").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_es").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_zhs").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_ko").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_nl").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_pt").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_ru").InnerText = gameName.Replace(",", "\n");
                doc.SelectSingleNode("menu/longname_zht").InnerText = gameName.Replace(",", "\n");

                doc.SelectSingleNode("menu/product_code").InnerText = $"WUP-N-{id2}";
                if (Index > 0)
                {
                    doc.SelectSingleNode("menu/drc_use").InnerText = "65537";
                }
                doc.SelectSingleNode("menu/title_id").InnerText = $"00050002{id1}";
                doc.SelectSingleNode("menu/group_id").InnerText = $"0000{id2}";

                doc.SelectSingleNode("menu/shortname_ja").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_fr").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_de").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_en").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_it").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_es").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_zhs").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_ko").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_nl").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_pt").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_ru").InnerText = gameName.Split(',')[0];
                doc.SelectSingleNode("menu/shortname_zht").InnerText = gameName.Split(',')[0];
                doc.Save(xml);
            }
            catch (NullReferenceException) { }
        }

        /// <summary>
        /// Updates the newly created app.xml file in the injection's 'code' folder.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        private void EditAppXml(string directory, string id1, string id2)
        {
            string xml = Path.Combine(directory, "code", "app.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xml);
            doc.SelectSingleNode("app/title_id").InnerText = $"00050002{id1}";
            doc.SelectSingleNode("app/group_id").InnerText = $"0000{id2}";
            doc.Save(xml);
        }

        /// <summary>
        /// Move a file from one folder to another.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="destDir"></param>
        private static void MoveFile(string fullPath, string destDir)
        {
            if (fullPath == null) return;
            string fileName = Path.GetFileName(fullPath);
            if (File.Exists(Path.Combine(destDir, fileName)))
            {
                if (File.Exists(Path.Combine(destDir, fileName))) { File.Delete(Path.Combine(destDir, fileName)); }
                File.Move(fullPath, Path.Combine(destDir, fileName));
            }
        }
    }
}
