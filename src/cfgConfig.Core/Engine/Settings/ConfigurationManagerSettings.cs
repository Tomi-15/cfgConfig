using cfgConfig.Core.Files;
using System;

namespace cfgConfig.Core.Engine.Settings
{
    /// <summary>
    /// Contains the settings to build any <see cref="ConfigurationManager"/>
    /// </summary>
    public class ConfigurationManagerSettings
    {
        #region Public Properties

        internal SaveModes SaveMode { get; set; }
        internal TimeSpan AutoSaveTimeout { get; set; } = new TimeSpan(0, 0, 0, 0, -1);
        internal bool CreateBackups { get; set; } // TODO: Improve backup systems
        internal bool EncryptationEnabled { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Indicates the manager to use an specific save mode
        /// </summary>
        /// <param name="mode">The mode to use</param>
        public ConfigurationManagerSettings WithSaveMode(SaveModes mode)
        {
            SaveMode = mode;

            return this;
        }

        /// <summary>
        /// Indicates the manager to save all the configurations at specific interval of time
        /// </summary>
        /// <param name="interval">The interval timespan</param>
        public ConfigurationManagerSettings WithAutoSaveEach(TimeSpan interval)
        {
            AutoSaveTimeout = interval;
            //mManager.SetupAutoSave();

            return this;
        }

        /// <summary>
        /// Indicates the manager to create backup files of the configurations files 
        /// </summary>
        public ConfigurationManagerSettings ConfigureBackups()
        {
            CreateBackups = true;

            return this;
        }

        /// <summary>
        /// Indicates the manager to encrypt the files before serialize
        /// </summary>
        public ConfigurationManagerSettings Encrypt()
        {
            EncryptationEnabled = true;

            return this;
        }

        #endregion
    }
}
