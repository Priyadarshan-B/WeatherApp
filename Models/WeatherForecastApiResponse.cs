using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace BlazorMauiApp1.Models
{
    public class WeatherForecastApiResponse
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; } = new();

        [JsonPropertyName("current")]
        public Current Current { get; set; } = new();

        [JsonPropertyName("forecast")]
        public Forecast Forecast { get; set; } = new();
    }

    public class Location
    {
        public string Name { get; set; } = "";
        public string Region { get; set; } = "";
    }

    public class Current
    {
        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("temp_f")]
        public double TempF { get; set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; set; } = new();
    }

    public class Forecast
    {
        [JsonPropertyName("forecastday")]
        public List<ForecastDay> Forecastday { get; set; } = new();
    }
}
