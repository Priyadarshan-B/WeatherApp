using System;
using System.Text.Json.Serialization;

namespace BlazorMauiApp1.Models
{
    public class SupabaseUserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("email_confirmed")]
        public bool EmailConfirmed { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("user_metadata")]
        public SupabaseUserMetadata? UserMetadata { get; set; }
    }
} 