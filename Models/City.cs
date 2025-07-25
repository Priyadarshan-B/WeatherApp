using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorMauiApp1.Models
{
    public class City
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
    }
} 