using System;

namespace MvcExample.Core
{
    public static class WeatherApiUrlBuilder
    {
        public static string BuildGeocodeUrl(string apiKey, string cityName)
        {
            return $"http://api.openweathermap.org/geo/1.0/direct?q={Uri.EscapeDataString(cityName)}&limit=1&appid={apiKey}";
        }

        public static string BuildWeatherUrl(OpenWeatherMapSecret secret, double lat, double lon)
        {
            return $"{secret.Url}?lat={lat}&lon={lon}&appid={secret.Key}&units=imperial";
        }
    }
}
