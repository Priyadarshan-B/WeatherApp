using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorMauiApp1.Models
{
    public class ForecastDay
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("day")]
        public Day Day { get; set; } = new();
    }

    public class Day
    {
        [JsonPropertyName("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonPropertyName("maxtemp_f")]
        public double MaxTempF { get; set; }

        [JsonPropertyName("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonPropertyName("mintemp_f")]
        public double MinTempF { get; set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; set; } = new();
    }

    public class Condition
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = "";
    }
}
