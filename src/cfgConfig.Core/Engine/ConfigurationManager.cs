using cfgConfig.Core.Engine;
using cfgConfig.Core.Engine.Settings;
using cfgConfig.Core.Exceptions;
using cfgConfig.Core.Implementation.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cfgConfig.Core
{
    /// <summary>
    /// Creates a new manager to handle configurations
    /// </summary>
    public sealed class ConfigurationManager
    {
        #region Private Members

        private string mPath; // The directory where the config files will be saved
        private static readonly IList<ConfigurationManager> mCreatedManagers = new List<ConfigurationManager>(); // Contains all the configuration managers

        #endregion

        #region Constructors

        internal ConfigurationManager(string identifier) { Identifier = identifier; }

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

        /// <summary>
        /// Gets the working path of the current configuration manager
        /// </summary>
        public string Path => mPath;

        /// <summary>
        /// A name that identifies this configuration manager
        /// </summary>
        public string Identifier { get; }

        #endregion

        #region Methods

        #region Static

        /// <summary>
        /// Creates a new <see cref="ConfigurationManager"/>
        /// </summary>
        /// <param name="path">The path to the folder where the config files will be stored</param>
        /// <param name="identifier">A unique name that identifies the manager</param>
        public static ConfigurationManager Make(string path, string identifier)
        {
            // If identifier is null or whitespace, throw exception
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentNullException(nameof(identifier));

            // Avoid managers with same identifier
            if (mCreatedManagers.Any(x => x.Identifier == identifier))
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

            // Create config manager
            var configManager = new ConfigurationManager(identifier);
            configManager.mPath = path; // Set path
            configManager.Setup();

            // Add the manager
            mCreatedManagers.Add(configManager);

            Logger.LogInfo($"Configuration Manager registered at '{path}' with identifier {identifier}.");

            return configManager;
        }

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

        #region Overriden

        /// <summary>
        /// Returns an string representation of the manager that contains
        /// the identifier name and the watching path
        /// </summary>
        public override string ToString() => $"{Identifier}:{Path}";

        #endregion

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
