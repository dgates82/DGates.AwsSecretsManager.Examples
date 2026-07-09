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
    }
}
