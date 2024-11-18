using JWTValidation.KeyManagement;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JWTValidation
{
    public partial class JwtValidator(IKeyProvider keyProvider)
    {
        private readonly IKeyProvider _keyProvider = keyProvider;

        /// <summary>
        /// Validates JWT.
        /// </summary>
        /// <param name="token">Token to validate.</param>
        /// <param name="audiences">Valid Audiences.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<TokenValidationResult?> ValidateJwtAsync(string token, string audiences)
        {
            var keys = await _keyProvider.GetSigningKeysAsync().ConfigureAwait(false);
            var tokenHandler = new JsonWebTokenHandler();

            return await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _keyProvider.GetIssuer(),
                ValidateAudience = true,
                ValidAudiences = _keyProvider.GetAudiences(audiences),
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = keys,
                ClockSkew = TimeSpan.FromMinutes(2)
            }).ConfigureAwait(false);

            throw new SecurityTokenValidationException("Token not found");
        }
    }
}
