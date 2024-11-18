using System.Collections.Immutable;
using System.Net.Http.Json;
using JWTValidation.Models;
using JWTValidation.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace JWTValidation.KeyManagement
{
    public class CloudflareKeyProvider() : IKeyProvider, IDisposable
    {
        private readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly KeyCache _keyCache = new(TimeSpan.FromHours(1));
        private bool _disposed;

        public string? TeamName { get; set; }

        public async Task<List<SecurityKey>> GetSigningKeysAsync()
        {
            var issuer = GetIssuer();
            var audiences = GetAudiences(issuer);

            if (_keyCache.TryGetCachedKeys(issuer, audiences, out var cachedKeys))
            {
                return cachedKeys!;
            }

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                // Double-check cache after acquiring the lock
                if (_keyCache.TryGetCachedKeys(issuer, audiences, out cachedKeys))
                {
                    return cachedKeys!;
                }

                // Fetch new keys from Cloudflare
                var keySet = await _httpClient.GetFromJsonAsync<KeySet>(GetKeysetUrl())
                                   .ConfigureAwait(false) ?? throw new SecurityTokenValidationException("Failed to fetch keys");
                var keys = keySet.Keys.ConvertAll<SecurityKey>((key) => { return key; });
                _keyCache.UpdateCache(keys, issuer, audiences);

                return keys;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public string GetIssuer()
        {
            return $"https://{TeamName}.cloudflareaccess.com";
        }

        private string GetKeysetUrl()
        {
            return $"{GetIssuer()}/cdn-cgi/access/certs";
        }

        public ImmutableHashSet<string>? GetAudiences(string audiences)
        {
            return AudienceHelper.GetAudiences(audiences);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                    _semaphore.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
