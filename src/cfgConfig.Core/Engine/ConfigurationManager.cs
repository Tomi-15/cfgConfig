using cfgConfig.Core.Backups;
using cfgConfig.Core.Engine;
using cfgConfig.Core.Engine.Settings;
using cfgConfig.Core.Exceptions;
using cfgConfig.Core.Files;
using cfgConfig.Core.Implementation.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace cfgConfig.Core
{
    /// <summary>
    /// Creates a new manager to handle configurations
    /// </summary>
    public sealed class ConfigurationManager
    {
        #region Private Members

        private string mPath; // The directory where the config files will be saved
        private BackupManager mBackupManager;
        internal static readonly IList<ConfigurationManager> mCreatedManagers = new List<ConfigurationManager>(); // Contains all the configuration managers

        #endregion

        #region Constructors

        internal ConfigurationManager(string identifier, string path)
        {
            Identifier = identifier;
            mPath = path;
            Implementations = new ConfigImplementer(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property that is used to implement new configurations to the manager
        /// </summary>
        public ConfigImplementer Implementations { get; internal set; }

        /// <summary>
        /// Gets the working path of the current configuration manager
        /// </summary>
        public string Path => mPath;

        /// <summary>
        /// A name that identifies this configuration manager
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The serialization mode
        /// </summary>
        public SaveModes SaveMode { get; internal set; }

        /// <summary>
        /// Indicates if the encryptation is enabled
        /// </summary>
        public bool Encryptation { get; internal set; }

        #endregion

        #region Methods

        #region Static

        /// <summary>
        /// Saves any pending configuration change and terminates all the managers created
        /// </summary>
        public static void Terminate()
        {
            // If there are no managers, throw exception
            if (mCreatedManagers.Count <= 0)
                throw new InvalidOperationException("There is no Configuration Managers created.");

            // Foreach created manager
            foreach (var manager in mCreatedManagers)
            {
                // Save all implementations
                manager.Implementations.SaveAllImplementations();

                // Create backups if enabled
                if(manager.mBackupManager != null)
                    manager.mBackupManager.CreateBackups();

                Logger.LogInfo($"Saved {manager.Implementations.Count} implementations for the manager '{manager.Identifier}'.");
            }
        }

        /// <summary>
        /// Search and returns a <see cref="ConfigurationManager"/>. If not found, it will return null
        /// </summary>
        /// <param name="identifier">The identifier of the configuration manager to return</param>
        public static ConfigurationManager GetManager(string identifier)
            => mCreatedManagers.FirstOrDefault(x => x.Identifier == identifier);

        /// <summary>
        /// If its true, instead of throwing exceptions when something happens, it will send a message to the console. Otherwise, it will throw an exception.
        /// </summary>
        public static void UseConsole() => GlobalSettings.UseConsole = true;

        /// <summary>
        /// Returns a <see cref="ConfigurationManager"/> who implements a configuration type
        /// </summary>
        /// <typeparam name="TConfig">The configuration type</typeparam>
        internal static ConfigurationManager GetManagerWhoImplements<TConfig>()
            => mCreatedManagers.FirstOrDefault(x => x.Implementations.Has<TConfig>());

        #endregion

        /// <summary>
        /// Gets a configuration type from the manager
        /// </summary>
        /// <typeparam name="TConfig">The type of configuration to get</typeparam>
        public TConfig GetConfig<TConfig>()
        {
            // If the implementation is not found, throw exception
            if (!Implementations.GetConfigImplementations().Any(x => x.Type == typeof(TConfig)))
                throw new ConfigNotFoundException($"Configuration of type {typeof(TConfig).ToString()} not found.");

            // Return the runtime instance of the implementation
            return Implementations.GetImplementation<TConfig>().RuntimeInstance;
        }

        /// <summary>
        /// Gets a configuration type from the manager
        /// </summary>
        /// <param name="configType">The type of configuration to get</param>
        public object GetConfig(Type configType)
        {
            // If the implementation is not found, throw exception
            if (!Implementations.GetConfigImplementations().Any(x => x.Type == configType))
                throw new ConfigNotFoundException($"Configuration of type {configType.ToString()} not found.");

            // Return the runtime instance of the implementation
            return Implementations.GetBaseImplementation(configType).RuntimeInstance;
        }

        /// <summary>
        /// Restores the configurations from a backup file. Once restore is completed, all backup files will be deleted.
        /// NOTE: This must be called one time, and then, remove the line from the code
        /// </summary>
        public void RestoreLastBackup()
        {
            // Prevent bugs
            if (mBackupManager == null)
                throw new InvalidOperationException("Backup manager not enabled.");

            // Restore backups
            mBackupManager.RestoreLastBackup();
        }

        #region Internal

        internal void SetupAutoSave(TimeSpan timeout)
        {
            // If timeout is infinite or zero, return
            if (timeout == new TimeSpan(0, 0, 0, 0, -1) || timeout == TimeSpan.Zero)
                return;

            // Create timer
            Timer timer = new Timer();
            timer.Interval = timeout.TotalMilliseconds;
            timer.Elapsed += (s, e) =>
            {
                Implementations.SaveAllImplementations();
            };
            timer.Start(); // Start the timer

            Logger.LogInfo($"Auto save configured to work each {timeout.ToString()}.");
        }

        internal void SetupBackups()
        {
            mBackupManager = new BackupManager();
            mBackupManager.ConfigureBackupForManager(this, System.IO.Path.Combine(Path, "Backups"));
        }

        internal void SetupEncryptation()
        {
            Encryptation = true;
        }

        #endregion

        #region Overriden

        /// <summary>
        /// Returns an string representation of the manager that contains
        /// the identifier name and the watching path
        /// </summary>
        public override string ToString() => $"{Identifier}:{Path}";

        #endregion

        #endregion
    }

    /// <summary>
    /// Class used to create <see cref="ConfigurationManager"/>s
    /// </summary>
    public sealed class ConfigurableManager
    {
        #region Public Properties

        /// <summary>
        /// The settings to configure the manager
        /// </summary>
        public ConfigurationManagerSettings Settings => mSettings;

        #endregion

        #region Constructors

        internal ConfigurableManager(string identifier, string path)
        {
            mSettings = new ConfigurationManagerSettings();
            mPath = path;
            mIdentifier = identifier;
        }

        #endregion

        #region Private Members

        private string mIdentifier;
        private string mPath;
        private ConfigurationManagerSettings mSettings;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ConfigurationManager"/>
        /// </summary>
        /// <param name="path">The path to the folder where the config files will be stored</param>
        /// <param name="identifier">A unique name that identifies the manager</param>
        public static ConfigurableManager Make(string path, string identifier)
        {
            // If identifier is null or whitespace, throw exception
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentNullException(nameof(identifier));

            // Avoid managers with same identifier
            if (ConfigurationManager.mCreatedManagers.Any(x => x.Identifier == identifier))
                throw new DuplicatedConfigException($"A Configuration Manager with the identifier '{identifier}' already exists.");

            // If the directory does not exists, create it
            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
                throw;
            }

            return new ConfigurableManager(identifier, path);
        }

        /// <summary>
        /// Configures the settings of the manager
        /// </summary>
        public ConfigurableManager Configure(Action<ConfigurationManagerSettings> settings)
        {
            // Set settings
            settings.Invoke(mSettings);

            return this;
        }

        /// <summary>
        /// Builds the configuration manager
        /// </summary>
        public ConfigurationManager Build()
        {
            // Create config manager
            var configManager = new ConfigurationManager(mIdentifier, mPath);

            // Set settings
            configManager.SetupAutoSave(Settings.AutoSaveTimeout); // Configure auto save
            configManager.SaveMode = Settings.SaveMode; // Serialization mode
            if(mSettings.EncryptationEnabled) configManager.SetupEncryptation(); // Encryptation 
            if(Settings.CreateBackups) configManager.SetupBackups(); // Configure backups

            // Add the manager
            ConfigurationManager.mCreatedManagers.Add(configManager);

            Logger.LogInfo($"Configuration Manager registered at '{mPath}' with identifier {mIdentifier}.");

            return configManager;
        }

        #endregion
    }
}
