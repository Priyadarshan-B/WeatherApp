using System.Web;

namespace BlazorMauiApp1.Services
{
    public static class DeepLinkService
    {
        /// <summary>
        /// Stores the password reset token extracted from the deep link URI.
        /// </summary>
        public static string? PendingResetToken { get; set; }

        /// <summary>
        /// This event is triggered when a deep link is received.
        /// The MAUI App class will subscribe to this to perform navigation.
        /// The string parameter will be the path, e.g., "/reset-password".
        /// </summary>
        public static event Action<string>? OnNavigationRequested;

        /// <summary>
        /// This is the central method called by both Android and iOS native code.
        /// </summary>
        public static void HandleDeepLink(Uri uri)
        {
            System.Diagnostics.Debug.WriteLine($"DeepLinkService: HandleDeepLink called with URI: {uri}");
            System.Diagnostics.Debug.WriteLine($"DeepLinkService: URI Scheme: {uri.Scheme}");
            System.Diagnostics.Debug.WriteLine($"DeepLinkService: URI Host: {uri.Host}");
            System.Diagnostics.Debug.WriteLine($"DeepLinkService: URI Query: {uri.Query}");
            System.Diagnostics.Debug.WriteLine($"DeepLinkService: URI Fragment: {uri.Fragment}");
            
            try
            {
                // First try to get token from query parameters
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                PendingResetToken = query.Get("access_token");
                
                // If not found in query, try fragment
                if (string.IsNullOrEmpty(PendingResetToken) && !string.IsNullOrEmpty(uri.Fragment))
                {
                    System.Diagnostics.Debug.WriteLine($"DeepLinkService: Token not found in query, checking fragment: {uri.Fragment}");
                    
                    // Remove the leading # from fragment
                    var fragment = uri.Fragment.TrimStart('#');
                    
                    // Parse the fragment as query string
                    var fragmentQuery = System.Web.HttpUtility.ParseQueryString(fragment);
                    PendingResetToken = fragmentQuery.Get("access_token");
                    
                    System.Diagnostics.Debug.WriteLine($"DeepLinkService: Token extracted from fragment: {PendingResetToken}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"DeepLinkService: Token extracted from query: {PendingResetToken}");
                }

                // Determine the navigation path from the host or path of the URI.
                // e.g., blazormauiapp1://reset-password -> /reset-password
                string path = uri.Host.ToLowerInvariant();
                System.Diagnostics.Debug.WriteLine($"DeepLinkService: Path extracted: {path}");
                
                if (path == "reset-password")
                {
                    System.Diagnostics.Debug.WriteLine($"DeepLinkService: Path matches 'reset-password', triggering navigation event");
                    // Trigger the event to notify the app to navigate.
                    OnNavigationRequested?.Invoke("/reset-password");
                    System.Diagnostics.Debug.WriteLine($"DeepLinkService: Navigation event triggered");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"DeepLinkService: Path '{path}' does not match 'reset-password'");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeepLinkService: Error in HandleDeepLink: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DeepLinkService: Stack trace: {ex.StackTrace}");
            }
        }
    }
}
