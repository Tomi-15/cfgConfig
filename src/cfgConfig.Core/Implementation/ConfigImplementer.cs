using cfgConfig.Core.Attributes;
using cfgConfig.Core.Engine;
using cfgConfig.Core.Engine.Settings;
using cfgConfig.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cfgConfig.Core.Implementation.Base
{
    /// <summary>
    /// Reponsable class that implements configuration types
    /// </summary>
    public sealed class ConfigImplementer
    {
        #region Private Members

        private readonly List<BaseConfigImplementation> mConfigImplementations;
        private ConfigurationManager mManager; // The manager that is calling this implementer

        #endregion

        #region Constructors

        internal ConfigImplementer(ConfigurationManager manager)
        {
            mConfigImplementations = new List<BaseConfigImplementation>();
            mManager = manager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implements a configuration type
        /// </summary>
        /// <typeparam name="TConfig">The type of config to implement</typeparam>
        public ConfigImplementer Implement<TConfig>()
        {
            Type configType = typeof(TConfig); // Get config type
            var configAttribute = (ConfigAttribute)configType.GetCustomAttributes(typeof(ConfigAttribute), true).FirstOrDefault(); // Get ConfigAttribute from the class
            var implementation = new ConfigImplementation<TConfig>();

            // If the class is not marked as config, throw exception
            if (configAttribute == null)
            {
                if (GlobalSettings.UseConsole)
                {
                    Logger.LogError($"The type {configType.ToString()} is not a valid Config type.");
                    return this;
                }
                else
                    throw new BadConfigTypeException($"The type {configType.ToString()} is not a valid Config type.");
            }

            // If the type is already implemented, throw exception
            if (mConfigImplementations.Any(x => x.Type == configType))
            {
                if(GlobalSettings.UseConsole)
                {
                    Logger.LogError($"The configuration type {configType.ToString()} is already implemented.");
                    return this;
                }
                else
                    throw new DuplicatedConfigException($"The configuration type {configType.ToString()} is already implemented.");
            }

            // Configure the implementation
            implementation.Name = configAttribute.Name != null ? configAttribute.Name : configType.Name; // Set the name
            implementation.Type = configType; // Set type
            implementation.RuntimeInstance = Activator.CreateInstance<TConfig>(); // Create a new runtime instance

            // Add to the list
            mConfigImplementations.Add(implementation);

            Logger.Log($"{implementation.Name} implemented. ({mConfigImplementations.Count} implemented)");

            return this;
        }

        /// <summary>
        /// Implements a configuration type
        /// </summary>
        /// <param name="configType">The type of config to implement</param>
        public ConfigImplementer Implement(Type configType)
        {
            var configAttribute = (ConfigAttribute)configType.GetCustomAttributes(typeof(ConfigAttribute), true).FirstOrDefault(); // Get ConfigAttribute from the class
            var implementation = new ConfigImplementation(configType);

            // If the class is not marked as config, throw exception
            if (configAttribute == null) throw new BadConfigTypeException($"The type {configType.ToString()} is not a valid Config type.");

            // If the type is already implemented, throw exception
            if (mConfigImplementations.Any(x => x.Type == configType)) throw new DuplicatedConfigException($"The configuration type {configType.ToString()} is already implemented.");

            // Configure the implementation
            implementation.Name = configAttribute.Name != null ? configAttribute.Name : configType.Name; // Set the name
            implementation.Type = configType; // Set type
            implementation.RuntimeInstance = Activator.CreateInstance(configType); // Create a new runtime instance

            // Add to the list
            mConfigImplementations.Add(implementation);

            return this;
        }

        /// <summary>
        /// Gets all config implementations
        /// </summary>
        internal BaseConfigImplementation[] GetConfigImplementations()
            => mConfigImplementations.AsReadOnly().ToArray();

        #endregion

        #region Private Helper Methods

        

        #endregion
    }
}
