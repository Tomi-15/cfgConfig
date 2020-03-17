using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when is tried to load a file as a configuration and its not one
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="InvalidConfigurationException"/> the invalid file that generated the exception
        /// </summary>
        /// <param name="file">The file that is invalid</param>
        public InvalidConfigurationException(string file) : base($"Invalid configuration file: '{file}'") { }
    }
}