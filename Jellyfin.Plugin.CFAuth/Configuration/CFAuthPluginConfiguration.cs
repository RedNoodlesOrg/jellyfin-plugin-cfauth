using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CFAuth.Configuration;

/// <summary>
/// CFAuthPlugin configuration.
/// </summary>
public class CFAuthPluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CFAuthPluginConfiguration"/> class.
    /// </summary>
    public CFAuthPluginConfiguration()
    {
        Teamname = "example";
        Audience = "<your-aud-tag>";
        CookieName = "CF_Authorization";
        HeaderName = "Cf-Access-Jwt-Assertion";
    }

    /// <summary>
    /// Gets or sets the Teamname.
    /// </summary>
    public string Teamname { get; set; }

    /// <summary>
    /// Gets or sets the Audience(s).
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Gets or sets the CookieName.
    /// </summary>
    public string CookieName { get; set; }

    /// <summary>
    /// Gets or sets the HeaderName.
    /// </summary>
    public string HeaderName { get; set; }
}
