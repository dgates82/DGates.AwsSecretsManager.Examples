using System;

namespace MvcExample.Core
{
    public static class WeatherEmojiMapper
    {
        public static string GetEmoji(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }

            var text = description.ToLowerInvariant();

            if (text.Contains("thunder")) return "⛈️";
            if (text.Contains("snow")) return "❄️";
            if (text.Contains("rain") || text.Contains("drizzle") || text.Contains("shower")) return "🌧️";
            if (text.Contains("mist") || text.Contains("fog") || text.Contains("haze")) return "🌫️";
            if (text.Contains("cloud")) return "☁️";
            if (text.Contains("clear")) return "☀️";

            return "🌡️";
        }
    }
}
