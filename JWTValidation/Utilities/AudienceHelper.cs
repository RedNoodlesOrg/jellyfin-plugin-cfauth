using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace JWTValidation.Utilities
{
    public static class AudienceHelper
    {
        public static ImmutableHashSet<string>? GetAudiences(string? audienceConfig)
        {
            if (string.IsNullOrEmpty(audienceConfig))
            {
                return null;
            }

            var cleanInput = Regex.Replace(
                audienceConfig,
                @"[^\w,]",
                string.Empty,
                RegexOptions.None,
                TimeSpan.FromSeconds(1.5));

            return ImmutableHashSet.Create(cleanInput.Split(",", StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
