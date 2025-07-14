using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace BlazorMauiApp1.Platforms.Android
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.googleusercontent.apps.414117394957-a38nmfe71pm3lgi3m7d2qhiv53p0alpu"
        // No DataHost, since the path is just "/"
    )]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
} 