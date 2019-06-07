using System.IO;

namespace cfgConfig.Core.Files
{
    /// <summary>
    /// Represents a configuration file
    /// </summary>
    public sealed class ConfigFile
    {
        #region Public Properties

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The path to the file with its name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Indicates if the file exists
        /// </summary>
        public bool Exists { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ConfigFile
        /// </summary>
        /// <param name="path">The path to the file</param>
        public ConfigFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            Name = fi.Name;
            FullName = fi.FullName;
            Exists = fi.Exists;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the file
        /// </summary>
        public void Create()
        {
            try
            {
                using (var fs = new FileStream(FullName, FileMode.Create)) { }
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
