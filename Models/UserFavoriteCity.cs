using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BlazorMauiApp1.Models
{
    public class UserFavoriteCity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("favoriteCities")]
        public List<string> FavoriteCities { get; set; } = new List<string>();
    }
}