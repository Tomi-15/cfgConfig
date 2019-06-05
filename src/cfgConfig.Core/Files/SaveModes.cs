namespace cfgConfig.Core.Files
{
    /// <summary>
    /// The available save modes for configuration files
    /// </summary>
    public enum SaveModes
    {
        /// <summary>
        /// The configuration type will be serialized in Json and then, saved
        /// </summary>
        Json,

        /// <summary>
        /// The configuration type will be serialized in XML and then, saved
        /// </summary>
        Xml,

        /// <summary>
        /// The configuration type will be binary serialized and then, saved
        /// </summary>
        Binary
    }
}
