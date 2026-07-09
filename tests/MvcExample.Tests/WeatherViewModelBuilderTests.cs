using MvcExample.Core;
using Xunit;

namespace MvcExample.Tests
{
    public class WeatherViewModelBuilderTests
    {
        [Fact]
        public void Ready_PopulatesFetchInfoWithNoResultOrError()
        {
            var model = WeatherViewModelBuilder.Ready("dev/MvcExample/OpenWeatherMap", "AWS Secrets Manager");

            Assert.Equal("dev/MvcExample/OpenWeatherMap", model.SecretName);
            Assert.Equal("AWS Secrets Manager", model.Source);
            Assert.False(model.HasResult);
            Assert.True(model.IsConfigured);
            Assert.Null(model.ErrorMessage);
        }

        [Fact]
        public void NotConfigured_SetsIsConfiguredFalseWithMessage()
        {
            var model = WeatherViewModelBuilder.NotConfigured();

            Assert.False(model.IsConfigured);
            Assert.False(string.IsNullOrEmpty(model.ConfigurationMessage));
            Assert.False(model.HasResult);
        }

        [Fact]
        public void BackendUnavailable_SetsIsConfiguredFalseWithMessage()
        {
            var model = WeatherViewModelBuilder.BackendUnavailable();

            Assert.False(model.IsConfigured);
            Assert.Contains("LocalStack", model.ConfigurationMessage);
            Assert.False(model.HasResult);
        }

        [Fact]
        public void LocalFallbackFileNotFound_SetsIsConfiguredFalseWithMessage()
        {
            var model = WeatherViewModelBuilder.LocalFallbackFileNotFound();

            Assert.False(model.IsConfigured);
            Assert.Contains("LocalJsonFallbackPath", model.ConfigurationMessage);
            Assert.False(model.HasResult);
        }

        [Fact]
        public void MissingCityName_SetsErrorMessage()
        {
            var model = WeatherViewModelBuilder.MissingCityName();

            Assert.Equal("Please enter a city name.", model.ErrorMessage);
            Assert.False(model.HasResult);
        }

        [Fact]
        public void CityNotFound_IncludesCityNameInMessage()
        {
            var model = WeatherViewModelBuilder.CityNotFound("Atlantis");

            Assert.Equal("Atlantis", model.CityName);
            Assert.Contains("Atlantis", model.ErrorMessage);
            Assert.False(model.HasResult);
        }

        [Fact]
        public void Error_DoesNotLeakExceptionDetails()
        {
            var model = WeatherViewModelBuilder.Error("San Diego");

            Assert.Equal("Something went wrong fetching weather data. Please try again.", model.ErrorMessage);
            Assert.False(model.HasResult);
        }

        [Fact]
        public void Success_PopulatesResultFromGeocodeAndWeather()
        {
            var geocode = new GeocodeResult { Name = "San Diego", Country = "US", Lat = 32.7157, Lon = -117.1611 };
            var weather = new WeatherApiResponse
            {
                Temperature = 72.5,
                FeelsLike = 71.2,
                Description = "clear sky",
                Humidity = 45,
                Pressure = 1015,
                WindSpeed = 5.75
            };

            var model = WeatherViewModelBuilder.Success(
                "San Diego", geocode, weather,
                "dev/MvcExample/OpenWeatherMap", "https://api.openweathermap.org/data/2.5/weather",
                "AWS Secrets Manager");

            Assert.True(model.HasResult);
            Assert.Null(model.ErrorMessage);
            Assert.Equal("San Diego, US", model.Location);
            Assert.Equal(72.5, model.Temperature);
            Assert.Equal(71.2, model.FeelsLike);
            Assert.Equal("clear sky", model.Description);
            Assert.Equal(45, model.Humidity);
            Assert.Equal(1015, model.Pressure);
            Assert.Equal(5.75, model.WindSpeed);
            Assert.Equal("dev/MvcExample/OpenWeatherMap", model.SecretName);
            Assert.Equal("https://api.openweathermap.org/data/2.5/weather", model.RetrievedUrl);
            Assert.Equal("AWS Secrets Manager", model.Source);
        }
    }
}
