using cfgConfig.Core.Configuration;
using cfgConfig.Core.Validation;
using System;

namespace cfgConfig.Core.Environment
{
    /// <summary>
    /// Represents an environment. A collection of configurations
    /// </summary>
    public class ConfigurationEnvironment
    {
        #region Members

        // The collection where to store all the configurations
        private ConfigurationCollection mConfigurations;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the configuration environment
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The directory where all configurations will be stored
        /// </summary>
        public string WorkingPath { get; }

        /// <summary>
        /// An unique id for this environment
        /// </summary>
        public string Id { get; set; }

        #endregion

        #region Constructors

        internal ConfigurationEnvironment(string workingPath, string name, ConfigurationCollection configurations)
        {
            Validator.ValidateAll(workingPath, name, configurations);

            WorkingPath = workingPath;
            Name = name;
            mConfigurations = configurations;
            Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a generic configuration by its name
        /// </summary>
        /// <param name="name">The name of the configuration to search for.</param>
        public IConfiguration GetConfiguration(string name)
        {
            Validator.Validate(name);

            // Try to find the configuration
            IConfiguration configuration = mConfigurations.Find(name);

            return configuration;
        }

        /// <summary>
        /// Returns a configuration converting to a specific type
        /// </summary>
        /// <typeparam name="TConfiguration">The type of configuration to get.</typeparam>
        /// <param name="name">The name of the configuration to get</param>
        public TConfiguration GetConfiguration<TConfiguration>(string name)
            where TConfiguration : IConfiguration
        {
            Validator.Validate(name);

            // Try to get a configuration
            TConfiguration configuration = (TConfiguration)mConfigurations.Find(name, typeof(TConfiguration));

            // Return the configuration if successful
            return configuration;
        }

        /// <summary>
        /// Returns a configuration of type <see cref="KeyValuePairConfiguration"/> by its name
        /// </summary>
        /// <param name="name">The name of the configuration</param>
        public KeyValuePairConfiguration GetKeyValueConfiguration(string name)
        {
            Validator.Validate(name);

            return GetConfiguration<KeyValuePairConfiguration>(name);
        }

        #endregion
    }
}