using cfgConfig.Core.Environment;
using cfgConfig.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cfgConfig.Core
{
    /// <summary>
    /// Startup class used to create configuration environments
    /// </summary>
    public static class ConfigurationManager
    {
        #region Constants

        internal const string DEFAULT_EXTENSION = ".bcfg";

        #endregion

        #region Members

        /// <summary>
        /// A list that contains all the created environments
        /// </summary>
        internal static List<ConfigurationEnvironment> Environments => new List<ConfigurationEnvironment>();

        #endregion

        #region Static Methods

        /// <summary>
        /// Starts the creation of a new configuration environment
        /// </summary>
        /// <param name="folder">The folder where all the configurations file for this manager will be saved.</param>
        /// <param name="name">An optional name for the environment. If null, the name of the folder will be used.</param>
        public static ConfigurationEnvironmentBuilder NewEnvironment(string folder, string name = null)
        {
            Validator.Validate(folder);

            return new ConfigurationEnvironmentBuilder(name, folder);
        }

        /// <summary>
        /// Returns a <see cref="ConfigurationEnvironment"/> by its name
        /// </summary>
        /// <param name="name">The name of the configuration environment to get</param>
        public static ConfigurationEnvironment GetEnvironment(string name)
        {
            Validator.Validate(name);

            // Try to get an environment
            ConfigurationEnvironment environment = Environments.FirstOrDefault(x => x.Name == name);

            // Environment not found
            if (environment == null)
                throw new InvalidOperationException($"Environment with name '{name}' not found.");

            return environment;
        }

        /// <summary>
        /// Returns all the environments created
        /// </summary>
        public static ConfigurationEnvironment[] GetAllEnvironments() => Environments.AsReadOnly().ToArray();

        /// <summary>
        /// Saves all the configurations of all environments
        /// </summary>
        public static async Task SaveAll()
        {
            await Task.Run(async () =>
            {

            });
        }

        #endregion
    }
}