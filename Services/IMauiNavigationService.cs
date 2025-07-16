// Services/IMauiNavigationService.cs
using Microsoft.AspNetCore.Components; 
using Microsoft.Maui.Controls; // For Page

namespace BlazorMauiApp1.Services
{
    public interface IMauiNavigationService
    {
        /// <summary>
        /// Navigates the MAUI application's MainPage to a new ContentPage hosting a BlazorWebView
        /// with the specified Blazor component as its root.
        /// </summary>
        /// <typeparam name="TBlazorComponent">The Blazor component type to load (e.g., Pages.Login, Pages.Profile).</typeparam>
        /// <param name="hostPage">The host HTML page for the BlazorWebView (defaults to "wwwroot/index.html").</param>
        /// <param name="selector">The CSS selector for the root component (defaults to "#app").</param>
        void NavigateToBlazorMauiPage<TBlazorComponent>(string hostPage = "wwwroot/index.html", string selector = "#app")
            where TBlazorComponent : IComponent;

        /// <summary>
        /// Navigates the MAUI application's MainPage to a specific MAUI Page instance.
        /// This is useful for navigating to a non-Blazor MAUI Page directly.
        /// </summary>
        /// <param name="mauiPage">The MAUI Page instance to set as MainPage.</param>
        void SetMauiMainPage(Page mauiPage);
    }
}