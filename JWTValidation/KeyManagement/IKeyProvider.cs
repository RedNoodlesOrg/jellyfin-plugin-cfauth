using System.Collections.Immutable;
using Microsoft.IdentityModel.Tokens;

namespace JWTValidation.KeyManagement
{
    public interface IKeyProvider
    {
        ImmutableHashSet<string>? GetAudiences(string audiences);
        string GetIssuer();
        Task<List<SecurityKey>> GetSigningKeysAsync();
    }
}
