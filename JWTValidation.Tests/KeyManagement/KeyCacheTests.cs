using Microsoft.VisualStudio.TestTools.UnitTesting;
using JWTValidation.KeyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;

namespace JWTValidation.KeyManagement.Tests
{
    [TestClass()]
    public class KeyCacheTests
    {
        [TestMethod()]
        public void TryGetCachedKeysTest()
        {
            // Arrange
            var cacheTtl = TimeSpan.FromMinutes(10);
            var keyCache = new KeyCache(cacheTtl);
            var issuer = "https://example.com";
            var audiences = ImmutableHashSet.Create("audience1", "audience2");
            var securityKeys = new List<SecurityKey> { new SymmetricSecurityKey(Guid.NewGuid().ToByteArray()) };

            // Act
            keyCache.UpdateCache(securityKeys, issuer, audiences);
            var result = keyCache.TryGetCachedKeys(issuer, audiences, out var cachedKeys);

            // Assert
            Assert.IsTrue(result, "Cache should return true when keys are available and valid.");
            Assert.IsNotNull(cachedKeys, "Cached keys should not be null.");
            Assert.AreEqual(securityKeys.Count, cachedKeys?.Count, "Cached keys count should match the updated keys count.");
        }

        [TestMethod()]
        public void UpdateCacheTest()
        {
            // Arrange
            var cacheTtl = TimeSpan.FromMinutes(10);
            var keyCache = new KeyCache(cacheTtl);
            var issuer = "https://example.com";
            var audiences = ImmutableHashSet.Create("audience1", "audience2");
            var securityKeys = new List<SecurityKey> { new SymmetricSecurityKey(Guid.NewGuid().ToByteArray()) };

            // Act
            keyCache.UpdateCache(securityKeys, issuer, audiences);
            var result = keyCache.TryGetCachedKeys(issuer, audiences, out var cachedKeys);

            // Assert
            Assert.IsTrue(result, "Cache should be updated and return true for valid keys.");
            Assert.IsNotNull(cachedKeys, "Cached keys should not be null after update.");
            Assert.AreEqual(securityKeys.Count, cachedKeys?.Count, "Cached keys count should match the updated keys count.");
        }
    }
}
