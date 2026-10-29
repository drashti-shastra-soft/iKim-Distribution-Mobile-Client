using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Views;

public partial class OrderView : ContentView
{
    private List<Order> _allOrders = new();
    public OrderLister mOrderLister;
    private bool _isLoaded;

    public static event Action<Order>? OnEditOrder;
    public OrderView()
    {
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("OrderView Created");
        mOrderLister = new OrderLister();
        //LoadOrders();
        //SetFilterTab(TabPending, true);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        //if (_isLoaded)
        //    return;
        //_isLoaded = true;

        await GetOrders();
    }


    private void OnUnloaded(object? sender, EventArgs e)
    {
        //Loaded -= OnLoaded;
        //Unloaded -= OnUnloaded;
    }


    private async Task GetOrders()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            //if (!NetworkService.HasInternet())
            //{
            //    await DialogService.ShowNoInternetPopupAsync();
            //    return;
            //}

            SetFilterTab(TabPending, false);
            SetFilterTab(TabPF, false);
            SetFilterTab(TabFulfilled, false);

            //GetHomeTabbedPage()?.ShowPageLoader();
            await PopLoaderService.ShowAsync();
            var mResponse = await new ApiServiceHandler()
                .PostAsync<OrderLister>(EndPointsUrl.GetAllOrder, mOrderLister, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is OrderLister mOrderLister &&
                    mOrderLister.List != null &&
                    mOrderLister.List.Count > 0)
                {
                    foreach (var item in mOrderLister.List)
                    {
                        var url = string.Format(EndPointsUrl.GetOrderLine, item.Id);
                        var mOrderlineResponse = await new ApiServiceHandler()
                            .GetAsync<List<OrderLine>>(url, Common.AuthToken);

                        if (mOrderlineResponse != null && mOrderlineResponse.IsSuccess)
                        {
                            var rawLines = (List<OrderLine>)mOrderlineResponse.Value;

                            // Take only LATEST line per ItemId (highest Id = most recent update)
                            var groupedLines = rawLines
                                .GroupBy(x => x.ItemId)
                                .Select(g => g.OrderByDescending(x => x.Id).First()) //  Latest only
                                .ToList();

                            item.Lines = groupedLines;

                            // Total items = count of unique items
                            item.TotalItemsText = groupedLines.Count.ToString();

                            //  Item names with quantity
                            item.QuantitySummary = string.Join(", ",
                                groupedLines.Select(x => $"{x.Item} ({x.QuantityRequested})"));

                            //  Total amount
                            //decimal total = groupedLines.Sum(x => x.Rate * x.QuantityRequested);
                            //item.TotalAmount = $"{total:0.00}";


                            //  Total amount
                            item.TotalAmount = item.Amount.ToString();


                        }
                    }

                    _allOrders = Common.mUser?.StoreId > 0 ? mOrderLister.List.Where(x => x.StoreId == Common.mUser.StoreId)
                    .ToList() : mOrderLister.List;

                    OrdersCollection.ItemsSource = null;
                    OrdersCollection.ItemsSource = _allOrders;
                }
                else
                {
                    _allOrders = new List<Order>();
                    OrdersCollection.ItemsSource = _allOrders;
                }
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
        finally
        {
            //GetHomeTabbedPage()?.ShowPageLoader();
            await PopLoaderService.HideAsync();
            //OrderRefreshView.IsRefreshing = false;
        }
    }

    private void OnFilterTapped(object sender, EventArgs e)
    {
        if (sender is not Border tapped)
            return;

        SetFilterTab(TabPending, false);
        SetFilterTab(TabPF, false);
        SetFilterTab(TabFulfilled, false);

        SetFilterTab(tapped, true);

        var filtered = tapped.AutomationId switch
        {
            "Pending" => _allOrders
                .Where(x => x.Status == "Pending")
                .ToList(),

            "Partially Fulfilled" => _allOrders
                .Where(x => x.Status == "Partially Fulfilled")
                .ToList(),

            "Fulfilled" => _allOrders
                .Where(x => x.Status == "Fulfilled")
                .ToList(),

            _ => _allOrders
        };

        OrdersCollection.ItemsSource = filtered;
    }

    private void SetFilterTab(Border tab, bool active)
    {
        if (active)
        {
            tab.Background = new SolidColorBrush(Color.FromArgb("#FF6B00"));
            tab.Stroke = new SolidColorBrush(Color.FromArgb("#FF6B00"));
            tab.StrokeThickness = 0;
            if (tab.Content is Label lbl)
            {
                lbl.TextColor = Colors.White;
            }
        }
        else
        {
            tab.Background = new SolidColorBrush(Colors.White);
            tab.Stroke = new SolidColorBrush(Color.FromArgb("#E0E0E0"));
            tab.StrokeThickness = 1;
            if (tab.Content is Label lbl)
            {
                lbl.TextColor = Color.FromArgb("#9E9E9E");
            }
        }
    }
    void ClearAllFilter()
    {
        //TabAllOrders.Background = 
        TabPending.Background = TabPF.Background = TabFulfilled.Background = Colors.White;
        //lblAll.TextColor = 
        lblPending.TextColor = lblTabPF.TextColor = lblFulfilled.TextColor = Color.FromArgb("#9E9E9E");
        //TabAllOrders.Stroke = 
        TabPending.Stroke = TabPF.Stroke = TabFulfilled.Stroke = Color.FromArgb("#E0E0E0");
        //TabAllOrders.StrokeThickness =
        TabPending.StrokeThickness = TabPF.StrokeThickness = TabFulfilled.StrokeThickness = 1;
    }
    private async void OnRefreshing(object sender, EventArgs e)
    {
        OrderRefreshView.IsRefreshing = true;
        await GetOrders();
        OrderRefreshView.IsRefreshing = false;
    }

    private async Task GetOrders1()
    {
        try
        {
            await PopLoaderService.ShowAsync();
            var mResponse = await new ApiServiceHandler().PostAsync<OrderLister>(EndPointsUrl.GetAllOrder, mOrderLister, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null && mResponse.Value is OrderLister mOrderLister && mOrderLister.List != null && mOrderLister.List.Count > 0)
                {
                    //_allOrders = mOrderLister.List;
                    foreach (var item in mOrderLister.List)
                    {
                        var url = string.Format(EndPointsUrl.GetOrderLine, item.Id);
                        var mOrderlineResponse = await new ApiServiceHandler().GetAsync<List<OrderLine>>(url, Common.AuthToken);
                        if (mOrderlineResponse != null && mOrderlineResponse.IsSuccess)
                        {
                            item.Lines = (List<OrderLine>)mOrderlineResponse.Value;

                            // Total Items
                            item.TotalItemsText = item.Lines.Count.ToString();

                            // Item Names
                            item.ItemsSummary = string.Join(", ", item.Lines.Select(x => x.Item));

                            item.QuantitySummary = string.Join(", ",
        item.Lines.Select(x =>
            $"{x.Item} ({x.QuantityRequested})"));
                            // Total Amount
                            decimal total = item.Lines.Sum(x => x.Price * x.QuantityRequested);

                            item.TotalAmount = $"{total:0.00}";
                        }
                    }
                    _allOrders = mOrderLister.List;
                    OrdersCollection.ItemsSource = null;
                    OrdersCollection.ItemsSource = _allOrders;
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
    private void OnFilterTapped1(object sender, EventArgs e)
    {
        if (sender is Border bdrSelectedFilter)
        {
            ClearAllFilter();
            if (bdrSelectedFilter.AutomationId == "All")
            {
                //TabAllOrders.Background = Color.FromArgb("#FF7A1A");
                //lblAll.TextColor = Color.FromArgb("#FFFFFF");
                //TabAllOrders.Stroke = Color.FromArgb("#E0E0E0");
                //TabAllOrders.StrokeThickness = 0;
            }
            else if (bdrSelectedFilter.AutomationId == "Pending")
            {
                TabPending.Background = Color.FromArgb("#FF7A1A");
                lblPending.TextColor = Color.FromArgb("#FFFFFF");
                TabPending.Stroke = Color.FromArgb("#E0E0E0");
                TabPending.StrokeThickness = 0;
            }
            else if (bdrSelectedFilter.AutomationId == "Completed")
            {
                TabPF.Background = Color.FromArgb("#FF7A1A");
                lblTabPF.TextColor = Color.FromArgb("#FFFFFF");
                TabPF.Stroke = Color.FromArgb("#E0E0E0");
                TabPF.StrokeThickness = 0;
            }
            else if (bdrSelectedFilter.AutomationId == "Completed")
            {
                TabFulfilled.Background = Color.FromArgb("#FF7A1A");
                lblFulfilled.TextColor = Color.FromArgb("#FFFFFF");
                TabFulfilled.Stroke = Color.FromArgb("#E0E0E0");
                TabFulfilled.StrokeThickness = 0;
            }

            var filtered = bdrSelectedFilter.AutomationId switch
            {
                "All" => _allOrders,
                "Pending" => _allOrders.Where(o => o.Status == "Pending").ToList(),
                "Partially Fulfilled" => _allOrders.Where(o => o.Status == "Delivered" || o.Status == "Completed").ToList(),
                _ => _allOrders
            };

            OrdersCollection.ItemsSource = filtered;
        }
    }

    private async void BdrSelectedObject_Tapped(object sender, TappedEventArgs e)
    {
        //if (e.Parameter is Order order)
        //{
        //    var homePage = GetHomeTabbedPage();
        //    if (homePage != null)
        //    {
        //        await homePage.EditOrder(order);
        //    }
        //}
    }

    private HomeTabbedPage? GetHomeTabbedPage()
    {
        // Method 1: Direct window page
        if (Application.Current?.Windows[0].Page is HomeTabbedPage directPage)
            return directPage;

        // Method 2: Wrapped in NavigationPage
        if (Application.Current?.Windows[0].Page is NavigationPage navPage &&
            navPage.CurrentPage is HomeTabbedPage navHomePage)
            return navHomePage;

        // Method 3: Walk up from this view using Parent
        Element? parent = this.Parent;
        while (parent != null)
        {
            if (parent is HomeTabbedPage foundPage)
                return foundPage;
            parent = parent.Parent;
        }

        return null;
    }

}