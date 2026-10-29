using Android.Webkit;
using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Pages.Notifications;
using i_KimMobileClient.Utilities;
using Mopups.Pages;
using Mopups.Services;

namespace i_KimMobileClient.PopupPages;

public partial class NotificationPopupPage : PopupPage
{
    public event EventHandler? ViewAllClicked;
    public event EventHandler<Models.Notification>? NotificationClicked;
    private List<Notification> _allNotification = new();
    public NotificationPopupPage()
	{
		InitializeComponent();
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetNotification();
    }
    private async Task GetNotification()
    {
        try
        {
            string url = string.Format(EndPointsUrl.GetNotification, Common.mUser.Id);
            var mResponse = await new ApiServiceHandler().GetAsync<List<Notification>>(url, Common.mUser.Token);
            
            if (mResponse != null && mResponse.IsSuccess)
            {
                _allNotification = new List<Models.Notification>();

                if (mResponse.Value != null &&
                    mResponse.Value is List<Models.Notification> notifications &&
                    notifications.Count > 0)
                {
                    notifications = notifications
                        .OrderByDescending(x => x.CreatedDate)
                        .Take(2)
                        .ToList();

                    string previousHeader = "";

                    for (int i = 0; i < notifications.Count; i++)
                    {
                        notifications[i].Index = i;

                        notifications[i].Title = notifications[i].Title?.Trim();
                        notifications[i].Description = notifications[i].Description?.Trim();

                        string currentHeader;

                        if (notifications[i].CreatedDate.Date == DateTime.Today)
                            currentHeader = "TODAY";
                        else if (notifications[i].CreatedDate.Date == DateTime.Today.AddDays(-1))
                            currentHeader = "YESTERDAY";
                        else
                            currentHeader = notifications[i].CreatedDate.ToString("dd MMM yyyy");

                        notifications[i].HeaderText = currentHeader;

                        if (currentHeader != previousHeader)
                        {
                            notifications[i].ShowHeader = true;
                            previousHeader = currentHeader;
                        }

                        _allNotification.Add(notifications[i]);
                    }
                }

                clsNotificationList.ItemsSource = _allNotification;
            }
            else
            {
                clsNotificationList.ItemsSource = new List<Models.Notification>();
                Common.ErrorMessage(mResponse.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"{ex.Message}");
        }
    }

    private async void ViewAll_Tapped(object sender, TappedEventArgs e)
    {
        ViewAllClicked?.Invoke(this, EventArgs.Empty);
    }

    private async void OnNotificationTapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grid &&
            grid.BindingContext is Models.Notification notification)
        {
            NotificationClicked?.Invoke(this, notification);
        }
    }
}