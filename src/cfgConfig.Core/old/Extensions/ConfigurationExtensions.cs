using cfgConfig.Core.old.Exceptions;

namespace cfgConfig.Core.old.Extensions
{
    /// <summary>
    /// Extensions methods to work with configurations
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Saves an specific configuration type
        /// </summary>
        /// <typeparam name="TConfig">The configuration type to save</typeparam>
        /// <param name="config">The configuration to save</param>
        public static void SaveConfig<TConfig>(this TConfig config)
        {
            // Get manager
            var manager = ConfigurationManager.GetManagerWhoImplements<TConfig>();

            // If null, throw exception
            if (manager == null)
                throw new ConfigNotFoundException($"Configuration of type {typeof(TConfig).ToString()} not found.");

            // Save implementation
            manager.Implementations.SaveImplementation(typeof(TConfig));
        }
    }
}
