using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcExample.Core;
using MvcExample.Infrastructure;

namespace MvcExample.Controllers
{
    public class WeatherController : Controller
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [HttpGet]
        public ActionResult Index()
        {
            return View(WeatherViewModelBuilder.Empty());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return View(WeatherViewModelBuilder.MissingCityName());
            }

            var secretName = ConfigurationManager.AppSettings["Secrets:OpenWeatherMapName"];

            OpenWeatherMapSecret secret;
            try
            {
                secret = await SecretsManagerAccessor.GetSecretAsync<OpenWeatherMapSecret>(secretName);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to retrieve OpenWeatherMap secret '" + secretName + "': " + ex);
                return View(WeatherViewModelBuilder.Error(cityName));
            }

            if (secret == null || ApiKeyEvaluator.IsPlaceholder(secret.Key))
            {
                return View(WeatherViewModelBuilder.NotConfigured());
            }

            try
            {
                var geocodeUrl = WeatherApiUrlBuilder.BuildGeocodeUrl(secret.Key, cityName);
                var geocodeJson = await HttpClient.GetStringAsync(geocodeUrl);
                var geocodeResults = WeatherResponseParser.ParseGeocode(geocodeJson);

                if (geocodeResults.Length == 0)
                {
                    return View(WeatherViewModelBuilder.CityNotFound(cityName));
                }

                var geocode = geocodeResults[0];
                var weatherUrl = WeatherApiUrlBuilder.BuildWeatherUrl(secret, geocode.Lat, geocode.Lon);
                var weatherJson = await HttpClient.GetStringAsync(weatherUrl);
                var weather = WeatherResponseParser.ParseWeather(weatherJson);

                return View(WeatherViewModelBuilder.Success(cityName, geocode, weather));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to retrieve weather data for '" + cityName + "': " + ex);
                return View(WeatherViewModelBuilder.Error(cityName));
            }
        }
    }
}
