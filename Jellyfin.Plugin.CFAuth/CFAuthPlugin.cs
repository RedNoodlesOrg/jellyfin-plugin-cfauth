using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.CFAuth.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.CFAuth;

/// <summary>
/// The main plugin.
/// </summary>
public class CFAuthPlugin : BasePlugin<CFAuthPluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CFAuthPlugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    public CFAuthPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "CFAuth";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("09edc1d4-b95a-464b-8059-ae6b5f5daa93");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static CFAuthPlugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        ];
    }
}
