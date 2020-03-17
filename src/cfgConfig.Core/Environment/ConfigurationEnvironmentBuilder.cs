using cfgConfig.Core.Configuration;
using cfgConfig.Core.Exceptions;
using cfgConfig.Core.File;
using cfgConfig.Core.Validation;
using System;
using System.IO;

namespace cfgConfig.Core.Environment
{
    /// <summary>
    /// Provides methods to help building configuration environments
    /// </summary>
    public class ConfigurationEnvironmentBuilder
    {
        #region Members

        private string mName; // The name of the environment
        private string mPath; // The path of the environment
        private ConfigurationCollection mConfigurationCollection;

        #endregion

        #region Constructors

        internal ConfigurationEnvironmentBuilder(string name, string workingPath)
        {
            Validator.Validate(workingPath);

            // Create the environment builder
            mPath = workingPath;
            mConfigurationCollection = new ConfigurationCollection();

            if (string.IsNullOrWhiteSpace(name))
                mName = Path.GetDirectoryName(workingPath);
            else
                mName = name;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a configuration to the environment
        /// </summary>
        /// <typeparam name="TConfiguration">The type of configuration to add</typeparam>
        /// <param name="name">The name of the configuration</param>
        public ConfigurationEnvironmentBuilder WithConfiguration<TConfiguration>(string name)
            where TConfiguration : IConfiguration, new()
        {
            Validator.Validate(name);

            // Get a new instance of the configuration type
            IConfiguration instance = new TConfiguration();

            // Add to the collection
            mConfigurationCollection.Add(instance, name, null);

            // Not found directory
            if (!Directory.Exists(mPath))
                throw new DirectoryNotFoundException($"Directory '{mPath}' not found.");

            // Create the file
            ConfigurationFile file = new ConfigurationFile(Path.Combine(mPath, $"{name}{ConfigurationManager.DEFAULT_EXTENSION}"));
            file.Create();
            file.WriteHeader();

            return this;
        }

        /// <summary>
        /// Adds a configuration to the environment and specifies special settings
        /// </summary>
        /// <typeparam name="TConfiguration">The type of configuration to add</typeparam>
        /// <param name="name">The name of the configuration</param>
        /// <param name="configure">Used to configure the configuration type</param>
        public ConfigurationEnvironmentBuilder WithConfiguration<TConfiguration>(string name, Func<ConfigurationBuilder, ConfigurationSettings> configure)
            where TConfiguration : IConfiguration, new()
        {
            Validator.Validate(name);
            Validator.Validate(configure);

            // Duplicated configuration
            if(mConfigurationCollection.Exists(name))
                throw new DuplicatedConfigurationException(name);

            // Get a new instance of the configuration
            TConfiguration configuration = new TConfiguration();

            // Get settings
            ConfigurationBuilder builder = new ConfigurationBuilder();
            ConfigurationSettings settings = configure.Invoke(builder);

            // Add the configuration
            mConfigurationCollection.Add(configuration, name, settings);

            return this;
        }

        /// <summary>
        /// Builds the configuration environment
        /// </summary>
        public ConfigurationEnvironment Build()
        {
            // Create the environment
            ConfigurationEnvironment environment = new ConfigurationEnvironment(mPath, mName, mConfigurationCollection);

            // Add the environment to static list
            ConfigurationManager.Environments.Add(environment);

            return environment;
        }

        #endregion
    }
}