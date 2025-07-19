using Foundation;
using UIKit;
using BlazorMauiApp1.Services;

namespace BlazorMauiApp1
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            // Handle deep link navigation
            if (url != null)
            {
                DeepLinkService.HandleDeepLink(new Uri(url.ToString()));
            }
            return true;
        }
    }
}
