using Android.App;
using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;

namespace i_KimMobileClient.Pages.Notifications;

public partial class NotificationListPage : ContentPage
{
    private List<Models.Notification> _allNotification = new();
    public NotificationListPage()
    {
        InitializeComponent();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        NotificationRefreshView.IsRefreshing = true;
        await GetNotification();
        NotificationRefreshView.IsRefreshing = false;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetNotification();

    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async Task GetNotification()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetNotification, Common.mUser.Id);
            var mResponse = await new ApiServiceHandler().GetAsync<List<Models.Notification>>(url, Common.mUser.Token);
            //if (mResponse != null && mResponse.IsSuccess)
            //{
            //    if (mResponse.Value != null &&
            //        mResponse.Value is List<Models.Notification> notifications &&
            //        notifications.Count > 0)
            //    {
            //        for (int i = 0; i < notifications.Count; i++)
            //            notifications[i].Index = i;

            //        _allNotification = notifications;

            //        clsNotificationItemList.ItemsSource = _allNotification;

            //    }
            //    else
            //    {
            //        clsNotificationItemList.ItemsSource = null;
            //    }
            //}
            //else
            //{
            //    Common.ErrorMessage(mResponse.ErrorMessage);
            //}

            if (mResponse != null && mResponse.IsSuccess)
            {
                _allNotification = new List<Models.Notification>();

                if (mResponse.Value != null &&
                    mResponse.Value is List<Models.Notification> notifications &&
                    notifications.Count > 0)
                {
                    notifications = notifications
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();

                    string previousHeader = "";

                    for (int i = 0; i < notifications.Count; i++)
                    {
                        notifications[i].Index = i;

                        notifications[i].Title = notifications[i].Title?.Trim();
                        notifications[i].Description = notifications[i].Description?.Trim();
                        string currentHeader;

                        if (notifications[i].CreatedDate.Date == DateTime.Today)
                            currentHeader = "Today";
                        else if (notifications[i].CreatedDate.Date == DateTime.Today.AddDays(-1))
                            currentHeader = "Yesterday";
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

                clsNotificationItemList.ItemsSource = _allNotification;
            }
            else
            {
                clsNotificationItemList.ItemsSource = new List<Models.Notification>();
                Common.ErrorMessage(mResponse.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

    private async void OnNotificationTapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grid &&
        grid.BindingContext is Models.Notification notification)
        {
            await Navigation.PushAsync(new NotificationDetailPage(notification));
        }
    }
}



//notification.IsRead = true;

//// Refresh the UI
//clsNotificationItemList.ItemsSource = null;
//clsNotificationItemList.ItemsSource = _allNotification;