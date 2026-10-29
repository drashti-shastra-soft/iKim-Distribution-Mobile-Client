using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using static Android.Provider.Telephony.Mms;

namespace i_KimMobileClient.Pages.Notifications;

public partial class NotificationDetailPage : ContentPage
{
    private Notification _notification;

    public string TitleText => _notification?.Title.Trim();
    public string DescriptionText => _notification?.Description.Trim();
    public NotificationDetailPage(Notification notification)
	{
		InitializeComponent();
        _notification = notification;
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_notification.IsRead)
        {
            await MarkAsRead();
        }
    }
    private async Task MarkAsRead()
    {
        try
        {
            await PopLoaderService.ShowAsync();

            _notification.IsRead = true;
            _notification.LastModifiedBy = Common.mUser.Id;

            string url = string.Format(EndPointsUrl.MarkNotificationAsRead, _notification.Id, Common.mUser.Id);
            var mResponse = await new APIService.ApiServiceHandler().PatchAsync<Notification>(url, _notification, Common.mUser.Token);
            
        }
        catch (Exception ex)
        {
            // Handle or log exception
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

    private async Task MarkAsRead1()
    {
        try
        {
            await PopLoaderService.ShowAsync();

            _notification.IsRead = true;
            _notification.LastModifiedBy = Common.mUser.Id;

            string url = string.Format(
                EndPointsUrl.MarkNotificationAsRead,
                _notification.Id,
                Common.mUser.Id);

            var response = await new APIService.ApiServiceHandler()
                .PostAsync<Notification>(url, _notification, Common.mUser.Token);
        }
        catch (Exception ex)
        {
            // Log exception
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }
    private async void BackButton_Clicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
}