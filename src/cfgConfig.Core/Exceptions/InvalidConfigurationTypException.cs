using System;
using cfgConfig.Core.Configuration;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a <see cref="IConfiguration"/> does not matches with its <see cref="Type"/>
    /// </summary>
    public class InvalidConfigurationTypException : Exception
    {
        public InvalidConfigurationTypException(string configuration) : base($"Invalid type of configuration for '{configuration}'.") { }
    }
}