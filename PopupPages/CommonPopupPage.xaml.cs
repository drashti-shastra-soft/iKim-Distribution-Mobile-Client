using Mopups.Pages;
using Mopups.Services;

namespace i_KimMobileClient.PopupPages;

public partial class CommonPopupPage : PopupPage
{
    public event EventHandler logoutEvent;
    public CommonPopupPage(bool isDelete = false, bool isExit =  false,bool isClear = false)
    {
        InitializeComponent();

        if (isDelete)
            lblMessage.Text = "Are you sure want to delete this item?";
        else
            lblMessage.Text = "Are you sure want to logout?";

        if (isExit)
        {
            lblTitle.Text = "Exit App";
            lblMessage.Text = "Are you sure you want to exit?";
            //lblMessage.TextColor = Color.FromArgb("#000000");
            lblMessage.TextColor = Colors.Black;
        }

        if (isClear)
        {
            lblTitle.Text = "Clear Cache";
            lblMessage.Text = "Are you sure you want to clear the cache?";
            lblMessage.TextColor = Colors.Black;
        }


    }

    private async void BtnClose_Cliked(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }

    private async void BtnAction(object sender, EventArgs e)
    {
        if (sender is Button btnAction)
        {
            if (btnAction.ClassId == "Yes")
            {
                logoutEvent?.Invoke(this, EventArgs.Empty);
            }
            else if (btnAction.ClassId == "No")
            {
                await MopupService.Instance.PopAsync();
            }
        }
    }
}