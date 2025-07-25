using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BlazorMauiApp1.Models
{
    public class MongoUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("lastSignInAt")]
        public DateTime LastSignInAt { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("provider")]
        public string Provider { get; set; } = string.Empty;

        [BsonElement("avatarUrl")]
        public string? AvatarUrl { get; set; }
    }
} 