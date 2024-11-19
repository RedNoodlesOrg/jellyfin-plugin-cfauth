namespace JWTValidation.KeyManagement.Tests
{
    [TestClass()]
    public class CloudflareKeyProviderTests
    {
        [TestMethod()]
        public async Task GetSigningKeysAsyncTest()
        {
            // Arrange
            var keyProvider = new CloudflareKeyProvider { TeamName = "test" };

            // Act
            var signingKeys = await keyProvider.GetSigningKeysAsync();

            // Assert
            Assert.IsNotNull(signingKeys, "Signing keys should not be null.");
            Assert.IsTrue(signingKeys.Count > 0, "Signing keys should contain at least one key.");
        }

        [TestMethod()]
        public void GetIssuerTest()
        {
            // Arrange
            var keyProvider = new CloudflareKeyProvider { TeamName = "test" };

            // Act
            var issuer = keyProvider.GetIssuer();

            // Assert
            Assert.AreEqual("https://test.cloudflareaccess.com", issuer, "Issuer URL is not as expected.");
        }

        [TestMethod()]
        public void GetAudiencesTest()
        {
            // Arrange
            var keyProvider = new CloudflareKeyProvider { TeamName = "test" };
            var audiencesString = "padieasdjeakdejha, aodijeaofjffeso9ij,,, ,";

            // Act
            var audiences = keyProvider.GetAudiences(audiencesString);

            // Assert
            Assert.IsNotNull(audiences, "Audiences should not be null.");
            Assert.IsTrue(audiences.Count == 2, "Audiences should have two entries.");
        }

        [TestMethod()]
        public void DisposeTest()
        {
            // Arrange
            var keyProvider = new CloudflareKeyProvider { TeamName = "test" };

            // Act
            keyProvider.Dispose();

            // Assert
            Assert.ThrowsException<ObjectDisposedException>(() => keyProvider.GetSigningKeysAsync().GetAwaiter().GetResult(), "ObjectDisposedException should be thrown after disposal.");
        }
    }
}
