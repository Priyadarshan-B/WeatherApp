using BlazorMauiApp1.Models;

namespace BlazorMauiApp1.Services
{
    public interface IAuthService
    {
        Task<bool> IsAuthenticatedAsync();
        Task<UserDetails?> GetCurrentUserAsync();
        Task<bool> SignInWithGoogleAsync();
        Task SignOutAsync();
        Task<UserDetails?> GetUserDetailsAsync();
        Task<bool> SignInWithSupabaseGoogleAsync();
        event Action<UserDetails?> OnUserChanged;
        Task<bool> RegisterAsync(string email, string password);
        Task<bool> LoginAsync(string email, string password);
        Task<bool> SendPasswordResetEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string accessToken, string newPassword);
    }
} 