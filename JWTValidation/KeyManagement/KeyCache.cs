using System.Collections.Immutable;
using Microsoft.IdentityModel.Tokens;

namespace JWTValidation.KeyManagement
{
    public class KeyCache(TimeSpan cacheTtl)
    {
        private readonly TimeSpan _cacheTtl = cacheTtl;

        private List<SecurityKey>? _cachedKeys;
        private DateTime _cacheExpiry = DateTime.MinValue;
        private string _cachedIssuer = string.Empty;
        private ImmutableHashSet<string>? _cachedAudiences;

        public bool TryGetCachedKeys(string issuer, ImmutableHashSet<string>? audiences, out List<SecurityKey>? keys)
        {
            if (IsCacheValid(issuer, audiences))
            {
                keys = _cachedKeys;
                return true;
            }

            keys = null;
            return false;
        }

        public void UpdateCache(List<SecurityKey> keys, string issuer, ImmutableHashSet<string>? audiences)
        {
            _cachedKeys = keys;
            _cachedIssuer = issuer;
            _cachedAudiences = audiences;
            _cacheExpiry = DateTime.UtcNow.Add(_cacheTtl);
        }

        private bool IsCacheValid(string issuer, ImmutableHashSet<string>? audiences)
        {
            return _cachedKeys != null &&
                   DateTime.UtcNow < _cacheExpiry &&
                   _cachedIssuer.Equals(issuer, StringComparison.Ordinal) &&
                   SetEqualsNotNull(_cachedAudiences, audiences);
        }

        private static bool SetEqualsNotNull(ImmutableHashSet<string>? set1, ImmutableHashSet<string>? set2)
        {
            return set1 != null && set2 != null && set1.SetEquals(set2);
        }
    }
}
