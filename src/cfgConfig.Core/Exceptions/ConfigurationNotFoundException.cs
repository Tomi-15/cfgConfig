using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a configuration is not found
    /// </summary>
    public class ConfigurationNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ConfigurationNotFoundException"/>
        /// </summary>
        /// <param name="name">The name of the not found configuration</param>
        public ConfigurationNotFoundException(string name) : base($"Configuration '{name}' not found.") { }
    }
}