using System.Security.Claims;
using System.Security.Cryptography;
using JWTValidation.KeyManagement;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace JWTValidation.Tests
{
    [TestClass]
    public class JwtValidatorTests
    {
        [TestMethod]
        public async Task ValidateJwtAsync_ValidToken_ReturnsTokenValidationResult()
        {
            // Arrange
            var validIssuer = "https://validissuer.cloudflareaccess.com";
            var validAudience = "audience1";
            using var rsa = RSA.Create(2048);
            var key = new RsaSecurityKey(rsa);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var publicParameters = rsa.ExportParameters(false);
            var publicKey = new RsaSecurityKey(publicParameters);

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = validAudience,
                Issuer = validIssuer,
                Claims = new Dictionary<string, object>
                {
                    { ClaimTypes.Email, "valid@email.com" }
                },
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = signingCredentials
            };

            var handler = new JsonWebTokenHandler();
            var validToken = handler.CreateToken(descriptor);

            var mockKeyProvider = new Mock<IKeyProvider>();
            mockKeyProvider.Setup(k => k.GetIssuer()).Returns(validIssuer);
            mockKeyProvider.Setup(k => k.GetAudiences(It.IsAny<string>())).Returns([validAudience]);
            mockKeyProvider.Setup(k => k.GetSigningKeysAsync()).ReturnsAsync([publicKey]);

            var jwtValidator = new JwtValidator(mockKeyProvider.Object);

            // Act
            var result = await jwtValidator.ValidateJwtAsync(validToken, validAudience);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
        }
    }
}
