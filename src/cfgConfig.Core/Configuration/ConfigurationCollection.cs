using cfgConfig.Core.Exceptions;
using cfgConfig.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cfgConfig.Core.Configuration
{
    /// <summary>
    /// A collection of configurations in the environment
    /// </summary>
    internal class ConfigurationCollection
    {
        #region Constants

        private const int MAX_CONFIGURATIONS_PER_ENVIRONMENT = 10;

        #endregion

        #region Members

        private Dictionary<string, IConfiguration> mConfigurations;
        private IList<ConfigurationSettings> mConfigurationsSettings;
        private IList<Type> mConfigurationsTypes;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConfigurationCollection()
        {
            mConfigurations = new Dictionary<string, IConfiguration>(MAX_CONFIGURATIONS_PER_ENVIRONMENT);
            mConfigurationsSettings = new List<ConfigurationSettings>(MAX_CONFIGURATIONS_PER_ENVIRONMENT);
            mConfigurationsTypes = new List<Type>(MAX_CONFIGURATIONS_PER_ENVIRONMENT);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a configuration
        /// </summary>
        /// <param name="configuration">The configuration to add</param>
        /// <param name="name">The name of the configuration</param>
        public void Add(IConfiguration configuration, string name, ConfigurationSettings settings)
        {
            // Validate parameters
            Validator.Validate(name);
            Validator.Validate(configuration);

            // Duplicated configuration
            if (mConfigurations.ContainsKey(name))
                throw new DuplicatedConfigurationException(name);

            // Reached max size
            if (mConfigurations.Count == 10)
                throw new FullEnvironmentException(MAX_CONFIGURATIONS_PER_ENVIRONMENT);

            // Add the configuration
            mConfigurations.Add(name, configuration);
            mConfigurationsTypes.Add(configuration.GetType());

            // If settings are null, set defaults
            if (settings == null)
                settings = new ConfigurationSettings();

            mConfigurationsSettings.Add(settings);
        }

        /// <summary>
        /// Removes a configuration by name
        /// </summary>
        /// <param name="name">The name of the configuration</param>
        public void Remove(string name)
        {
            Validator.Validate(name);

            // Configuration not found
            if (!mConfigurations.ContainsKey(name))
                throw new ConfigurationNotFoundException(name);

            // Remove from the list
            int index = mConfigurations.Values.ToList().IndexOf(mConfigurations[name]);
            mConfigurationsSettings.RemoveAt(index);
            mConfigurationsTypes.RemoveAt(index);
            mConfigurations.Remove(name);
        }

        /// <summary>
        /// Returns the amount of added configurations
        /// </summary>
        public int Count() => mConfigurations.Count;

        /// <summary>
        /// Returns true if a configuration exists
        /// </summary>
        /// <param name="name">The name of the configuration to check if exists</param>
        public bool Exists(string name) => mConfigurations.ContainsKey(name);

        /// <summary>
        /// Returns true if a configuration exists
        /// </summary>
        /// <param name="configuration">The configuration to check if exists</param>
        public bool Exists(IConfiguration configuration) => mConfigurations.ContainsValue(configuration);

        /// <summary>
        /// Checks if a configuration matches with a specific type
        /// </summary>
        /// <param name="configuration">The configuration to check</param>
        /// <param name="type">The type that configuration must be</param>
        public bool TypeMatches(IConfiguration configuration, Type type)
        {
            Validator.ValidateAll(configuration, type);

            // Configuration not found
            if (!Exists(configuration))
                throw new ConfigurationNotFoundException(null);

            // Get index of the configuration
            int index = mConfigurations.Values.ToList().IndexOf(configuration);

            // The correct type of the configuration
            Type configurationType = mConfigurationsTypes.ElementAt(index);

            // If its no equals, return false
            if (configurationType != type)
                return false;

            // Otherwise, return true
            return true;
        }

        /// <summary>
        /// Finds a configuration by its name and returns it
        /// </summary>
        /// <param name="name">The name of the configuration to search</param>
        public IConfiguration Find(string name)
        {
            Validator.Validate(name);

            // Not found configuration
            if (!mConfigurations.ContainsKey(name))
                throw new ConfigurationNotFoundException(name);

            // Get the configuration
            IConfiguration configuration = mConfigurations.First(x => x.Key == name).Value;

            // Return it
            return configuration;
        }

        /// <summary>
        /// Finds a configuration by its name and its type and returns it
        /// </summary>
        /// <param name="name">The name of the configuration to get</param>
        /// <param name="type">The type of the configuration to get</param>
        public IConfiguration Find(string name, Type type)
        {
            Validator.Validate(type);

            // Try to get the configuration
            IConfiguration configuration = Find(name);

            // Invalid type
            if (!TypeMatches(configuration, type))
                throw new InvalidConfigurationTypException(name);

            return configuration;
        }

        /// <summary>
        /// Returns the <see cref="ConfigurationSettings"/> of a <see cref="IConfiguration"/>
        /// </summary>
        /// <param name="configuration">The configuration to get its settings</param>
        public ConfigurationSettings FindSettings(IConfiguration configuration)
        {
            Validator.Validate(configuration);

            // Configuration not found
            if (!Exists(configuration))
                throw new ConfigurationNotFoundException(null);

            // Get settings
            int index = mConfigurations.Values.ToList().IndexOf(configuration);
            ConfigurationSettings settings = mConfigurationsSettings.ElementAtOrDefault(index);

            // In case of null.
            if (settings == null)
                throw new InvalidOperationException("For some reason, the settings for this configuration are null");

            return settings;
        }

        /// <summary>
        /// Returns the type of a configuration
        /// </summary>
        /// <param name="configuration">The configuration to get its type</param>
        public Type FindType(IConfiguration configuration)
        {
            Validator.Validate(configuration);

            // Configuration not found
            if (!Exists(configuration))
                throw new ConfigurationNotFoundException(null);

            // Return the type
            return mConfigurationsTypes.ElementAt(mConfigurations.Values.ToList().IndexOf(configuration));
        }

        /// <summary>
        /// Returns a configuration and its settings from name
        /// </summary>
        /// <param name="name">The name of the configuration to search</param>
        public (IConfiguration configuration, ConfigurationSettings settings) FindAll(string name)
        {
            Validator.Validate(name);

            // Get configuration and settings
            IConfiguration configuration = Find(name);
            ConfigurationSettings settings = FindSettings(configuration);

            return (configuration, settings);
        }

        #endregion
    }
}