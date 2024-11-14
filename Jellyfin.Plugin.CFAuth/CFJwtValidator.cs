using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Jellyfin.Plugin.CFAuth
{
    /// <summary>
    /// Validates JWT.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CFJwtValidator"/> class.
    /// </remarks>
    public class CFJwtValidator()
    {
        private readonly TimeSpan _cacheTtl = new(1, 0, 0);
        private static readonly HttpClient _httpClient = new();
        private List<SecurityKey>? _cachedKeys;
        private DateTime _cacheExpiry = DateTime.MinValue;
        private string _cacheIssuer = string.Empty;
        private string _cacheAudience = string.Empty;

        private static string Issuer => $"https://{Plugin.Instance?.Configuration.Teamname}.cloudflareaccess.com";

        private static string KeysetUrl => $"{Issuer}/cdn-cgi/access/certs";

        private static string? Audience => Plugin.Instance?.Configuration.Audience;

        private async Task<List<SecurityKey>> FetchAndCacheKeysAsync()
        {
            if (_cachedKeys != null && DateTime.UtcNow < _cacheExpiry && _cacheIssuer.Equals(Issuer, StringComparison.Ordinal) && _cacheAudience.Equals(Audience, StringComparison.Ordinal))
            {
                return _cachedKeys;
            }

            var response = await _httpClient.GetAsync(KeysetUrl).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch keyset: {response.StatusCode}");
            }

            var publickeys = await response.Content.ReadFromJsonAsync<PublicKeys>().ConfigureAwait(false);

            if (publickeys == null)
            {
                throw new JsonException("Failed to deserialize the keyset");
            }

            var keys = new List<SecurityKey>();
            foreach (var key in publickeys.Keys)
            {
                keys.Add(ConvertToSecurityKey(key));
            }

            _cachedKeys = keys;
            _cacheExpiry = DateTime.UtcNow.Add(_cacheTtl);
            _cacheIssuer = Issuer;
            _cacheAudience = Audience ?? string.Empty;

            return keys;
        }

        private static RsaSecurityKey ConvertToSecurityKey(Key keySet)
        {
            var rsa = new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(keySet.N),
                Exponent = Base64UrlEncoder.DecodeBytes(keySet.E)
            };
            return new RsaSecurityKey(rsa) { KeyId = keySet.Kid };
        }

        /// <summary>
        /// Validates token from current request.
        /// </summary>
        /// <param name="httpContext">Context with the request.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        /// <exception cref="SecurityTokenValidationException">Invalid Token.</exception>
        /// <exception cref="HttpRequestException">Error while fetching Keyset.</exception>
        /// <exception cref="JsonException">Error while parsing Keyset.</exception>
        /// <exception cref="UriFormatException">Team Domain incorrect.</exception>
        public async Task<TokenValidationResult?> ValidateJwtAsync(HttpContext httpContext)
        {
            if (httpContext is null || !httpContext.Request.Cookies.TryGetValue("CF_Authorization", out var token))
            {
                throw new HttpRequestException("Could not get cookie");
            }

            var keys = await FetchAndCacheKeysAsync().ConfigureAwait(false);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var result = await tokenHandler.ValidateTokenAsync(token, validationParameters).ConfigureAwait(false);
            if (result == null || !result.IsValid)
            {
                throw new SecurityTokenValidationException("token is invalid");
            }

            return result;
        }

        internal sealed class Key
        {
            public required string Kid { get; set; }

            public required string Kty { get; set; }

            public required string Alg { get; set; }

            public required string Use { get; set; }

            public required string E { get; set; }

            public required string N { get; set; }
        }

        internal sealed class PublicCert
        {
            public required string Kid { get; set; }

            public required string Cert { get; set; }
        }

        internal sealed class PublicKeys
        {
            public required List<Key> Keys { get; set; }

            public required PublicCert Public_Cert { get; set; }

            public required List<PublicCert> Public_Certs { get; set; }
        }
    }
}
