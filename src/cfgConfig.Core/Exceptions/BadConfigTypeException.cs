using System;

namespace cfgConfig.Core.Exceptions
{
    /// <summary>
    /// Thrown when a class that is not marked as Config is tried to be
    /// implemented as one of it
    /// </summary>
    public class BadConfigTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadConfigTypeException"/> exception with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        public BadConfigTypeException(string message) : base(message) { }
    }
}
