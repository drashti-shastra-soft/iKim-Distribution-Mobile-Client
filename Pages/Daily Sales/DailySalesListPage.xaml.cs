using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Newtonsoft.Json;

namespace i_KimMobileClient.Pages.Daily_Sales;

public partial class DailySalesListPage : ContentPage
{

    private List<DailySales> _allSales = new();
    public DailySalesLister mDailySalesLister;

    public DailySalesListPage()
	{
		InitializeComponent();
        mDailySalesLister = new  DailySalesLister();
	}

    private async void OnRefreshing(object sender, EventArgs e)
    {
        SalesRefreshView.IsRefreshing = true;
        await GetDailySales();
        SalesRefreshView.IsRefreshing = false;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetDailySales();

    }

    private async Task GetDailySales()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();
            var mResponse = await new ApiServiceHandler()
                .PostAsync<DailySalesLister>(EndPointsUrl.GetDailySales, mDailySalesLister, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is DailySalesLister dailySalesLister &&
                    dailySalesLister.List != null &&
                    dailySalesLister.List.Count > 0)
                {
                    //if (Common.mUser != null && Common.mUser.StoreId > 0)
                    //    _allSales = dailySalesLister.List.Where(x => x.StoreId == Common.mUser.StoreId).ToList();

                    if (Common.mUser != null && Common.mUser.StoreId > 0)
                    {
                        _allSales = dailySalesLister.List
                            .Where(x => x.StoreId == Common.mUser.StoreId)
                            .OrderByDescending(x => x.Date)
                            .ToList();
                    }

                    //bool hasTodaySales = _allSales.Any(x => x.Date.Date == DateTime.Today);

                    //BdrAddBtn.IsEnabled = !hasTodaySales;
                    //BdrAddBtn.Opacity = hasTodaySales ? 0.5 : 1.0;

                    bool hasTodaySales = _allSales.Any(x => x.Date.Date == DateTime.Today);

                    if (hasTodaySales)
                    {
                        BdrAddBtn.IsEnabled = false;
                        BdrAddBtn.Opacity = 0.5;
                    }
                    else
                    {
                        BdrAddBtn.IsEnabled = true;
                        BdrAddBtn.Opacity = 1.0;
                    }

                    clsDailySalesList.ItemsSource = null;
                    clsDailySalesList.ItemsSource = _allSales;
                }
                else
                {
                    _allSales = new List<DailySales>();
                    clsDailySalesList.ItemsSource = _allSales;

                    BdrAddBtn.IsEnabled = true;
                    BdrAddBtn.Opacity = 1.0;
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


    //private void BdrAddDailySales_Tapped(object sender, TappedEventArgs e)
    //{
    //    Navigation.PushAsync(new AddDailySalesPage());
    //}

    private async void BdrAddDailySales_Tapped(object sender, TappedEventArgs e)
    {
        DateTime pendingDate;

        if (_allSales != null && _allSales.Count > 0)
        {
            DateTime lastSalesDate = _allSales
                .Max(x => x.Date)
                .Date;

            pendingDate = lastSalesDate.AddDays(1);
        }
        else
        {
            pendingDate = DateTime.Today;
        }

        await Navigation.PushAsync(
            new AddDailySalesPage(pendingDate));
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        CleanupNavigationStack();
        await Navigation.PopAsync();
    }

    private void CleanupNavigationStack()
    {
        var stack = Navigation.NavigationStack.ToList();

        var homePage = stack.FirstOrDefault(p => p is HomeTabbedPage);

        if (homePage == null)
            return;

        int homeIndex = stack.IndexOf(homePage);

        for (int i = stack.Count - 2; i > homeIndex; i--)
        {
            Navigation.RemovePage(stack[i]);
        }
    }
    protected override bool OnBackButtonPressed()
    {
        CleanupNavigationStack();

        return base.OnBackButtonPressed();
    }


    private void PrintNavigationStack()
    {
        Console.WriteLine("=== Navigation Stack ===");

        foreach (var page in Navigation.NavigationStack)
        {
            Console.WriteLine(page.GetType().Name);
        }

        Console.WriteLine("========================");
    }
}