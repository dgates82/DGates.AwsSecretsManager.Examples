using System;

namespace MvcExample.Core
{
    public static class ApiKeyEvaluator
    {
        public const string PlaceholderKey = "YOUR_KEY_HERE";

        public static bool IsPlaceholder(string key)
        {
            return string.IsNullOrWhiteSpace(key)
                || string.Equals(key.Trim(), PlaceholderKey, StringComparison.OrdinalIgnoreCase);
        }
    }
}
