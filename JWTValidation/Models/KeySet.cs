using Microsoft.IdentityModel.Tokens;

namespace JWTValidation.Models
{
    internal sealed class KeySet
    {
        public required List<JsonWebKey> Keys { get; set; }
    }
}
