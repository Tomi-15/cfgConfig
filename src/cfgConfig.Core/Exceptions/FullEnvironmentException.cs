using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the max size of configurations per environment is reached
    /// </summary>
    public class FullEnvironmentException : Exception
    {
        public FullEnvironmentException(int size) : base($"Environment size reached. You need to create a new environment an then add the configuration to it. Max configurations per environment: {size}")
        {

        }
    }
}