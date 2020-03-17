using System.Collections.Generic;

namespace cfgConfig.Core.Configuration
{
    /// <summary>
    /// Represents a configuration that stores values using a key-value pair sintax
    /// </summary>
    public class KeyValuePairConfiguration : IConfiguration
    {
        #region Members

        private Dictionary<string, string> mEntries;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public KeyValuePairConfiguration()
        {
            mEntries = new Dictionary<string, string>();
        }

        #endregion

        /// <summary>
        /// Returns this instance
        /// </summary>
        public object Get(object key)
        {
            return key;
        }
    }
}