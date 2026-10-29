using Mopups.Pages;
using Mopups.Services;

namespace i_KimMobileClient.PopupPages;

public partial class CommonAlertPopupPage : PopupPage
{
    public event EventHandler ActionEvent;

    public CommonAlertPopupPage(string title, string message, string buttonText = "OK")
    {
        InitializeComponent();

        lblTitle.Text = title;
        lblMessage.Text = message;
        btnAction.Text = buttonText;
    }
    private async void BtnAction(object sender, EventArgs e)
    {
        ActionEvent?.Invoke(this, EventArgs.Empty);
        await MopupService.Instance.PopAsync();
    }

    private async void BtnClose_Clicked(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
}