using BlazorMauiApp1.Models;
using Microsoft.Maui.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BlazorMauiApp1.Services
{
    public class AuthService : IAuthService
    {
        private UserDetails? _currentUser;

        public event Action<UserDetails?> OnUserChanged = null!;

        public AuthService()
        {
            LoadUserFromStorageAsync();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            if (_currentUser != null)
                return true;

            // Try to load from storage
            var user = await LoadUserDetailsAsync();
            return user != null;
        }

        public async Task<UserDetails?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            return await LoadUserDetailsAsync();
        }

        public async Task<bool> SignInWithGoogleAsync()
        {
            try
            {
                var clientId = GoogleAuthConfig.ClientId;
                var redirectUri = GoogleAuthConfig.RedirectUri;
                var scope = "openid email profile";
                
                var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                             $"client_id={clientId}&" +
                             $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                             $"response_type=code&" +
                             $"scope={Uri.EscapeDataString(scope)}&" +
                             $"access_type=offline";

                var callbackUrl = new Uri(redirectUri);
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(new Uri(authUrl), callbackUrl);

                if (authResult?.Properties != null && authResult.Properties.TryGetValue("code", out var code))
                {
                    var tokenResponse = await ExchangeCodeForTokensAsync(code, redirectUri);
                    
                    if (tokenResponse != null)
                    {
                        var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);
                        
                        if (userInfo != null)
                        {
                            _currentUser = new UserDetails
                            {
                                Id = userInfo.Id,
                                Email = userInfo.Email,
                                Name = userInfo.Name,
                                AvatarUrl = userInfo.Picture,
                                GivenName = userInfo.GivenName,
                                FamilyName = userInfo.FamilyName,
                                Picture = userInfo.Picture,
                                EmailVerified = userInfo.EmailVerified,
                                CreatedAt = DateTime.UtcNow,
                                LastSignInAt = DateTime.UtcNow
                            };

                            OnUserChanged?.Invoke(_currentUser);
                            await SaveUserDetailsAsync(_currentUser);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Google sign-in: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> SignInWithSupabaseGoogleAsync()
        {
            try
            {
                var supabaseUrl = SupabaseConfig.SupabaseUrl;
                var redirectUri = SupabaseConfig.RedirectUri;
                var authUrl = $"{supabaseUrl}/auth/v1/authorize?provider=google&redirect_to={Uri.EscapeDataString(redirectUri)}";

                var callbackUrl = new Uri(redirectUri);
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(new Uri(authUrl), callbackUrl);

                // Debug: Log all properties returned from the callback
                if (authResult?.Properties != null)
                {
                    foreach (var kvp in authResult.Properties)
                    {
                        Console.WriteLine($"AuthResult Property: {kvp.Key} = {kvp.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("No properties returned from WebAuthenticator.");
                }

                // Supabase returns access_token and other info in the URL fragment or as query params
                string? accessToken = null;
                string? refreshToken = null;
                string? providerToken = null;
                if (authResult?.Properties != null)
                {
                    // Extract tokens from properties
                    authResult.Properties.TryGetValue("access_token", out accessToken);
                    authResult.Properties.TryGetValue("refresh_token", out refreshToken);
                    authResult.Properties.TryGetValue("provider_token", out providerToken);

                    // Optional: Log for debugging
                    Console.WriteLine($"Supabase access_token: {accessToken}");
                    Console.WriteLine($"Supabase refresh_token: {refreshToken}");
                    Console.WriteLine($"Supabase provider_token: {providerToken}");
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var userInfo = await GetSupabaseUserInfoAsync(accessToken);
                    if (userInfo != null)
                    {
                        Console.WriteLine("Supabase user info successfully retrieved.");
                        _currentUser = new UserDetails
                        {
                            Id = userInfo.Id,
                            Email = userInfo.Email,
                            Name = userInfo.UserMetadata?.Name ?? userInfo.Email,
                            AvatarUrl = userInfo.UserMetadata?.AvatarUrl ?? string.Empty,
                            GivenName = userInfo.UserMetadata?.GivenName ?? string.Empty,
                            FamilyName = userInfo.UserMetadata?.FamilyName ?? string.Empty,
                            Picture = userInfo.UserMetadata?.Picture ?? string.Empty,
                            EmailVerified = userInfo.EmailConfirmed,
                            CreatedAt = userInfo.CreatedAt,
                            LastSignInAt = DateTime.UtcNow
                        };
                        OnUserChanged?.Invoke(_currentUser);
                        await SaveUserDetailsAsync(_currentUser);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Supabase user info is null. Failed to fetch user info with access token.");
                    }
                }
                else
                {
                    Console.WriteLine("Supabase access_token is null or empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Supabase Google sign-in: {ex.Message}");
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var supabaseUrl = SupabaseConfig.SupabaseUrl;
                var anonKey = SupabaseConfig.SupabaseAnonKey;
                var payload = new { email, password };
                var json = JsonSerializer.Serialize(payload);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", anonKey);
                var response = await client.PostAsync(
                    $"{supabaseUrl}/auth/v1/signup",
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var supabaseUrl = SupabaseConfig.SupabaseUrl;
                var anonKey = SupabaseConfig.SupabaseAnonKey;
                var payload = new { email, password };
                var json = JsonSerializer.Serialize(payload);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", anonKey);
                var response = await client.PostAsync(
                    $"{supabaseUrl}/auth/v1/token?grant_type=password",
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                );
                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadAsStringAsync();
                    var tokenResp = JsonSerializer.Deserialize<TokenResponse>(respJson);
                    if (tokenResp?.AccessToken != null)
                    {
                        var userInfo = await GetSupabaseUserInfoAsync(tokenResp.AccessToken);
                        if (userInfo != null)
                        {
                            _currentUser = new UserDetails
                            {
                                Id = userInfo.Id,
                                Email = userInfo.Email,
                                Name = userInfo.UserMetadata?.Name ?? userInfo.Email,
                                AvatarUrl = userInfo.UserMetadata?.AvatarUrl ?? string.Empty,
                                GivenName = userInfo.UserMetadata?.GivenName ?? string.Empty,
                                FamilyName = userInfo.UserMetadata?.FamilyName ?? string.Empty,
                                Picture = userInfo.UserMetadata?.Picture ?? string.Empty,
                                EmailVerified = userInfo.EmailConfirmed,
                                CreatedAt = userInfo.CreatedAt,
                                LastSignInAt = DateTime.UtcNow
                            };
                            OnUserChanged?.Invoke(_currentUser);
                            await SaveUserDetailsAsync(_currentUser);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
            }
            return false;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var supabaseUrl = SupabaseConfig.SupabaseUrl;
                var anonKey = SupabaseConfig.SupabaseAnonKey;
                var payload = new { email, redirect_to = "yourapp://reset-password" }; // Replace with your app's scheme
                var json = JsonSerializer.Serialize(payload);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", anonKey);
                var response = await client.PostAsync(
                    $"{supabaseUrl}/auth/v1/recover",
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string accessToken, string newPassword)
        {
            try
            {
                var supabaseUrl = SupabaseConfig.SupabaseUrl;
                var anonKey = SupabaseConfig.SupabaseAnonKey;
                var payload = new { password = newPassword };
                var json = JsonSerializer.Serialize(payload);
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("apikey", anonKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var response = await client.PutAsync(
                    $"{supabaseUrl}/auth/v1/user",
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                );
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting password: {ex.Message}");
                return false;
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                _currentUser = null;
                OnUserChanged?.Invoke(null);
                await ClearUserDetailsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sign-out: {ex.Message}");
            }
        }

        public async Task<UserDetails?> GetUserDetailsAsync()
        {
            // First try to get from memory
            if (_currentUser != null)
                return _currentUser;

            // Then try to get from local storage
            var user = await GetCurrentUserAsync();
            if (user != null)
                return user;

            // Finally try to get from local storage
            return await LoadUserDetailsAsync();
        }

        private async Task SaveUserDetailsAsync(UserDetails userDetails)
        {
            try
            {
                var json = JsonSerializer.Serialize(userDetails);
                await SecureStorage.Default.SetAsync("user_details", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user details: {ex.Message}");
            }
        }

        private async Task<UserDetails?> LoadUserDetailsAsync()
        {
            try
            {
                var json = await SecureStorage.Default.GetAsync("user_details");
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<UserDetails>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user details: {ex.Message}");
            }

            return null;
        }

        private async Task ClearUserDetailsAsync()
        {
            try
            {
                SecureStorage.Default.Remove("user_details");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing user details: {ex.Message}");
            }
        }

        private async Task<TokenResponse?> ExchangeCodeForTokensAsync(string code, string redirectUri)
        {
            try
            {
                var clientId = GoogleAuthConfig.ClientId;
                var clientSecret = GoogleAuthConfig.ClientSecret;

                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                });

                using var client = new HttpClient();
                var response = await client.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TokenResponse>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exchanging code for tokens: {ex.Message}");
            }

            return null;
        }

        private async Task<GoogleUserInfo?> GetUserInfoAsync(string accessToken)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<GoogleUserInfo>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user info: {ex.Message}");
            }

            return null;
        }

        private async Task<SupabaseUserInfo?> GetSupabaseUserInfoAsync(string accessToken)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                client.DefaultRequestHeaders.Add("apikey", SupabaseConfig.SupabaseAnonKey); // <-- Add this line!

                var supabaseUrl = SupabaseConfig.SupabaseUrl;
                var response = await client.GetAsync($"{supabaseUrl}/auth/v1/user");
                Console.WriteLine($"Supabase /auth/v1/user status: {response.StatusCode}");
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Supabase /auth/v1/user response: {json}");
                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<SupabaseUserInfo>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Supabase user info: {ex.Message}");
            }
            return null;
        }

        private async Task LoadUserFromStorageAsync()
        {
            try
            {
                var user = await LoadUserDetailsAsync();
                if (user != null)
                {
                    _currentUser = user;
                    OnUserChanged?.Invoke(_currentUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user from storage: {ex.Message}");
            }
        }
    }

    // Helper classes for Google OAuth
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }

    public class GoogleUserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; } = string.Empty;
        
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; } = string.Empty;
        
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;
        
        [JsonPropertyName("verified_email")]
        public bool EmailVerified { get; set; }
    }

    // Add SupabaseUserInfo model for deserialization
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
