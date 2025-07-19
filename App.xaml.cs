using Microsoft.Maui.Controls;
using BlazorMauiApp1.Services;
using System.Web;

namespace BlazorMauiApp1
{
    public partial class App : Application
    {
        private readonly IMauiNavigationService _navigationService;

        public App(IAuthService authService, IMauiNavigationService navigationService)
        {
            System.Diagnostics.Debug.WriteLine("App: Constructor called");
            InitializeComponent();
            
            _navigationService = navigationService;
            System.Diagnostics.Debug.WriteLine("App: Navigation service injected");
            
            // Subscribe to navigation events
            DeepLinkService.OnNavigationRequested += HandleNavigationRequested;
            System.Diagnostics.Debug.WriteLine("App: Subscribed to DeepLinkService.OnNavigationRequested");
            
            MainPage = new ContentPage { Content = new ActivityIndicator { IsRunning = true } };
            System.Diagnostics.Debug.WriteLine("App: Set initial MainPage");
            SetMainPageAsync(authService);
        }

        private async void SetMainPageAsync(IAuthService authService)
        {
            System.Diagnostics.Debug.WriteLine("App: SetMainPageAsync called");
            try
            {
                if (await authService.IsAuthenticatedAsync())
                {
                    System.Diagnostics.Debug.WriteLine("App: User is authenticated, setting MainPage");
                    MainPage = new MainPage(); // TabbedPage
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("App: User is not authenticated, setting LoginPage");
                    MainPage = new LoginPage(); // Standalone login page
                }
                System.Diagnostics.Debug.WriteLine("App: MainPage set successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App: Error in SetMainPageAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"App: Stack trace: {ex.StackTrace}");
            }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            System.Diagnostics.Debug.WriteLine($"App: OnAppLinkRequestReceived called with URI: {uri}");
            try
            {
                // Handle deep link navigation using the navigation service
                DeepLinkService.HandleDeepLink(uri);
                System.Diagnostics.Debug.WriteLine("App: DeepLinkService.HandleDeepLink completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App: Error in OnAppLinkRequestReceived: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"App: Stack trace: {ex.StackTrace}");
            }
            
            base.OnAppLinkRequestReceived(uri);
        }

        private void HandleNavigationRequested(string path)
        {
            System.Diagnostics.Debug.WriteLine($"App: HandleNavigationRequested called with path: {path}");
            try
            {
                // When the event fires, use your navigation service to show the right page.
                if (path == "/reset-password")
                {
                    System.Diagnostics.Debug.WriteLine($"App: Navigating to reset password page. Path: {path}");
                    _navigationService.NavigateToBlazorMauiPage<BlazorMauiApp1.Components.Pages.ResetPasswordPage>();
                    System.Diagnostics.Debug.WriteLine("App: Navigation service called successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"App: Unknown path: {path}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App: Error in HandleNavigationRequested: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"App: Stack trace: {ex.StackTrace}");
            }
        }

        // Method to manually test deep link (for debugging)
        public void TestDeepLink()
        {
            System.Diagnostics.Debug.WriteLine("App: TestDeepLink called manually");
            var testUri = new Uri("blazormauiapp1://reset-password?access_token=test123");
            DeepLinkService.HandleDeepLink(testUri);
        }
    }
}
