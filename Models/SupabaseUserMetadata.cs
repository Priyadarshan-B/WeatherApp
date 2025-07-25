using System.Text.Json.Serialization;

namespace BlazorMauiApp1.Models
{
    public class SupabaseUserMetadata
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }
        [JsonPropertyName("family_name")]
        public string? FamilyName { get; set; }
        [JsonPropertyName("picture")]
        public string? Picture { get; set; }
    }
} 