using MvcExample.Core;
using Xunit;

namespace MvcExample.Tests
{
    public class WeatherEmojiMapperTests
    {
        [Theory]
        [InlineData("clear sky", "☀️")]
        [InlineData("few clouds", "☁️")]
        [InlineData("light rain", "🌧️")]
        [InlineData("thunderstorm", "⛈️")]
        [InlineData("snow", "❄️")]
        [InlineData("mist", "🌫️")]
        public void GetEmoji_MapsKnownDescriptions(string description, string expectedEmoji)
        {
            Assert.Equal(expectedEmoji, WeatherEmojiMapper.GetEmoji(description));
        }

        [Fact]
        public void GetEmoji_ReturnsEmptyString_ForNullOrWhitespace()
        {
            Assert.Equal(string.Empty, WeatherEmojiMapper.GetEmoji(null));
            Assert.Equal(string.Empty, WeatherEmojiMapper.GetEmoji("   "));
        }

        [Fact]
        public void GetEmoji_ReturnsFallback_ForUnknownDescription()
        {
            Assert.Equal("🌡️", WeatherEmojiMapper.GetEmoji("volcanic ash"));
        }
    }
}
