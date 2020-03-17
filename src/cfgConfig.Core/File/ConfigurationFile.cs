using cfgConfig.Core.Exceptions;
using cfgConfig.Core.Factory;
using cfgConfig.Core.Helpers;
using System;
using System.IO;

namespace cfgConfig.Core.File
{
    /// <summary>
    /// Contains information about a configuration file
    /// </summary>
    public class ConfigurationFile
    {
        #region Properties

        /// <summary>
        /// The full path where the file is
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Indicates if the file exists or not
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// The size in bytes of the file
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Indicates if the file is a valid configuration file
        /// </summary>
        public bool IsValid { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new configuration file
        /// </summary>
        /// <param name="path">The path where the file is or if it no exists, it can be created later</param>
        public ConfigurationFile(string path)
        {
            // Throw exception if path is a directory
            if (path.IsDirectory())
                throw new ArgumentException("Path cannot be a directory.", nameof(path));

            FileInfo fi = new FileInfo(path);

            // Set variables
            Path = path;
            Exists = fi.Exists;
            FileName = fi.Name;
            Length = fi.Length;

            // If the file exists, check if its a valid configuration file
            if(fi.Exists)
                if (!ConfigFileFactory.IsValidConfigurationFile(path))
                    throw new InvalidConfigurationException(path);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the configuration file if does not exists
        /// </summary>
        public FileStream Create()
        {
            if (!Exists)
                return new FileStream(Path, FileMode.Create);

            throw new InvalidOperationException("Configuration file already exists.");
        }

        /// <summary>
        /// Writes the configuration header in the file
        /// </summary>
        public FileStream WriteHeader() => ConfigFileFactory.GetNewConfigurationFile(this);

        #endregion
    }
}