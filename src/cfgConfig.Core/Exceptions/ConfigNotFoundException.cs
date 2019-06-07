using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a config type is requested but it isn't implemented
    /// </summary>
    public class ConfigNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ConfigNotFoundException"/> with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        public ConfigNotFoundException(string message) : base(message) { }
    }
}
