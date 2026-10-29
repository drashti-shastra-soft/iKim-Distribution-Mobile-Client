using CommunityToolkit.Maui.Behaviors;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using System.Threading.Tasks;

namespace i_KimMobileClient.Pages.Accounts;

public partial class ChangePasswordPage : ContentPage
{
    private bool _isNewPasswordVisible = false;
    private bool _isConfirmPasswordVisible = false;
    public ChangePasswordPage()
    {
        InitializeComponent();
    }
    private bool ValidateControl()
    {
        bool isValid = false;
        if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
        {
            Common.ErrorMessage("Please enter current password!");
        }
        else if (string.IsNullOrWhiteSpace(EntryNewPassword.Text) || string.IsNullOrWhiteSpace(EntryConfirmPassword.Text))
        {
            Common.ErrorMessage("Please enter new password and confirm password!");
        }
        else if (string.IsNullOrWhiteSpace(EntryNewPassword.Text))
        {
            Common.ErrorMessage("Please enter new password!");
        }
        else if (string.IsNullOrWhiteSpace(EntryConfirmPassword.Text))
        {
            Common.ErrorMessage("Please enter confirm password!");
        }
        else if (EntryNewPassword.Text != EntryConfirmPassword.Text)
        {
            Common.ErrorMessage("Not match new password and confirm password!");
        }
        else
        {
            isValid = true;
        }

        return isValid;
    }
    private void OnToggleCurrentPassword(object sender, TappedEventArgs e)
    {
        bool isHidden = iconCurrentPassword.Source?.ToString().Contains("icon_unhide.svg") == true;
        iconCurrentPassword.Source = isHidden ? "icon_hide.svg" : "icon_unhide.svg";
        txtCurrentPassword.IsPassword = isHidden;
    }
    private void OnToggleNewPassword(object sender, EventArgs e)
    {
        bool isHidden = IconNewPassword.Source?.ToString().Contains("icon_unhide.svg") == true;
        IconNewPassword.Source = isHidden ? "icon_hide.svg" : "icon_unhide.svg";
        EntryNewPassword.IsPassword = isHidden;
    }

    private void OnToggleConfirmPassword(object sender, EventArgs e)
    {
        bool isHidden = IconConfirmPassword.Source?.ToString().Contains("icon_unhide.svg") == true;
        IconConfirmPassword.Source = isHidden ? "icon_hide.svg" : "icon_unhide.svg";
        EntryConfirmPassword.IsPassword = isHidden;
    }

    private async void BtnResetPassword_Tapped(object sender, EventArgs e)
    {

        try
        {

            if (!await NetworkService.CheckInternetAsync())
                return;

            await Common.ControlBounceEffect(sender);
            await ProcessToChangePassword();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"ForgotPasswordPage: {ex.Message}");
        }
    }

    private async Task ProcessToChangePassword()
    {
        try
        {
            if (!ValidateControl())
                return;


            string currentPassword = txtCurrentPassword.Text?.Trim() ?? "";
            string newPassword = EntryNewPassword.Text?.Trim() ?? "";
            string confirmPassword = EntryConfirmPassword.Text?.Trim() ?? "";

            ChangePassword mChangePassword = new ChangePassword
            {
                UserId = Common.mUser.Id,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword

            };
            await PopLoaderService.ShowAsync();
            var mResponse = await new APIService.ApiServiceHandler().PutAsync<ChangePassword>(EndPointsUrl.ChangePassword, mChangePassword, Common.AuthToken);
            if (mResponse == null || !mResponse.IsSuccess)
            {
                Common.ErrorMessage(mResponse.ErrorMessage);
                return;
            }

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Password has been chnaged Successfully");
                ClearControl();
                await Navigation.PopAsync();
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

    private void ClearControl()
    {
        txtCurrentPassword.Text = string.Empty;
        EntryNewPassword.Text = string.Empty;
        EntryConfirmPassword.Text = string.Empty;
    }

    private void OnBackTapped(object sender, TappedEventArgs e)
    {
        Navigation.PopAsync();
    }

    
}
