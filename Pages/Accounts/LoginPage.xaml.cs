using i_KimMobileClient.Models;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Microsoft.Maui;
using Mopups.Services;
using SQLitePCL;
using System.Diagnostics;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace i_KimMobileClient.Pages.Accounts;

public partial class LoginPage : ContentPage
{
    public bool isRememberMe = false;
    public LoginPage()
    {
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        isRememberMe = AppSetting.GetRememberMe();
        if (isRememberMe)
        {
            txtEmail.Text = AppSetting.GetEmail();
            TxtPassword.Text = AppSetting.GetPassword();
            imgCheckBox.Source = AppConstraints.Check;
        }
        else
        {
            txtEmail.Text = string.Empty;
            TxtPassword.Text = string.Empty;
            imgCheckBox.Source = AppConstraints.UnCheck;
        }
    }
    private bool ValidateControl()
    {
        bool isValid = false;
        if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(TxtPassword.Text))
        {
            Common.ErrorMessage("Please enter username and password!");
        }
        else if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            Common.ErrorMessage("Please enter email address!");
        }
        else if (string.IsNullOrWhiteSpace(TxtPassword.Text))
        {
            Common.ErrorMessage("Please enter password!");
        }
        else if (!IsValidEmail(txtEmail.Text.Trim()))
        {
            Common.ErrorMessage("Please enter a valid email address!");
        }
        else
        {
            isValid = true;
        }

        return isValid;
    }

    //private bool IsValidEmail(string email)
    //{
    //    try
    //    {
    //        var mailAddress = new MailAddress(email);
    //        return mailAddress.Address == email;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}

 
    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);

            return mailAddress.Address == email &&
                   mailAddress.Host.Contains('.') &&
                   !mailAddress.Host.EndsWith('.');
        }
        catch
        {
            return false;
        }
    }
    private bool IsValidEmail1(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            var popup = new CommonPopupPage(isExit: true);

            popup.logoutEvent += async (s, e) =>
            {
                await MopupService.Instance.PopAsync();

#if ANDROID
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#endif
            };

            await MopupService.Instance.PushAsync(popup);
        });

        return true;
    }
    private async void LblForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ForgotPasswordPage());
    }

    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        try
        {

            if (!await NetworkService.CheckInternetAsync())
                return;

            await Common.ControlBounceEffect(sender);
            //await Navigation.PushAsync(new HomeTabbedPage());
            await ProcessToLogin();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"LoginPage/BtnLogin_Clicked: {ex.Message}");
        }
    }

    private void StkRememberMe_Tapped(object sender, TappedEventArgs e)
    {
        bool isChecked = imgCheckBox.Source?.ToString().Contains("uncheck") == true;
        imgCheckBox.Source = isChecked ? "check" : "uncheck";
        isRememberMe = isChecked;
    }

    private async Task ProcessToLogin()
    {
        try
        {
            if (!ValidateControl())
                return;

            var mCredential = new Models.Credential
            {
                EmailAddress = txtEmail.Text?.Trim(),
                Password = TxtPassword.Text?.Trim()
            };
            if (mCredential != null)
            {
                await PopLoaderService.ShowAsync();
                var mResponce = await new APIService.ApiServiceHandler().PostAsync<Models.Credential, Models.User>(EndPointsUrl.Authentication, mCredential);
                //if (mResponce == null && !mResponce.IsSuccess)
                //    return; Common.ErrorMessage(mResponce.ErrorMessage);

                if (mResponce == null || !mResponce.IsSuccess)
                {
                    Common.ErrorMessage(mResponce.ErrorMessage);
                    return;
                }

                await PopLoaderService.HideAsync();

                if (mResponce != null && mResponce.IsSuccess)
                {
                    //Debug.WriteLine(JsonSerializer.Serialize(mResponce.Value, new JsonSerializerOptions
                    //{
                    //    WriteIndented = true
                    //}));
                    if (mResponce.Value is Models.User mUser)
                    {
                        // Only UserTypeId = 3 is allowed to login
                        if (mUser.UserTypeId != 3)
                        {
                            Common.ErrorMessage("You are not authorized to login.");
                            return;
                        }

                        Common.mUser = mUser;
                        if (!string.IsNullOrEmpty(Common.mUser.Token))
                            Common.AuthToken = Common.mUser.Token;
                        //Common.AuthToken = "InValid Token";

                        // Load Item cache
                        await ItemCacheService.LoadAsync();

                        if (!ItemCacheService.IsForUser(Common.mUser.Id))
                        {
                            ItemCacheService.Clear();
                        }


                        // Load Category cache
                        await CategoryCacheService.LoadAsync();

                        if (!CategoryCacheService.IsForUser(Common.mUser.Id))
                        {
                            CategoryCacheService.Clear();
                        }


                        if (isRememberMe)
                        {
                            AppSetting.SetEmail(txtEmail.Text);
                            AppSetting.SetPassword(TxtPassword.Text);
                        }
                        else
                        {
                            AppSetting.SetEmail(string.Empty);
                            AppSetting.SetPassword(string.Empty);
                        }

                        AppSetting.SetRememberMe(isRememberMe);
                        await Navigation.PushAsync(new HomeTabbedPage());
                    }
                }
               
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
        finally 
        {
            await PopLoaderService.HideAsync();
        }
    }

    private async void LblSignup_tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ChangePasswordPage());
    }

    private void ImgIsPasswordShow_Tapped(object sender, TappedEventArgs e)
    {
        bool isHidden = ImgEye.Source?.ToString().Contains("icon_unhide.svg") == true;
        ImgEye.Source = isHidden ? "icon_hide.svg" : "icon_unhide.svg";
        TxtPassword.IsPassword = isHidden;
    }
}