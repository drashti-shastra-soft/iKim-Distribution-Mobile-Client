using i_KimMobileClient.APIService;
using i_KimMobileClient.Pages.Accounts;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using System.Diagnostics;
using System.Text.Json;

namespace i_KimMobileClient.Views;

public partial class ProfileView : ContentView
{
    private bool _isProcessingOption;
    public ProfileView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        LblLoginUserName.Text = $"{Common.mUser.FirstName} {Common.mUser.LastName}";
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
    }
    //private async void OptionSelection_Tapped(object sender, TappedEventArgs e)
    //{
    //    if (sender is Border bdrOption)
    //    {
    //        if (bdrOption.ClassId == "ChangePassword")
    //        {
    //            await Navigation.PushAsync(new ChangePasswordPage());

    //        }
    //        else if (bdrOption.ClassId == "Address")
    //        {

    //            if (!await NetworkService.CheckInternetAsync())
    //                return ;

    //            var data = await GetAddress();
    //            await Navigation.PushAsync(new AddressPage(data));
    //            //var addressPopupPage = new AddressPopupPage(data);
    //            //addressPopupPage.AddressEvent += async (s1, e1) =>
    //            //{
    //            //    await MopupService.Instance.PopAsync();
    //            //};
    //            //await MopupService.Instance.PushAsync(addressPopupPage);
    //        }
    //        else if (bdrOption.ClassId == "Logout")
    //        {
    //            var data = new CommonPopupPage();
    //            data.logoutEvent += async (s, e) =>
    //            {
    //                await MopupService.Instance.PopAsync();
    //                await SessionManager.LogoutAsync();
    //                //Application.Current.MainPage = new NavigationPage(new LoginPage());
    //            };
    //            await MopupService.Instance.PushAsync(data);
    //        }
    //        else if (bdrOption.ClassId == "Clear")
    //        {
    //            var data = new CommonPopupPage(isClear: true);
    //            data.logoutEvent += async (s, e) =>
    //            {
    //                //CategoryCacheService.Clear();
    //                //ItemCacheService.Clear();
    //                //await MopupService.Instance.PopAsync();

    //                try
    //                {
    //                    CategoryCacheService.Clear();
    //                    ItemCacheService.Clear();

    //                    await MopupService.Instance.PopAsync();

    //                    Common.SuccessMessage("Data cleared successfully!");
    //                }
    //                catch (Exception ex)
    //                {
    //                    await MopupService.Instance.PopAsync();

    //                    Common.ErrorMessage("Failed to clear data.");
    //                }

    //            };
    //            await MopupService.Instance.PushAsync(data);

    //        }
    //    }
    //}


    private async void OptionSelection_Tapped(object sender, TappedEventArgs e)
    {
        if (_isProcessingOption)
            return;

        if (sender is not Border bdrOption)
            return;

        _isProcessingOption = true;

        try
        {
            // Optional: disable all option borders visually/functionally
            // bdrOption.IsEnabled = false;

            if (bdrOption.ClassId == "ChangePassword")
            {
                await Navigation.PushAsync(new ChangePasswordPage());
            }
            else if (bdrOption.ClassId == "Address")
            {
                if (!await NetworkService.CheckInternetAsync())
                    return;

                var data = await GetAddress();
                await Navigation.PushAsync(new AddressPage(data));
            }
            else if (bdrOption.ClassId == "Logout")
            {
                var data = new CommonPopupPage();

                data.logoutEvent += async (s, e) =>
                {
                    await MopupService.Instance.PopAsync();
                    await SessionManager.LogoutAsync();
                };

                await MopupService.Instance.PushAsync(data);
            }
            else if (bdrOption.ClassId == "Clear")
            {
                var data = new CommonPopupPage(isClear: true);

                data.logoutEvent += async (s, e) =>
                {
                    try
                    {
                        CategoryCacheService.Clear();
                        ItemCacheService.Clear();

                        await MopupService.Instance.PopAsync();

                        Common.SuccessMessage("Data cleared successfully!");
                    }
                    catch (Exception)
                    {
                        await MopupService.Instance.PopAsync();

                        Common.ErrorMessage("Failed to clear data.");
                    }
                };

                await MopupService.Instance.PushAsync(data);
            }
        }
        finally
        {
            _isProcessingOption = false;
        }
    }
    private async Task<Models.Address> GetAddress()
    {
        var address = new Models.Address();
        try
        {
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetAddress, Common.mUser.StoreId);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<Models.Address>(url, Common.mUser.Token);

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null && mResponse.Value is Models.Address mAddress)
                {

                    address = mAddress;

                    //Debug.WriteLine(JsonSerializer.Serialize(mAddress, new JsonSerializerOptions
                    //{
                    //    WriteIndented = true
                    //}));
                }
            }
            await PopLoaderService.HideAsync();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"GetAddress: {ex.Message}");
        }
        return address;
    }
    
}