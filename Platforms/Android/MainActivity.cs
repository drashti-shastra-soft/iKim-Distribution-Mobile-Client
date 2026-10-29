using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;

namespace i_KimMobileClient
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set background color of the status bar
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FF7A1A")); 
            Window?.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#FF7A1A"));
           
            UserDialogs.Init(this);
        }
    }
}
