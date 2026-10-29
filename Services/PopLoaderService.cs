using i_KimMobileClient.PopupPages;
using Mopups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Services;

//public static class PopLoaderService
//{
//    private static CustomLoaderPopup? _loaderPopup;
//    private static bool _isShowing;


//    public static async Task ShowAsync()
//    {
//        if (_isShowing)
//            return;

//        _isShowing = true;

//        await MainThread.InvokeOnMainThreadAsync(async () =>
//        {
//            _loaderPopup = new CustomLoaderPopup();

//            await MopupService.Instance.PushAsync(_loaderPopup);
//        });
//    }



//    public static async Task HideAsync()
//    {
//        if (!_isShowing || _loaderPopup == null)
//            return;

//        await MainThread.InvokeOnMainThreadAsync(async () =>
//        {
//            await MopupService.Instance.RemovePageAsync(_loaderPopup);

//            _loaderPopup = null;
//            _isShowing = false;
//        });
//    }
//}


public static class PopLoaderService
{
    private static CustomLoaderPopup? _loaderPopup;
    private static bool _isShowing;

    public static async Task ShowAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (_isShowing)
                return;

            var popup = new CustomLoaderPopup();

            _loaderPopup = popup;
            _isShowing = true;

            try
            {
                await MopupService.Instance.PushAsync(popup);
            }
            catch
            {
                _loaderPopup = null;
                _isShowing = false;
                throw;
            }
        });
    }

    public static async Task HideAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var popup = _loaderPopup;

            // Reset our state first
            _loaderPopup = null;
            _isShowing = false;

            if (popup == null)
                return;

            try
            {
                // Check if popup is still in the stack
                if (MopupService.Instance.PopupStack.Contains(popup))
                {
                    await MopupService.Instance.RemovePageAsync(popup);
                }
            }
            catch (InvalidOperationException)
            {
                // Popup was already removed, for example by Android Back button.
            }
        });
    }
}
