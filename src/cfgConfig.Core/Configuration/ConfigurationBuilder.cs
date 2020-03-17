namespace cfgConfig.Core.Configuration
{
    /// <summary>
    /// Provides methods and properties to configure a configuration
    /// </summary>
    public class ConfigurationBuilder
    {
        #region Members

        private ConfigurationSettings mSettings;

        #endregion

        #region Constructor

        internal ConfigurationBuilder()
        {
            mSettings = new ConfigurationSettings();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Indicates that the configuration must be encrypted after saved.
        /// </summary>
        public ConfigurationBuilder Encrypt()
        {
            mSettings.MakeEncryptable();

            return this;
        }

        /// <summary>
        /// Builds the settings
        /// </summary>
        public ConfigurationSettings Build()
            => mSettings;

        #endregion
    }
}