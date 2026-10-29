using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using i_KimMobileClient.Views;
using Mopups.Services;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Pages;

public partial class PartyOrderItemPage : ContentPage
{
    private int _partyOrderId;
    private bool _isFromCreateOrder;
    private bool _fromAddItem;
    PartyOrder _mPartyOrder = new();
    public PartyOrderItemPage(int partyOrderId = 0, PartyOrder partyOrder = null, bool isFromCreateOrder = false)
    {
        InitializeComponent();
        _partyOrderId = partyOrderId;
        _mPartyOrder = partyOrder;
        _isFromCreateOrder = isFromCreateOrder;
        lblTitle.Text = _mPartyOrder.Name + " Party Order Items";
        BindingContext = this;
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        OrderRefreshView.IsRefreshing = true;
        await GetPartyOrderLines();
        OrderRefreshView.IsRefreshing = false;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetPartyOrderLines();

    }

    //protected override void OnDisappearing()
    //{
    //    base.OnDisappearing();
    //    //var navigationStack = Navigation.NavigationStack;
    //    //while (navigationStack.Count > 1)
    //    //{
    //    //    Navigation.RemovePage(navigationStack[1]);
    //    //}

    //    var navigationStack = Navigation.NavigationStack.ToList();

    //    // Keep LoginPage and HomeTabbedPage
    //    while (Navigation.NavigationStack.Count > 3)
    //    {
    //        Navigation.RemovePage(Navigation.NavigationStack[2]);
    //    }
    //}

    //protected override void OnDisappearing()
    //{
    //    base.OnDisappearing();

    //    var stack = Navigation.NavigationStack.ToList();
    //    #region method 1
    //    var homePage = stack.FirstOrDefault(p => p is HomeTabbedPage);

    //    if (homePage != null)
    //    {
    //        int homeIndex = stack.IndexOf(homePage);

    //        // Remove all pages between HomeTabbedPage and the current page
    //        for (int i = stack.Count - 2; i > homeIndex; i--)
    //        {
    //            Navigation.RemovePage(stack[i]);
    //        }
    //    }
    //    #endregion
    //    #region Method 2
    //    //int homeIndex = stack.FindIndex(p => p.GetType().Name == "HomeTabbedPage");

    //    //if (homeIndex != -1)
    //    //{
    //    //    for (int i = stack.Count - 2; i > homeIndex; i--)
    //    //    {
    //    //        Navigation.RemovePage(stack[i]);
    //    //    }
    //    //}
    //    #endregion

    //}

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


    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        CleanupNavigationStack();
        //if (_isFromCreateOrder)
        //{
        //    CleanupNavigationStack();
        //}
        await Navigation.PopAsync();
    }
    private async void OnBackTappedV2(object sender, TappedEventArgs e)
    {
        if (_fromAddItem)
        {
            Navigation.RemovePage(Navigation.NavigationStack[^2]);
            await Navigation.PopAsync();
        }
        else
        {
            await Navigation.PopAsync();
        }
    }


    private async void OnBackTappedv1(object sender, TappedEventArgs e)
    {
        try
        {
            foreach (var page in Navigation.NavigationStack)
            {
                Console.WriteLine(page.GetType().Name);
            }

            while (Navigation.NavigationStack.Count > 1)
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

            }

            await Navigation.PopAsync();

            //if (HomeTabbedPage.Current != null)
            //{
            //    await HomeTabbedPage.Current.NavigateToTab(TabType.Delivery);
            //}


            // Find HomeTabbedPage in the navigation stack
            //var homePage = Navigation.NavigationStack
            //                         .OfType<HomeTabbedPage>()
            //                         .FirstOrDefault();

            //if (homePage != null)
            //{
            //    // Remove all pages above HomeTabbedPage
            //    while (Navigation.NavigationStack.Last() != homePage)
            //    {
            //        Navigation.RemovePage(Navigation.NavigationStack.Last());
            //    }

            //    // Show Delivery tab
            //    await homePage.NavigateToTab(TabType.Delivery);

            //    // Navigate back to HomeTabbedPage
            //    await Navigation.PopToRootAsync();
            //}
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
    }

    private void BdrAddPartyOrderItem_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new AddPartyOrderItemPage(_partyOrderId, _mPartyOrder));
    }


    private async Task GetPartyOrderLines()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;
            //await PopLoaderService.ShowAsync();
            await PopLoaderService.ShowAsync();

            var url = string.Format(EndPointsUrl.GetPartyOrderLineById, _partyOrderId);

            var mPartyOrderResponse = await new ApiServiceHandler()
                .GetAsync<List<PartyOrderLine>>(url, Common.AuthToken);

            //if (mPartyOrderResponse != null && mPartyOrderResponse.IsSuccess)
            //{
            //    var rawLines = (List<PartyOrderLine>)mPartyOrderResponse.Value;
            //    var lines = rawLines.OrderByDescending(x => x.Id).ToList();
            //    clsPartyOrderItemList.ItemsSource = lines;
            //}

            if (mPartyOrderResponse != null && mPartyOrderResponse.IsSuccess)
            {
                if (mPartyOrderResponse.Value != null &&
                    mPartyOrderResponse.Value is List<PartyOrderLine> rawLines &&
                    rawLines.Count > 0)
                {
                    var lines = rawLines.OrderByDescending(x => x.Id).ToList();

                    await Task.Run(() =>
                    {
                        Parallel.ForEach(lines, item =>
                        {
                            if (!string.IsNullOrWhiteSpace(item.Base64String))
                            {
                                try
                                {
                                    var bytes = Convert.FromBase64String(item.Base64String);

                                    item.ImageSourceCached =
                                        ImageSource.FromStream(() =>
                                            new MemoryStream(bytes));
                                }
                                catch
                                {
                                    item.ImageSourceCached = null;
                                }
                            }
                        });
                    });

                    clsPartyOrderItemList.ItemsSource = lines;
                }
                else
                {
                    clsPartyOrderItemList.ItemsSource = null;
                }
            }
            else
            {
                Common.ErrorMessage(mPartyOrderResponse.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
        finally
        {
            //await PopLoaderService.HideAsync();
            await PopLoaderService.HideAsync();
        }
    }

    private void OnViewTapped(object sender, TappedEventArgs e)
    {
        if (sender is not Border bdrPartyOrder)
            return;

        if (bdrPartyOrder != null && bdrPartyOrder.BindingContext is PartyOrderLine partyOrderLine && partyOrderLine.Id > 0)
        {

            Navigation.PushAsync(new AddPartyOrderItemPage(_partyOrderId, null, partyOrderLine, true));
        }
    }
    private async void BdrDelete_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border bdrDeleteAction &&
            bdrDeleteAction.BindingContext is PartyOrderLine partyOrderLine &&
            partyOrderLine != null && partyOrderLine.Id > 0)
        {
            var data = new CommonPopupPage(true);
            data.logoutEvent += async (s1, e1) =>
            {
                await MopupService.Instance.PopAsync();
                await DeleteOrder(partyOrderLine.Id);
            };
            await MopupService.Instance.PushAsync(data);
        }
    }

    private async Task DeleteOrder(int PartyOrderLineId)
    {
        try
        {
            //if (!Common.CheckInternetConnection())
            //{
            //    Common.InfoMessage("Please check your internet connection and try again.");
            //    return;
            //}

            //await PopLoaderService.ShowAsync();
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.DeletePartyOrderLine, PartyOrderLineId, Common.mUser.Id);
            var mResponse = await new APIService.ApiServiceHandler().DeleteAsync<Response>(url, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Item delete successfully.");
                await GetPartyOrderLines();
            }
            else
            {
                Common.ErrorMessage("Failed to delete item.");
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
        finally
        {
            //await PopLoaderService.HideAsync();
            await PopLoaderService.HideAsync();
        }
    }

}