using MvcExample.Core;
using Xunit;

namespace MvcExample.Tests
{
    public class WeatherApiUrlBuilderTests
    {
        [Fact]
        public void BuildGeocodeUrl_EscapesCityNameAndIncludesKey()
        {
            var url = WeatherApiUrlBuilder.BuildGeocodeUrl("abc123", "San Diego");

            Assert.Equal(
                "http://api.openweathermap.org/geo/1.0/direct?q=San%20Diego&limit=1&appid=abc123",
                url);
        }

        [Fact]
        public void BuildWeatherUrl_UsesSecretUrlAndImperialUnits()
        {
            var secret = new OpenWeatherMapSecret
            {
                Url = "https://api.openweathermap.org/data/2.5/weather",
                Key = "abc123"
            };

            var url = WeatherApiUrlBuilder.BuildWeatherUrl(secret, 32.7157, -117.1611);

            Assert.Equal(
                "https://api.openweathermap.org/data/2.5/weather?lat=32.7157&lon=-117.1611&appid=abc123&units=imperial",
                url);
        }
    }
}
