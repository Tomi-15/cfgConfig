using System;

namespace cfgConfig.Core.Attributes
{
    /// <summary>
    /// Attribute that a class must has to define it as configuration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
        /// <summary>
        /// The name of the configuration
        /// </summary>
        public string Name { get; set; }
    }
}
