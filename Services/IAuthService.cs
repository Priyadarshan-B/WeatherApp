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
        event Action<UserDetails?> OnUserChanged;
    }
} 