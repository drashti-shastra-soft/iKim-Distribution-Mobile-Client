using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Pages.Daily_Sales;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Mopups.Services;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Views;

public partial class DeliveryView : ContentView
{
    private List<PartyOrder> _allOrders = new();
    public PartyOrderLister mPartyOrderLister;

    public static event Action<Order>? OnEditOrder;
    public DeliveryView()
    {
        InitializeComponent();
        mPartyOrderLister = new PartyOrderLister();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await GetPartyOrders();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
    }

    private async Task GetPartyOrders()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();

            var mResponse = await new ApiServiceHandler()
                .PostAsync<PartyOrderLister>(EndPointsUrl.GetPartyOrders, mPartyOrderLister, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is PartyOrderLister partyOrderLister &&
                    partyOrderLister.List != null &&
                    partyOrderLister.List.Count > 0)
                {
                    foreach (var mPartyOrder in partyOrderLister.List)
                    {
                        var url = string.Format(EndPointsUrl.GetPartyOrderLineById, mPartyOrder.Id);
                        var mPartyOrderResponse = await new ApiServiceHandler()
                            .GetAsync<List<PartyOrderLine>>(url, Common.AuthToken);

                        if (mPartyOrderResponse != null && mPartyOrderResponse.IsSuccess)
                        {
                            var rawLines = (List<PartyOrderLine>)mPartyOrderResponse.Value;

                            // Take only LATEST line per ItemId (highest Id = most recent update)
                            var groupedLines = rawLines
                                .GroupBy(x => x.ItemId)
                                .Select(g => g.OrderByDescending(x => x.Id).First()) //  Latest only
                                .ToList();

                            //mPartyOrder.Lines = groupedLines;
                        }
                    }

                    if (Common.mUser != null && Common.mUser.StoreId > 0)
                        _allOrders = partyOrderLister.List.Where(x => x.StoreId == Common.mUser.StoreId).ToList();

                    clsPartyOrderList.ItemsSource = null;
                    clsPartyOrderList.ItemsSource = _allOrders;
                }
                else
                {
                    _allOrders = new List<PartyOrder>();
                    clsPartyOrderList.ItemsSource = _allOrders;
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

    private void OrderItem_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is PartyOrder selectedOrder)
        {
            Navigation.PushAsync(new PartyOrderItemPage(selectedOrder.Id, selectedOrder));
        }
    }

    private async void BdrSelectedObject_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is PartyOrder partyOrderorder)
        {
            
            await Navigation.PushAsync(new AddPartyOrderPage(partyOrderorder));
        }
    }

    private void OnBackTapped(object sender, TappedEventArgs e)
    {

    }

    private async void OnViewTapped(object sender, TappedEventArgs e)
    {
        if (sender is not Border bdrPartyOrder)
            return;

        if (bdrPartyOrder != null && bdrPartyOrder.BindingContext is PartyOrder partyOrder && partyOrder.Id > 0)
        {
            await PopLoaderService.ShowAsync();
            await Navigation.PushAsync(new AddPartyOrderPage(partyOrder, true));
            await PopLoaderService.HideAsync();

        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var query = e.NewTextValue?.Trim().ToLower() ?? "";

            //  Show/hide clear button
            BtnClearSearch.IsVisible = !string.IsNullOrEmpty(query);

            if (string.IsNullOrEmpty(query))
            {
                clsPartyOrderList.ItemsSource = null;
                clsPartyOrderList.ItemsSource = _allOrders;
                return;
            }

            var filtered = _allOrders.Where(o =>
                (!string.IsNullOrEmpty(o.Name) &&
                    o.Name.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(o.MobileNumber) &&
                    o.MobileNumber.Contains(query)) ||
                (!string.IsNullOrEmpty(o.Store) &&
                    o.Store.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(o.DeliveryLocation) &&
                    o.DeliveryLocation.ToLower().Contains(query))
            ).ToList();

            clsPartyOrderList.ItemsSource = null;
            clsPartyOrderList.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }

    private async void BdrAddPartyOrder_Tapped(object sender, TappedEventArgs e)
    {
        await PopLoaderService.ShowAsync();
        await Navigation.PushAsync(new AddPartyOrderPage());
        await PopLoaderService.HideAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        OrderRefreshView.IsRefreshing = true;
        await GetPartyOrders();
        OrderRefreshView.IsRefreshing = false;
    }

    private void OnClearSearch(object sender, TappedEventArgs e)
    {

        EntrySearch.Text = string.Empty;
        BtnClearSearch.IsVisible = false;
        clsPartyOrderList.ItemsSource = null;
        clsPartyOrderList.ItemsSource = _allOrders;
    }

    private async void BdrDelete_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border bdrDeleteAction &&
            bdrDeleteAction.BindingContext is PartyOrder partyOrder &&
            partyOrder != null && partyOrder.Id > 0)
        {
            var data = new CommonPopupPage(true);
            data.logoutEvent += async (s1, e1) =>
            {
                await MopupService.Instance.PopAsync();
                await DeleteOrder(partyOrder.Id);
            };
            await MopupService.Instance.PushAsync(data);
        }
    }

    private async Task DeleteOrder(int PartyOrderId)
    {
        try
        {
            //if (!Common.CheckInternetConnection())
            //{
            //    Common.InfoMessage("Please check your internet connection and try again.");
            //    return;
            //}

            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.DeletePartyOrder, PartyOrderId, Common.mUser.Id);
            var mResponse = await new APIService.ApiServiceHandler().DeleteAsync<Response>(url, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Order delete successfully.");
                await GetPartyOrders();
            }
            else
            {
                Common.ErrorMessage("Failed to delete order.");
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

}