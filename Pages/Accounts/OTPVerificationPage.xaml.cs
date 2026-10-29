using System.Threading.Tasks;
using i_KimMobileClient.Utilities;

namespace i_KimMobileClient.Pages.Accounts;

public partial class OTPVerificationPage : ContentPage
{

    public OTPVerificationPage(string emailAddress)
    {
        InitializeComponent();
        txtEmail.Text = emailAddress;
    }

    private async void BtnBack_tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(imgBack);
        await Navigation.PopAsync();
    }

    private async void BtnVerifyCode_Tapped(object sender, EventArgs e)
    {
        try
        {
            await Common.ControlBounceEffect(btnVerify);

            string otp = GetOtpValue();

            if (otp.Length < 4)
            {
                Common.ErrorMessage("Please enter all 4 digits");
                return;
            }
            // Your OTP verification logic here
            Common.SuccessMessage($"Entered OTP: {otp}");
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }

    private void OnOtpTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (sender is not Entry currentEntry) return;

            // Move forward when a digit is entered
            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                if (currentEntry == Otp1) Otp2.Focus();
                else if (currentEntry == Otp2) Otp3.Focus();
                else if (currentEntry == Otp3) Otp4.Focus();
                else if (currentEntry == Otp4) Otp4.Unfocus(); // last box, dismiss keyboard
            }
            // Move backward when deleted (backspace)
            else if (string.IsNullOrEmpty(e.NewTextValue))
            {
                if (currentEntry == Otp4) Otp3.Focus();
                else if (currentEntry == Otp3) Otp2.Focus();
                else if (currentEntry == Otp2) Otp1.Focus();
            }
        }
        catch (Exception ex)

        {
            _ = ex.Message;
        }
    }

    private string GetOtpValue()
    {
        return $"{Otp1.Text}{Otp2.Text}{Otp3.Text}{Otp4.Text}";
    }
}