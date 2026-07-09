namespace MvcExample.Core
{
    public static class WeatherViewModelBuilder
    {
        public static WeatherViewModel Empty()
        {
            return new WeatherViewModel();
        }

        public static WeatherViewModel NotConfigured()
        {
            return new WeatherViewModel
            {
                IsConfigured = false,
                ConfigurationMessage =
                    "Set a real OpenWeatherMap API key in the seeded secret to see live weather data. See README for instructions."
            };
        }

        public static WeatherViewModel MissingCityName()
        {
            return new WeatherViewModel
            {
                ErrorMessage = "Please enter a city name."
            };
        }

        public static WeatherViewModel CityNotFound(string cityName)
        {
            return new WeatherViewModel
            {
                CityName = cityName,
                ErrorMessage = $"Couldn't find '{cityName}' — try a different city name."
            };
        }

        public static WeatherViewModel Error(string cityName)
        {
            return new WeatherViewModel
            {
                CityName = cityName,
                ErrorMessage = "Something went wrong fetching weather data. Please try again."
            };
        }

        public static WeatherViewModel Success(string cityName, GeocodeResult geocode, WeatherApiResponse weather)
        {
            return new WeatherViewModel
            {
                CityName = cityName,
                HasResult = true,
                Location = $"{geocode.Name}, {geocode.Country}",
                Temperature = weather.Temperature,
                FeelsLike = weather.FeelsLike,
                Description = weather.Description,
                Humidity = weather.Humidity,
                Pressure = weather.Pressure,
                WindSpeed = weather.WindSpeed
            };
        }
    }
}
