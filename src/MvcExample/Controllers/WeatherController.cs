using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcExample.Core;
using MvcExample.Infrastructure;
using NLog;

namespace MvcExample.Controllers
{
    public class WeatherController : Controller
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            SecretFetchResult<OpenWeatherMapSecret> result;
            try
            {
                result = await FetchSecretAsync();
            }
            catch (FileNotFoundException ex)
            {
                Trace.TraceError("LocalJsonFallbackPath file not found on page load: " + ex);
                return View(WeatherViewModelBuilder.LocalFallbackFileNotFound());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to retrieve OpenWeatherMap secret on page load: " + ex);
                return View(WeatherViewModelBuilder.BackendUnavailable());
            }

            if (result.Secret == null || ApiKeyEvaluator.IsPlaceholder(result.Secret.Key))
            {
                return View(WeatherViewModelBuilder.NotConfigured());
            }

            return View(WeatherViewModelBuilder.Ready(result.SecretName, result.Source));
        }

        [HttpPost]
        public async Task<ActionResult> Index(string cityName)
        {
            Log.Info("Received request for weather data for city: {CityName}", cityName);
            
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return View(WeatherViewModelBuilder.MissingCityName());
            }

            SecretFetchResult<OpenWeatherMapSecret> result;
            try
            {
                result = await FetchSecretAsync();
            }
            catch (FileNotFoundException ex)
            {
                Trace.TraceError("LocalJsonFallbackPath file not found: " + ex);
                return View(WeatherViewModelBuilder.LocalFallbackFileNotFound());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to retrieve OpenWeatherMap secret: " + ex);
                return View(WeatherViewModelBuilder.Error(cityName));
            }

            var secret = result.Secret;
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

                return View(WeatherViewModelBuilder.Success(
                    cityName, geocode, weather, result.SecretName, secret.Url, result.Source));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to retrieve weather data for '" + cityName + "': " + ex);
                return View(WeatherViewModelBuilder.Error(cityName));
            }
        }

        private static Task<SecretFetchResult<OpenWeatherMapSecret>> FetchSecretAsync()
        {
            var secretName = ConfigurationManager.AppSettings["Secrets:OpenWeatherMapName"];
            return SecretsManagerAccessor.GetSecretAsync<OpenWeatherMapSecret>(secretName);
        }
    }
}
