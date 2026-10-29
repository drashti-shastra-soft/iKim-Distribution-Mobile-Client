
using Acr.UserDialogs;
using i_KimMobileClient.Models;


namespace i_KimMobileClient.Utilities
{
    public static class Common
    {
        public static ResourceDictionary? ResourceDictionary { get; set; }
        public static ResourceDictionary? StyleResource { get; set; }

        public static User mUser { get; set; }
        public static string AuthToken { get; set; }
        public static string CartItemCount { get; set; }

        public static async Task<string> GetCartItemCount()
        {
            string count = string.Empty;
            try
            {
                string url = string.Format(EndPointsUrl.TotalCartItemCount, Common.mUser.StoreId);
                var mResponse = await new APIService.ApiServiceHandler().GetAsync<int>(url, Common.mUser.Token);
                if (mResponse != null && mResponse.IsSuccess)
                {
                    count = mResponse.Value.ToString();
                    Common.CartItemCount = count;
                }

                return count;
            }
            catch (Exception ex)
            {
                Common.ErrorMessage($"Dashboard/GetCartItemCount : {ex.Message}");
                return string.Empty;
            }
        }


        public static bool CheckInternetConnection()
        {
            var networkAccess = Connectivity.Current.NetworkAccess;
            if (networkAccess == NetworkAccess.Internet)
                return true;
            else
                return false;
        }

        public static async Task ControlBounceEffect(object obj = null, double ScaleIn = 0.9, double ScaleOut = 1.0, uint Length = 100)
        {
            if (obj is Border bdr)
            {
                await bdr.ScaleTo(ScaleIn, Length, Easing.Linear);
                await bdr.ScaleTo(ScaleOut, Length, Easing.Linear);
            }
            else if (obj is Image img)
            {
                await img.ScaleTo(ScaleIn, Length, Easing.Linear);
                await img.ScaleTo(ScaleOut, Length, Easing.Linear);
            }
            else if (obj is Button btn)
            {
                await btn.ScaleTo(ScaleIn, Length, Easing.Linear);
                await btn.ScaleTo(ScaleOut, Length, Easing.Linear);
            }
            else if (obj is Label lbl)
            {
                await lbl.ScaleTo(ScaleIn, Length, Easing.Linear);
                await lbl.ScaleTo(ScaleOut, Length, Easing.Linear);
            }
            else if (obj is Grid grd)
            {
                await grd.ScaleTo(ScaleIn, Length, Easing.Linear);
                await grd.ScaleTo(ScaleOut, Length, Easing.Linear);
            }
        }

        public static void ErrorMessage(string message, int duration = 5)
        {
            var toastConfig = new ToastConfig(message)
            {
                Duration = TimeSpan.FromSeconds(duration, 0, 0),
                BackgroundColor = System.Drawing.Color.Red,
                MessageTextColor = System.Drawing.Color.White,
                Position = ToastPosition.Bottom
            };

            UserDialogs.Instance.Toast(toastConfig);
        }
        public static void SuccessMessage(string message, int duration = 5)
        {
            var toastConfig = new ToastConfig(message)
            {
                Duration = TimeSpan.FromSeconds(duration, 0, 0),
                BackgroundColor = System.Drawing.Color.Green,
                MessageTextColor = System.Drawing.Color.White,
                Position = ToastPosition.Bottom
            };

            UserDialogs.Instance.Toast(toastConfig);
        }

        public static void InfoMessage(string message, int duration = 5)
        {
            var toastConfig = new ToastConfig(message)
            {
                Duration = TimeSpan.FromSeconds(duration, 0, 0),
                Message = message,
                BackgroundColor = System.Drawing.Color.DodgerBlue,
                MessageTextColor = System.Drawing.Color.White,
                Position = ToastPosition.Bottom,
            };

            UserDialogs.Instance.Toast(toastConfig);
        }

        public static HomeTabbedPage? GetHomeTabbedPage(Element? caller = null)
        {
            // Method 1: Direct window page
            if (Application.Current?.Windows[0].Page is HomeTabbedPage directPage)
                return directPage;

            // Method 2: Wrapped in NavigationPage
            if (Application.Current?.Windows[0].Page is NavigationPage navPage &&
                navPage.CurrentPage is HomeTabbedPage navHomePage)
                return navHomePage;

            // Method 3: Walk up from caller using Parent
            if (caller != null)
            {
                Element? parent = caller.Parent;
                while (parent != null)
                {
                    if (parent is HomeTabbedPage foundPage)
                        return foundPage;
                    parent = parent.Parent;
                }
            }

            return null;
        }
    }
}
