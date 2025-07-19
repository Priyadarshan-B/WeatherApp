using Microsoft.AspNetCore.Components; // For IComponent
using Microsoft.AspNetCore.Components.WebView.Maui; // For BlazorWebView, RootComponent
using Microsoft.Maui.Controls; // For ContentPage, Application

namespace BlazorMauiApp1.Services
{
    public class MauiNavigationService : IMauiNavigationService
    {
        public void NavigateToBlazorMauiPage<TBlazorComponent>(string hostPage = "wwwroot/index.html", string selector = "#app")
            where TBlazorComponent : IComponent
        {
            System.Diagnostics.Debug.WriteLine($"MauiNavigationService: NavigateToBlazorMauiPage called for type: {typeof(TBlazorComponent).Name}");
            
            // Ensure this runs on the main UI thread, especially important for UI operations
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"MauiNavigationService: Creating BlazorWebView for {typeof(TBlazorComponent).Name}");
                    
                    var blazorWebView = new BlazorWebView
                    {
                        HostPage = hostPage,
                        RootComponents =
                        {
                            new RootComponent { Selector = selector, ComponentType = typeof(TBlazorComponent) }
                        }
                    };

                    System.Diagnostics.Debug.WriteLine($"MauiNavigationService: BlazorWebView created, setting MainPage");
                    
                    Application.Current.MainPage = new ContentPage
                    {
                        Content = blazorWebView
                    };
                    
                    System.Diagnostics.Debug.WriteLine($"MauiNavigationService: MainPage set successfully");
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"MauiNavigationService: Navigation error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"MauiNavigationService: Stack trace: {ex.StackTrace}");
                }
            });
        }

        public void SetMauiMainPage(Page mauiPage)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = mauiPage;
            });
        }
    }
}