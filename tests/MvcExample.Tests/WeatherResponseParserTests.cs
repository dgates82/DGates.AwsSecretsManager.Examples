using MvcExample.Core;
using Xunit;

namespace MvcExample.Tests
{
    public class WeatherResponseParserTests
    {
        [Fact]
        public void ParseGeocode_ReturnsResults_ForNonEmptyResponse()
        {
            const string json = @"
            [
                {
                    ""name"": ""San Diego"",
                    ""lat"": 32.7157,
                    ""lon"": -117.1611,
                    ""country"": ""US"",
                    ""state"": ""California""
                }
            ]";

            var results = WeatherResponseParser.ParseGeocode(json);

            var result = Assert.Single(results);
            Assert.Equal("San Diego", result.Name);
            Assert.Equal("US", result.Country);
            Assert.Equal(32.7157, result.Lat);
            Assert.Equal(-117.1611, result.Lon);
        }

        [Fact]
        public void ParseGeocode_ReturnsEmptyArray_ForNoMatches()
        {
            var results = WeatherResponseParser.ParseGeocode("[]");

            Assert.Empty(results);
        }

        [Fact]
        public void ParseWeather_ExtractsAllFields()
        {
            const string json = @"
            {
                ""weather"": [
                    { ""id"": 800, ""main"": ""Clear"", ""description"": ""clear sky"", ""icon"": ""01d"" }
                ],
                ""main"": {
                    ""temp"": 72.5,
                    ""feels_like"": 71.2,
                    ""humidity"": 45,
                    ""pressure"": 1015
                },
                ""wind"": {
                    ""speed"": 5.75,
                    ""deg"": 270
                }
            }";

            var weather = WeatherResponseParser.ParseWeather(json);

            Assert.Equal(72.5, weather.Temperature);
            Assert.Equal(71.2, weather.FeelsLike);
            Assert.Equal("clear sky", weather.Description);
            Assert.Equal(45, weather.Humidity);
            Assert.Equal(1015, weather.Pressure);
            Assert.Equal(5.75, weather.WindSpeed);
        }
    }
}
