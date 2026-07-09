using MvcExample.Core;
using Xunit;

namespace MvcExample.Tests
{
    public class ApiKeyEvaluatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("YOUR_KEY_HERE")]
        [InlineData("your_key_here")]
        [InlineData(" YOUR_KEY_HERE ")]
        public void IsPlaceholder_ReturnsTrue_ForPlaceholderOrEmptyKeys(string key)
        {
            Assert.True(ApiKeyEvaluator.IsPlaceholder(key));
        }

        [Fact]
        public void IsPlaceholder_ReturnsFalse_ForRealLookingKey()
        {
            Assert.False(ApiKeyEvaluator.IsPlaceholder("dc6f261631c6f0a5750492a24036fd28"));
        }
    }
}
