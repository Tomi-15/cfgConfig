using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when something related to the configuration happens.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ConfigurationException"/> indicating an error message
        /// </summary>
        /// <param name="message">The error message</param>
        public ConfigurationException(string message) : base(message) { }
    }
}