using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using Jellyfin.Plugin.CFAuth.Configuration;
using JWTValidation;
using JWTValidation.KeyManagement;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using Microsoft.AspNetCore.Http;

namespace Jellyfin.Plugin.CFAuth
{
    /// <summary>
    /// JWT Auth CFAuthPlugin.
    /// </summary>
    public partial class CFAuthenticationProviderPlugin : IAuthenticationProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserManager _userManager;
        private JwtValidator? _jwtValidator;
        private IKeyProvider? _keyProvider;
        private string? _audiences;
        private string? _cookieName;
        private string? _headerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CFAuthenticationProviderPlugin"/> class.
        /// </summary>
        /// <param name="contextAccessor">The HttpContext.</param>
        /// <param name="userManager">The UserManager.</param>
        public CFAuthenticationProviderPlugin(IHttpContextAccessor contextAccessor, IUserManager userManager)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        /// <inheritdoc/>
        public string Name => "JWT-Authentication";

        /// <inheritdoc/>
        public bool IsEnabled => true;

        [GeneratedRegex(@"[\w@\.]+")]
        private static partial Regex ValidCharsRegex();

        [MemberNotNullWhen(true, ["_keyProvider", "_jwtValidator", "_audiences", "_cookieName", "_headerName"])]
        private bool Init()
        {
            if (CFAuthPlugin.Instance is null)
            {
                return false;
            }

            _keyProvider = new CloudflareKeyProvider { TeamName = CFAuthPlugin.Instance.Configuration.Teamname };
            _jwtValidator = new JwtValidator(_keyProvider);
            _audiences = CFAuthPlugin.Instance.Configuration.Audience;
            _cookieName = CFAuthPlugin.Instance.Configuration.CookieName;
            _headerName = CFAuthPlugin.Instance.Configuration.HeaderName;

            CFAuthPlugin.Instance.ConfigurationChanged += ConfigurationChangedHandler;

            return true;
        }

        private void ConfigurationChangedHandler(object? sender, BasePluginConfiguration e)
        {
            if (_keyProvider is null)
            {
                return;
            }

            ((CloudflareKeyProvider)_keyProvider).TeamName = ((CFAuthPluginConfiguration)e).Teamname;
            _audiences = ((CFAuthPluginConfiguration)e).Audience;
            _cookieName = ((CFAuthPluginConfiguration)e).CookieName;
            _headerName = ((CFAuthPluginConfiguration)e).HeaderName;
        }

        private static AuthenticationException GenericError()
        {
            return new AuthenticationException("JWT is null or not valid");
        }

        private string FetchToken()
        {
            if (_cookieName is null || _headerName is null)
            {
                throw GenericError();
            }

            var httpContext = _contextAccessor.HttpContext ?? throw GenericError();
            if (!(httpContext.Request.Cookies.TryGetValue(_cookieName, out var tokenCookie) | httpContext.Request.Headers.TryGetValue(_headerName, out var tokenHeader)))
            {
                throw GenericError();
            }

            var token = (tokenCookie is not null ? tokenCookie : (tokenHeader.Count == 1) ? tokenHeader.FirstOrDefault() : throw GenericError()) ?? throw GenericError();
            return token;
        }

        /// <inheritdoc/>
        public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        {
            if (_jwtValidator is null || _audiences is null)
            {
                if (!Init())
                {
                    throw new InvalidOperationException("Init failed.");
                }
            }

            var result = await _jwtValidator.ValidateJwtAsync(FetchToken(), _audiences).ConfigureAwait(false);
            if (result is null || !result.IsValid)
            {
                throw GenericError();
            }

            string email = (string)result.Claims.Where(claim =>
            {
                return claim.Key.Equals(ClaimTypes.Email, StringComparison.Ordinal);
            }).FirstOrDefault().Value ?? throw GenericError();

            email = ValidCharsRegex().Replace(email, string.Empty);

            if (_userManager.GetUserByName(email) is null)
            {
                var user = await _userManager.CreateUserAsync(email).ConfigureAwait(false);
                user.EnableLocalPassword = false;
            }

            return new ProviderAuthenticationResult
            {
                Username = email
            };
        }

        /// <inheritdoc/>
        public Task ChangePassword(User user, string newPassword)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool HasPassword(User user)
        {
            return false;
        }
    }
}
