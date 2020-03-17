namespace cfgConfig.Core.Configuration
{
    /// <summary>
    /// Contains methods and properties to configure a configuration type
    /// </summary>
    public class ConfigurationSettings
    {
        #region Properties

        /// <summary>
        /// Defines if the configuration file will be encrypted or not
        /// </summary>
        public bool Encrypt { get; internal set; }

        #endregion

        #region Constructors

        internal ConfigurationSettings() { }

        #endregion

        #region Internal Methods

        internal void MakeEncryptable()
        {
            Encrypt = true;
        }

        #endregion
    }
}