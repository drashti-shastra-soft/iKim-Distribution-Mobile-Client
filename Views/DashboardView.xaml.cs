using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Pages.Daily_Sales;
using i_KimMobileClient.Pages.Notifications;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Views;

public partial class DashboardView : ContentView, INotifyPropertyChanged
{
    private System.Timers.Timer? _autoScrollTimer;
    private int _currentIndex = 0;
    private int _totalItems = 3;
    public List<Banner> _mBanner = new();
    private int _currentPosition = 0;

    private List<Models.Data> _allSalesData = new();

    // ---- Bindable properties ----
    private string _totalSalesDisplay = "0.00";
    public string TotalSalesDisplay
    {
        get => _totalSalesDisplay;
        set { _totalSalesDisplay = value; OnPropertyChanged(); }
    }

    private string _percentChangeDisplay = "0.0%";
    public string PercentChangeDisplay
    {
        get => _percentChangeDisplay;
        set { _percentChangeDisplay = value; OnPropertyChanged(); }
    }

    private string _periodComparisonLabel = "from last week";
    public string PeriodComparisonLabel
    {
        get => _periodComparisonLabel;
        set { _periodComparisonLabel = value; OnPropertyChanged(); }
    }

    private Color _percentBadgeColor = Color.FromArgb("#FFCDD2");
    public Color PercentBadgeColor
    {
        get => _percentBadgeColor;
        set { _percentBadgeColor = value; OnPropertyChanged(); }
    }

    private Color _percentTextColor = Color.FromArgb("#D32F2F");
    public Color PercentTextColor
    {
        get => _percentTextColor;
        set { _percentTextColor = value; OnPropertyChanged(); }
    }

    private List<ChartPoint> _chartEntries = new();
    public List<ChartPoint> ChartEntries
    {
        get => _chartEntries;
        set { _chartEntries = value; OnPropertyChanged(); }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public DashboardView()
    {
        InitializeComponent();
        BindingContext = this;
        StartAutoScroll();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    //private async void OnLoaded(object? sender, EventArgs e)
    //{
    //    if (!await NetworkService.CheckInternetAsync())
    //        return;

    //    //if (!Common.CheckInternetConnection())
    //    //{
    //    //    Common.InfoMessage("Please check your internet connection and try again.");
    //    //    return;
    //    //}
    //    lblUserName.Text = $"{Common.mUser.FirstName}";
    //    await CheckPendingSales();
    //    await GetCartItemCount();
    //    await GetActiveBanner();
    //    await GetNotificationCount();
    //    await GetSalesData();

    //    // Default selection: This Week (index 1 in the Picker items)
    //    periodPicker.SelectedIndex = 2;
    //}

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (!await NetworkService.CheckInternetAsync())
            return;

        try
        {
            await PopLoaderService.ShowAsync();

            lblUserName.Text = $"{Common.mUser.FirstName}";

            // Show only for User
            //bool isUser = Common.mUser.UserTypeId == 0;

            //bdrNotification.IsVisible = isUser;
            //bdrNotificationCount.IsVisible = isUser;

            //await CheckPendingSales();
            await GetNotificationCount();
            await GetCartItemCount();
            await GetActiveBanner();
            await GetSalesData();

            periodPicker.SelectedIndex = 2;
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
    }

    private void StartAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (bannerCarousel.ItemsSource is IList items && items.Count > 0)
            {
                _currentPosition++;
                if (_currentPosition >= items.Count)
                    _currentPosition = 0;

                bannerCarousel.Position = _currentPosition;
            }
            return true;
        });
    }

    private async Task CheckPendingSales()
    {
        try
        {
            var url = string.Format(
                EndPointsUrl.CheckPendingSales,
                Common.mUser.StoreId.Value);

            var mResponse = await new ApiServiceHandler()
                .GetAsync<DailySales>(url, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess && mResponse.Value != null)
            {
                var dailySales = JsonConvert.DeserializeObject<DailySales>(
                    JsonConvert.SerializeObject(mResponse.Value));

                DateTime lastSalesDate = dailySales.Date.Date;

                if (lastSalesDate.AddDays(1) < DateTime.Today)
                {
                    DateTime pendingDate = lastSalesDate.AddDays(1);

                    //await Application.Current.MainPage.DisplayAlert(
                    //    "Alert",
                    //    $"You haven’t entered Cash & UPI Amount for {pendingDate:dd-MM-yyyy}. Please add the details to continue.",
                    //    "OK");

                    //await Navigation.PushAsync(
                    //    new AddDailySalesPage(pendingDate));

                    var tcs = new TaskCompletionSource<bool>();
                    var popup = new CommonAlertPopupPage(
                        "Alert",
                        $"You haven’t entered Cash & UPI Amount for {pendingDate:dd/MM/yyyy}. Please add the details to continue.",
                        "OK");

                    popup.ActionEvent += async (s, e) =>
                    {
                        await MopupService.Instance.PopAsync();
                        await Navigation.PushAsync(new AddDailySalesPage(pendingDate,true));
                        tcs.TrySetResult(true);
                    };

                    await MopupService.Instance.PushAsync(popup);
                    // Wait until user clicks OK
                    await tcs.Task;

                }
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }

    private async Task GetSalesData()
    {
        try
        {
            string url = string.Format(EndPointsUrl.SealsChart, Common.mUser.StoreId.Value);
            var mResponse = await new APIService.ApiServiceHandler()
                .PostAsync<List<Models.Data>>(url, null, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess &&
                mResponse.Value is List<Models.Data> mSalesData &&
                mSalesData.Count > 0)
            {
                _allSalesData = mSalesData;
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"Dashboard/GetSalesData : {ex.Message}");
        }
    }

    public async Task<string> GetCartItemCount()
    {
        string count = string.Empty;
        try
        {
            string url = string.Format(EndPointsUrl.TotalCartItemCount, Common.mUser.StoreId);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<int>(url, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                count = mResponse.Value.ToString();
                Common.CartItemCount = count;
            }
            return count;
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"Dashboard/GetCartItemCount : {ex.Message}");
            return string.Empty;
        }
    }

    private async Task GetActiveBanner()
    {
        try
        {
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<Banner>>(EndPointsUrl.GetAllActiveBanner, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is List<Banner> mobjItems &&
                    mobjItems.Count > 0)
                {
                    _mBanner = mobjItems.OrderBy(x => x.Sequence).ToList();
                    bannerCarousel.ItemsSource = _mBanner;
                }
            }
            else
            {
                Common.ErrorMessage(mResponse.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"{ex.Message}");
        }
    }

    private async Task GetNotificationCount()
    {
        try
        {
            string url = string.Format(EndPointsUrl.GetNotification,Common.mUser.Id);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<Models.Notification>>(url, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                //Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(mResponse.Value, new JsonSerializerOptions
                //{
                //    WriteIndented = true
                //}));
                if (mResponse.Value is List<Models.Notification> notifications)
                {
                    //Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(notifications, new JsonSerializerOptions
                    //{
                    //    WriteIndented = true
                    //}));

                    //lblNotification.Text = notifications.Count.ToString();
                    int unreadCount = notifications.Count(n => !n.IsRead);

                    lblNotification.Text = unreadCount.ToString();

                    //bdrNotification.IsEnabled = notifications.Count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"{ex.Message}");
        }
    }

    private void PeriodPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_allSalesData == null || _allSalesData.Count == 0 || periodPicker.SelectedIndex < 0)
            return;

        var period = periodPicker.SelectedIndex switch
        {
            0 => SalePeriod.ThisDay,
            1 => SalePeriod.ThisWeek,
            2 => SalePeriod.ThisMonth,
            _ => SalePeriod.ThisWeek
        };

        ApplyPeriodFilter(period);
    }

    private void ApplyPeriodFilter(SalePeriod period)
    {
        double currentTotal;
        double previousTotal;
        string comparisonLabel;
        List<ChartPoint> chartPoints;

        switch (period)
        {
            case SalePeriod.ThisMonth:
                var ordered = _allSalesData.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
                var current = ordered.LastOrDefault();
                var previous = ordered.Count > 1 ? ordered[^2] : null;
                currentTotal = (double)(current?.Sales ?? 0);
                previousTotal = (double)(previous?.Sales ?? 0);
                comparisonLabel = "from last month";
                chartPoints = ordered
                    .Select(x => new ChartPoint { Label = x.MonthString, Value = (double)x.Sales })
                    .ToList();
                break;

            case SalePeriod.ThisWeek:
                currentTotal = SumForDateRange(StartOfWeek(DateTime.Today), DateTime.Today);
                previousTotal = SumForDateRange(StartOfWeek(DateTime.Today).AddDays(-7), StartOfWeek(DateTime.Today).AddDays(-1));
                comparisonLabel = "from last week";
                chartPoints = BuildDailyChartPoints(StartOfWeek(DateTime.Today), DateTime.Today);
                break;

            case SalePeriod.ThisDay:
                currentTotal = SumForDateRange(DateTime.Today, DateTime.Today);
                previousTotal = SumForDateRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));
                comparisonLabel = "from yesterday";
                chartPoints = BuildDailyChartPoints(DateTime.Today.AddDays(-6), DateTime.Today);
                break;

            default:
                currentTotal = 0;
                previousTotal = 0;
                comparisonLabel = "";
                chartPoints = new List<ChartPoint>();
                break;
        }

        double percentChange = previousTotal == 0
            ? (currentTotal > 0 ? 100 : 0)
            : ((currentTotal - previousTotal) / previousTotal) * 100;

        bool isPositive = percentChange >= 0;

        TotalSalesDisplay = currentTotal.ToString("N2");
        PercentChangeDisplay = $"{Math.Abs(percentChange):0.0}%";
        PeriodComparisonLabel = comparisonLabel;
        PercentBadgeColor = isPositive ? Color.FromArgb("#C8E6C9") : Color.FromArgb("#FFCDD2");
        PercentTextColor = isPositive ? Color.FromArgb("#2E7D32") : Color.FromArgb("#D32F2F");
        ChartEntries = chartPoints;
    }

    private double SumForDateRange(DateTime start, DateTime end)
    {
        // Placeholder — requires a `DateTime Date` field on Models.Data and
        // a day-level API. Wire it up like:
        // return _allSalesData.Where(x => x.Date.Date >= start.Date && x.Date.Date <= end.Date).Sum(x => x.Sales);
        return 0;
    }

    private List<ChartPoint> BuildDailyChartPoints(DateTime start, DateTime end)
    {
        // Placeholder — bucket day-level sales data once available:
        // return _allSalesData
        //     .Where(x => x.Date.Date >= start.Date && x.Date.Date <= end.Date)
        //     .GroupBy(x => x.Date.Date)
        //     .OrderBy(g => g.Key)
        //     .Select(g => new ChartPoint { Label = g.Key.ToString("dd-MM"), Value = g.Sum(x => x.Sales) })
        //     .ToList();
        return new List<ChartPoint>();
    }

    private static DateTime StartOfWeek(DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            _autoScrollTimer?.Stop();
            _autoScrollTimer?.Dispose();
        }
        else
        {
            StartAutoScroll();
        }
    }

    public void ResumeAutoScroll() => _autoScrollTimer?.Start();
    public void PauseAutoScroll() => _autoScrollTimer?.Stop();

    private async void OnRefreshing(object sender, EventArgs e)
    {
        OrderRefreshView.IsRefreshing = true;
        await PopLoaderService.ShowAsync();
        await Task.Delay(3000);
        await PopLoaderService.HideAsync();
        OrderRefreshView.IsRefreshing = false;
    }



    private async void BdrDailySales_Tapped(object sender, TappedEventArgs e)
    {
        await PopLoaderService.ShowAsync();
        await Navigation.PushAsync(new DailySalesListPage());
    }
    private async void BdrQuickAction_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            if (sender is not VerticalStackLayout tapped) return;

            var border = tapped.Children.OfType<Border>().FirstOrDefault();
            if (border != null)
            {
                await border.ScaleTo(0.9, 80);
                await border.ScaleTo(1.0, 80);
            }

            var tab = tapped.ClassId switch
            {
                "New Order" => TabType.Add,
                "View Orders" => TabType.Orders,
                "PartyOrders" => TabType.Delivery,
                _ => TabType.Home
            };

            await GetHomeTabbedPage()?.SwitchTab(tab)!;
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
    }

    private HomeTabbedPage? GetHomeTabbedPage()
    {
        Element? parent = this.Parent;
        while (parent != null)
        {
            if (parent is HomeTabbedPage page)
                return page;
            parent = parent.Parent;
        }
        return null;
    }

    private async void BdrNotification_Tapped(object sender, TappedEventArgs e)
    {
        //await Navigation.PushAsync(new NotificationListPage());

        var popup = new NotificationPopupPage();

        popup.ViewAllClicked += async (s,e) =>
        {
            await MopupService.Instance.PopAsync();
            await Navigation.PushAsync(new NotificationListPage());
        };

        popup.NotificationClicked += async (s, notification) =>
        {
            await MopupService.Instance.PopAsync();

            await Navigation.PushAsync(new NotificationDetailPage(notification));
        };

        await MopupService.Instance.PushAsync(popup);

    }

    private void OnPickerTapped(object sender, TappedEventArgs e)
    {
        periodPicker.Focus();
    }
}

public enum SalePeriod { ThisDay, ThisWeek, ThisMonth }

public class ChartPoint
{
    public string Label { get; set; }
    public double Value { get; set; }
}