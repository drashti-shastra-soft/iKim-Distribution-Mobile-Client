using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using System.Net.Mail;
using System.Threading.Tasks;

namespace i_KimMobileClient.Pages.Accounts;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void BdrBack_Tapped(object sender, TappedEventArgs e)
    {
        //await Navigation.PopAsync();
        await Navigation.PushAsync(new ChangePasswordPage());
    }

    private bool ValidateControl()
    {
        bool isValid = false;
        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            Common.ErrorMessage("Please enter email address!");
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
    private async void BtnSendCode_Tapped(object sender, EventArgs e)
    {
        try
        {

            if (!await NetworkService.CheckInternetAsync())
                return;

            await Common.ControlBounceEffect(sender);
            await ProcessToForgotPassword();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"ForgotPasswordPage: {ex.Message}");
        }

    }


    private async Task ProcessToForgotPassword()
    {
        try
        {
            if (!ValidateControl())
                return;

            ForgotPassword mForgotPassword = new ForgotPassword
            {
                EmailAddress = txtEmail.Text
            };
            await PopLoaderService.ShowAsync();
            var mResponse = await new APIService.ApiServiceHandler().PostAsync<ForgotPassword>(EndPointsUrl.ForgotPassword, mForgotPassword);
            if (mResponse == null || !mResponse.IsSuccess)
            {
                Common.ErrorMessage(mResponse.ErrorMessage);
                return;
            }

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Success");
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


    private void BtnBack_tapped(object sender, TappedEventArgs e)
    {
        Navigation.PopAsync();
    }
}