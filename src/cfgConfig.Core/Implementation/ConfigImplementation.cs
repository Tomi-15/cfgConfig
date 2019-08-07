using cfgConfig.Core.Files;
using System;

namespace cfgConfig.Core.Implementation
{
    /// <summary>
    /// The base class for each config implementation
    /// </summary>
    internal abstract class BaseConfigImplementation
    {
        #region Public Properties

        /// <summary>
        /// Implementation's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Implementation's runtime instance
        /// </summary>
        public object RuntimeInstance { get; set; }

        /// <summary>
        /// Implementation's type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Implementation's config file
        /// </summary>
        public ConfigFile File { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseConfigImplementation()
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseConfigImplementation(Type type)
        {
            Type = type;
        }

        #endregion
    }

    /// <summary>
    /// Implementation of the <see cref="BaseConfigImplementation"/> class
    /// </summary>
    internal class ConfigImplementation : BaseConfigImplementation
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConfigImplementation(Type type) : base(type) { }
    }

    /// <summary>
    /// Implementation of the <see cref="BaseConfigImplementation"/> with generic implementation
    /// </summary>
    internal class ConfigImplementation<TRuntime> : BaseConfigImplementation
    {
        /// <summary>
        /// The generic runtime instance
        /// </summary>
        public new TRuntime RuntimeInstance
        {
            get => (TRuntime)base.RuntimeInstance;
            set => base.RuntimeInstance = value;
        }

        public ConfigImplementation(Type type) : base(type) { }

        public ConfigImplementation(BaseConfigImplementation implementation) : base()
        {
            RuntimeInstance = (TRuntime)implementation.RuntimeInstance;
            Name = implementation.Name;
            Type = implementation.Type;
            File = implementation.File;
        }

        public ConfigImplementation() : base() { }
    }
}
