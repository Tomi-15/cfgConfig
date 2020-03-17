namespace cfgConfig.Core.Configuration
{
    /// <summary>
    /// Represents a base configuration object
    /// </summary>
    public interface IConfiguration
    {
        object Get(object key);
    }
}