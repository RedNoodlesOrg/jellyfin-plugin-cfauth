using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using MediaBrowser.Controller.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Jellyfin.Plugin.CFAuth
{
    /// <summary>
    /// JWT Auth Plugin.
    /// </summary>
    public class CFAuthenticationProviderPlugin : IAuthenticationProvider
    {
        private readonly CFJwtValidator _jwtValidator;
        private readonly ILogger<CFAuthenticationProviderPlugin> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CFAuthenticationProviderPlugin"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="contextAccessor">Context.</param>
        public CFAuthenticationProviderPlugin(ILogger<CFAuthenticationProviderPlugin> logger, IHttpContextAccessor contextAccessor)
        {
            _jwtValidator = new();
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        /// <inheritdoc/>
        public string Name => "JWT-Authentication";

        /// <inheritdoc/>
        public bool IsEnabled => true;

        /// <inheritdoc/>
        public async Task<ProviderAuthenticationResult> Authenticate(string username, string password)
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new AuthenticationException("HttpContext is null");
            }

            TokenValidationResult? result = null;
            try
            {
                result = await _jwtValidator.ValidateJwtAsync(httpContext).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning("Error while parsing keys: {}", [ex.Message]);
            }
            catch (UriFormatException ex)
            {
                _logger.LogWarning("Failed to fetch keyset: {}", [ex.Message]);
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning("Failed to validate token: {}", [ex.Message]);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning("Failed to fetch keyset: {}", [ex.Message]);
            }

            if (result == null || !result.IsValid)
            {
                throw new AuthenticationException("JWT is null or not valid");
            }

            var email = (string)result.Claims.Where(claim =>
            {
                return claim.Key.Equals(ClaimTypes.Email, StringComparison.Ordinal);
            }).FirstOrDefault().Value ?? throw new AuthenticationException("JWT is null or not valid");

            if (email == null)
            {
                throw new AuthenticationException("User does not exist");
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
