using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using BlazorMauiApp1.Services;

namespace BlazorMauiApp1
{
    // 1. Set LaunchMode to SingleTop to handle intents correctly if app is open
    // 2. Add the IntentFilter attribute here to register the deep link
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "blazormauiapp1",
        DataHost = "reset-password"
    )]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine("MainActivity: OnCreate called");
            base.OnCreate(savedInstanceState);
            RequestPermissions();
            
            // Handle the intent that may have launched the app
            System.Diagnostics.Debug.WriteLine($"MainActivity: OnCreate - Intent: {Intent?.Data}");
            HandleIntent(Intent);
        }

        protected override void OnNewIntent(Intent? intent)
        {
            System.Diagnostics.Debug.WriteLine($"MainActivity: OnNewIntent called with intent: {intent?.Data}");
            base.OnNewIntent(intent);
            // Handle the intent if the app was already running in the background
            HandleIntent(intent);
        }

        private void HandleIntent(Intent? intent)
        {
            System.Diagnostics.Debug.WriteLine($"MainActivity: HandleIntent called with intent: {intent?.Data}");
            
            if (intent?.Data is Android.Net.Uri uri)
            {
                System.Diagnostics.Debug.WriteLine($"MainActivity: URI found: {uri}");
                System.Diagnostics.Debug.WriteLine($"MainActivity: URI Scheme: {uri.Scheme}");
                System.Diagnostics.Debug.WriteLine($"MainActivity: URI Host: {uri.Host}");
                System.Diagnostics.Debug.WriteLine($"MainActivity: URI Query: {uri.Query}");
                
                try
                {
                    // When a deep link is received, pass the full URI to our central service
                    // This is the bridge from the native Android world to your shared .NET code.
                    DeepLinkService.HandleDeepLink(new Uri(uri.ToString()));
                    System.Diagnostics.Debug.WriteLine("MainActivity: DeepLinkService.HandleDeepLink completed");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MainActivity: Error in HandleIntent: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"MainActivity: Stack trace: {ex.StackTrace}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"MainActivity: No URI found in intent");
            }
        }

        private void RequestPermissions()
        {
#pragma warning disable CA1416 
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadMediaAudio) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.ReadMediaAudio }, 100);
                }
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 101);
                }
            }
#pragma warning restore CA1416
        }
    }
}
