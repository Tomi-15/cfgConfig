using System;

namespace cfgConfig.Core.old.Exceptions
{
    /// <summary>
    /// Thrown when a configuration type is tried to be added when
    /// its already implemented
    /// </summary>
    public class DuplicatedConfigException : Exception
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="DuplicatedConfigException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        public DuplicatedConfigException(string message) : base(message) { }
    }
}
