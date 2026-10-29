using i_KimMobileClient.PopupPages;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Services;

//public static class NetworkService
//{
//    public static async Task<bool> CheckInternetAsync(Page page)
//    {
//        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
//            return true;

//        await page.DisplayAlert(
//            "No Internet",
//            "Please check your internet connection and try again.",
//            "OK");

//        return false;
//    }
//}

public static class NetworkService
{
    public static async Task<bool> CheckInternetAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            return true;

        //var page = Application.Current?.Windows[0].Page;

        //if (page != null)
        //{
        //    await page.DisplayAlert(
        //        "No Internet",
        //        "Please check your internet connection and try again.",
        //        "OK");
        //}

        var popup = new CommonAlertPopupPage(
                       "Alert",
                       "Please check your internet connection and try again.",
                       "OK");

        popup.ActionEvent += async (s, e) =>
        {
            await MopupService.Instance.PopAsync();
        };

        await MopupService.Instance.PushAsync(popup);

        return false;
    }
}


//public static class NetworkService
//{
    //public static bool HasInternet()
    //{
    //    return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    //}
//}


//public static class NetworkService
//{
//    public static bool HasInternet() =>
//        Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
//}



//public static class DialogService
//{
//    public static async Task ShowNoInternetPopupAsync()
//    {
//        await MainThread.InvokeOnMainThreadAsync(async () =>
//        {
//            if (MopupService.Instance.PopupStack.Count > 0)
//                return;

//            await MopupService.Instance.PushAsync(
//                new CommonAlertPopupPage(
//                    "No Internet",
//                    "Please check your internet connection and try again.",
//                    "OK"));
//        });
//    }
//}


//public static class NetworkService
//{
//    private static bool _isPopupOpen;

//    public static async Task<bool> CheckInternetAsync()
//    {
//        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
//            return true;

//        if (_isPopupOpen)
//            return false;

//        _isPopupOpen = true;

//        try
//        {
//            await MainThread.InvokeOnMainThreadAsync(async () =>
//            {
//                var popup = new CommonAlertPopupPage(
//                    "No Internet",
//                    "Please check your internet connection and try again.",
//                    "OK");

//                popup.Disappearing += (s, e) =>
//                {
//                    _isPopupOpen = false;
//                };

//                await MopupService.Instance.PushAsync(popup);
//            });
//        }
//        catch
//        {
//            _isPopupOpen = false;
//        }

//        return false;
//    }
//}

