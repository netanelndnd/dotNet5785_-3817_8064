using DalApi;

namespace Dal;

/// <summary>
/// Implementation of the IConfig interface to manage configuration settings.
/// </summary>
public class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the current clock time.
    /// </summary>
    public DateTime Clock { get => Config.Clock; set => Config.Clock = value; }

    /// <summary>
    /// Gets or sets the risk range time span.
    /// </summary>
    public TimeSpan RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    /// <summary>
    /// Resets the configuration settings to their default values.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}
