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
            // Ensure this runs on the main UI thread, especially important for UI operations
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = new ContentPage
                {
                    Content = new BlazorWebView
                    {
                        HostPage = hostPage,
                        RootComponents =
                        {
                            new RootComponent { Selector = selector, ComponentType = typeof(TBlazorComponent) }
                        }
                    }
                };
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