namespace MvcExample.Core
{
    public class WeatherViewModel
    {
        public string CityName { get; set; }

        public bool IsConfigured { get; set; } = true;
        public string ConfigurationMessage { get; set; }

        public string ErrorMessage { get; set; }

        public bool HasResult { get; set; }
        public string Location { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public string Description { get; set; }
        public int Humidity { get; set; }
        public double Pressure { get; set; }
        public double WindSpeed { get; set; }

        // Populated whenever the secret was successfully fetched, so the UI can show how the
        // library actually behaved (not shown while IsConfigured is false or a fetch failed).
        public string SecretName { get; set; }
        public string RetrievedUrl { get; set; }
        public string Source { get; set; }
    }
}
