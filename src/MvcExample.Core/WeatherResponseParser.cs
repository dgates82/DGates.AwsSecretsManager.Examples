using System.Linq;
using Newtonsoft.Json.Linq;

namespace MvcExample.Core
{
    public static class WeatherResponseParser
    {
        public static GeocodeResult[] ParseGeocode(string json)
        {
            var array = JArray.Parse(json);
            return array.Select(t => new GeocodeResult
            {
                Lat = (double)t["lat"],
                Lon = (double)t["lon"],
                Name = (string)t["name"],
                Country = (string)t["country"]
            }).ToArray();
        }

        public static WeatherApiResponse ParseWeather(string json)
        {
            var root = JObject.Parse(json);
            return new WeatherApiResponse
            {
                Temperature = (double)root["main"]["temp"],
                FeelsLike = (double)root["main"]["feels_like"],
                Description = (string)root["weather"][0]["description"],
                Humidity = (int)root["main"]["humidity"],
                Pressure = (double)root["main"]["pressure"],
                WindSpeed = (double)root["wind"]["speed"]
            };
        }
    }
}
