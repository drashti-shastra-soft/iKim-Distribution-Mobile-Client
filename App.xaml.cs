using i_KimMobileClient.Pages.Accounts;
using i_KimMobileClient.Utilities;

namespace i_KimMobileClient
{
    public partial class App : Application
    {
        public static string baseUrl;
        public App()
        {
            InitializeComponent();
            baseUrl = "https://ikim-web-service.shastrasoft.net/";
            UserAppTheme = AppTheme.Light;
            Common.ResourceDictionary = App.Current.Resources.MergedDictionaries.First();
            Common.StyleResource = App.Current.Resources.MergedDictionaries.Last();
            MainPage = new NavigationPage(new LoginPage());
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new LoginPage());
        //}
    }
}