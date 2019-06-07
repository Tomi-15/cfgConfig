using cfgConfig.Core.Attributes;
using cfgConfig.Core.Engine;
using cfgConfig.Core.Engine.Settings;
using cfgConfig.Core.Exceptions;
using cfgConfig.Core.Files;
using cfgConfig.Core.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cfgConfig.Core.Implementation.Base
{
    /// <summary>
    /// Reponsable class that implements configuration types
    /// </summary>
    public sealed class ConfigImplementer
    {
        #region Public Properties

        /// <summary>
        /// Gets the count of configurations implemented
        /// </summary>
        public int Count => mConfigImplementations.Count;

        #endregion

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
            implementation.File = new ConfigFile(Path.Combine(mManager.Path, $"{implementation.Name}{GlobalSettings.DEFAULT_EXTENSION}"));

            // Get object instance
            DeserializeContentFromFile<TConfig>(implementation);

            // Add to the list
            mConfigImplementations.Add(implementation);

            Logger.LogInfo($"{implementation.Name} implemented. ({mConfigImplementations.Count} implemented)");

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
                if (GlobalSettings.UseConsole)
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
            implementation.File = new ConfigFile(Path.Combine(mManager.Path, $"{implementation.Name}{GlobalSettings.DEFAULT_EXTENSION}"));

            // Get content from file
            DeserializeContentFromFile(implementation);

            // Add to the list
            mConfigImplementations.Add(implementation);

            return this;
        }

        /// <summary>
        /// Returns a boolean indicating if a configuration type is implemented
        /// </summary>
        /// <typeparam name="TConfig">The type of configuration to check</typeparam>
        public bool Has<TConfig>() => mConfigImplementations.Any(x => x.Type == typeof(TConfig));

        #region Internal

        /// <summary>
        /// Saves all types
        /// </summary>
        internal void SaveAllImplementations()
        {
            // Foreach implementation...
            foreach (var implementation in mConfigImplementations)
                SaveImplementation(implementation.Type);
        }

        /// <summary>
        /// Saves an implementation
        /// </summary>
        internal void SaveImplementation(Type type)
        {
            // If the configuration type does not exists, throw exception
            if (!mConfigImplementations.Any(x => x.Type == type))
                throw new ConfigNotFoundException($"Configuration of type {type.ToString()} does not exists.");

            // Get config
            var config = mConfigImplementations.First(x => x.Type == type);

            // If the file does not exists, create its
            if (!config.File.Exists)
                config.File.Create();

            // Get the serialized content
            string serializedContent = GetSerializedContent(config.RuntimeInstance, mManager.Settings.SaveMode);

            // Write all the content to the file
            File.WriteAllText(config.File.FullName, serializedContent);

            // If the encryptation is enabled, encrypt the file
            if (mManager.Settings.EncryptationEnabled)
                config.File.Encrypt();
        }

        /// <summary>
        /// Gets a registered implementation
        /// </summary>
        /// <typeparam name="T">The type of implementation to get</typeparam>
        internal ConfigImplementation<T> GetImplementation<T>() 
            => (ConfigImplementation<T>)mConfigImplementations.First(x => x.Type == typeof(T));

        /// <summary>
        /// Gets a registered implementation
        /// </summary>
        internal BaseConfigImplementation GetBaseImplementation(Type implementationType)
            => mConfigImplementations.First(x => x.Type == implementationType);

        /// <summary>
        /// Gets all config implementations
        /// </summary>
        internal BaseConfigImplementation[] GetConfigImplementations() => mConfigImplementations.AsReadOnly().ToArray();

        #endregion

        #endregion

        #region Private Helper Methods

        private string GetSerializedContent(object content, SaveModes mode)
        {
            // String to store the serialized content
            string serialized = "";

            // Get save mode
            switch (mode)
            {
                // If its json..
                case SaveModes.Json:
                    serialized = JsonConvert.SerializeObject(content, Formatting.Indented);
                    break;

                // If its xml..
                case SaveModes.Xml:
                    serialized = XmlSerializer.Serialize(content);
                    break;

                // If its binary..
                case SaveModes.Binary:
                    serialized = BinarySerializer.Serialize(content);
                    break;
            }

            return serialized;
        }

        private void DeserializeContentFromFile<T>(BaseConfigImplementation implementation)
        {
            T deserialized = default;

            // If the file exists and contains data...
            if(implementation.File.Exists && File.ReadAllText(implementation.File.FullName).Length > 0)
            {
                // If the encryptation is enabled, decrypt the file
                if(mManager.Settings.EncryptationEnabled)
                    implementation.File.Decrypt();

                // Get serializer mode
                var saveMode = mManager.Settings.SaveMode;

                switch (saveMode)
                {
                    case SaveModes.Json:
                        deserialized = JsonConvert.DeserializeObject<T>(File.ReadAllText(implementation.File.FullName));
                        break;

                    case SaveModes.Xml:
                        deserialized = XmlSerializer.Deserialize<T>(File.ReadAllText(implementation.File.FullName));
                        break;

                    case SaveModes.Binary:
                        deserialized = BinarySerializer.Deserialize<T>(File.ReadAllText(implementation.File.FullName));
                        break;
                }

                implementation.RuntimeInstance = deserialized;
            }
            else
            {
                // Create an instance
                implementation.RuntimeInstance = Activator.CreateInstance<T>();

                // Create the file
                if(!File.Exists(implementation.File.FullName))
                    using (var fs = File.Create(implementation.File.FullName)) { }
            }
        }

        private void DeserializeContentFromFile(BaseConfigImplementation implementation)
        {
            object deserialized = default;

            // If the file exists and contains data...
            if (implementation.File.Exists && File.ReadAllText(implementation.File.FullName).Length > 0)
            {
                // Get serializer mode
                var saveMode = mManager.Settings.SaveMode;

                // Decrypt the file if encryptation is enabled
                if (mManager.Settings.EncryptationEnabled)
                    implementation.File.Decrypt();

                switch (saveMode)
                {
                    case SaveModes.Json:
                        deserialized = JsonConvert.DeserializeObject(File.ReadAllText(implementation.File.FullName));
                        break;

                    case SaveModes.Xml:
                        deserialized = XmlSerializer.Deserialize(File.ReadAllText(implementation.File.FullName), implementation.Type);
                        break;

                    case SaveModes.Binary:
                        deserialized = BinarySerializer.Deserialize(File.ReadAllText(implementation.File.FullName));
                        break;
                }

                implementation.RuntimeInstance = deserialized;
            }
            else
            {
                implementation.RuntimeInstance = Activator.CreateInstance(implementation.Type);

                // Create the file
                if (!File.Exists(implementation.File.FullName))
                    using (var fs = File.Create(implementation.File.FullName)) { }
            }
        }

        #endregion
    }
}
