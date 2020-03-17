using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when is tried to add a configuration with the same name
    /// </summary>
    public class DuplicatedConfigurationException : Exception
    {
        public DuplicatedConfigurationException(string configurationName) : base($"Already exists a configuration with the name '{configurationName}'") { }
    }
}