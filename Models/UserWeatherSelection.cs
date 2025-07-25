using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BlazorMauiApp1.Models
{
    public class UserWeatherSelection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userEmail")]
        public string UserEmail { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("selectedAt")]
        public DateTime SelectedAt { get; set; }

        [BsonElement("weatherJson")]
        public string WeatherJson { get; set; } = string.Empty;
    }
} 