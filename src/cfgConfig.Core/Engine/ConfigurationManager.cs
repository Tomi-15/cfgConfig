using cfgConfig.Core.Engine;
using cfgConfig.Core.Engine.Settings;
using cfgConfig.Core.Implementation.Base;
using System.IO;

namespace cfgConfig.Core
{
    /// <summary>
    /// Creates a new manager to handle configurations
    /// </summary>
    public sealed class ConfigurationManager
    {
        #region Private Members

        private string mPath; // The directory where the config files will be saved

        #endregion

        #region Constructors

        internal ConfigurationManager() { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property that is used to implement new configurations to the manager
        /// </summary>
        public ConfigImplementer Implementations { get; internal set; }

        /// <summary>
        /// Property that is used to configure the manager
        /// </summary>
        public ConfigurationManagerSettings Settings { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ConfigurationManager"/>
        /// </summary>
        /// <param name="path">The path to the folder where the config files will be stored</param>
        /// <param name="useConsole">If its true, instead of throwing exceptions when something happens, it will send a message to the console. Otherwise, it will throw an exception.</param>
        public static ConfigurationManager Make(string path, bool useConsole = false)
        {
            // If the directory does not exists, create it
            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
                throw;
            }

            // Create config manager
            var configManager = new ConfigurationManager();
            configManager.mPath = path; // Set path
            configManager.Setup();

            // Set global settings
            GlobalSettings.UseConsole = useConsole;

            return configManager;
        }

        #endregion

        #region Private Helper Methods

        private void Setup()
        {
            Implementations = new ConfigImplementer(this);
            Settings = new ConfigurationManagerSettings();
        }

        #endregion
    }
}
