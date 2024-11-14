using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CFAuth.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Teamname = "example.com";
        Audience = "<your-aud-tag>";
    }

    /// <summary>
    /// Gets or sets a string setting.
    /// </summary>
    public string Teamname { get; set; }

    /// <summary>
    /// Gets or sets a string setting.
    /// </summary>
    public string Audience { get; set; }
}
