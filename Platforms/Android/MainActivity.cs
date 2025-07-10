using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace BlazorMauiApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestPermissions();
        }

        private void RequestPermissions()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                // Android 13+ uses READ_MEDIA_AUDIO
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadMediaAudio) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.ReadMediaAudio }, 100);
                }
            }
            else
            {
                // Android 12 and below uses READ_EXTERNAL_STORAGE
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 101);
                }
            }
        }
    }
}
