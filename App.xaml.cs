using Microsoft.Maui.Controls;
using BlazorMauiApp1.Services;

namespace BlazorMauiApp1
{
    public partial class App : Application
    {
        public App(IAuthService authService)
        {
            InitializeComponent();
            MainPage = new ContentPage { Content = new ActivityIndicator { IsRunning = true } };
            SetMainPageAsync(authService);
        }

        private async void SetMainPageAsync(IAuthService authService)
        {
            if (await authService.IsAuthenticatedAsync())
                MainPage = new MainPage(); // TabbedPage
            else
                MainPage = new LoginPage(); // Standalone login page
        }
    }
}
