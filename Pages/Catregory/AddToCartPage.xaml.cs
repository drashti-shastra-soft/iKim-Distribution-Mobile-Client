using System.Threading.Tasks;
using i_KimMobileClient.Models;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Pages.Catregory;

public partial class AddToCartPage : ContentPage
{
    private List<CartItem> _mCartItems = new();
    private int TotalItem = 0;
    private decimal _grandTotal = 0;
    public AddToCartPage()
    {
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetAllActiveCartItem();
    }


    private async Task GetAllActiveCartItem()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetAllCart, Common.mUser.StoreId.Value);
            var mResponse = await new APIService.ApiServiceHandler().PostAsync(url, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value is List<CartItem> mCartItems && mCartItems != null && mCartItems.Count > 0)
                {
                    _mCartItems = mCartItems.ToList();
                    clsItems.ItemsSource = _mCartItems;
                    UpdateTotals();
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

    private async Task AddToCartItem()
    {
        try
        {
            // Validate grand total
            //if (_grandTotal <= 0)
            //{
            //    Common.ErrorMessage("Total payment must be greater than 0.00.");
            //    return;
            //}
            if (_grandTotal <= 0)
            {
                Common.ErrorMessage(
                    "Your order total must be greater than ₹0.00 to place an order.");
                return;
            }


            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();
            List<CartItem> cartItems = new List<CartItem>();
            foreach (var item in _mCartItems)
            {
                cartItems.Add(new CartItem
                {
                    Id = item.Id,
                    StoreId = Common.mUser.StoreId.Value,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    UnitOfMeasureId = item.UnitOfMeasureId,
                    UnitOfMeasure = item.UnitOfMeasure,
                    Rate = item.Rate,
                    CreatedBy = Common.mUser.Id,
                    LastModifiedBy = Common.mUser.Id
                });
            }

            var response = await new APIService.ApiServiceHandler().PostAsync<List<CartItem>, List<CartItem>>(EndPointsUrl.AddCartItem,
            cartItems, Common.mUser.Token);

            if (response == null || !response.IsSuccess)
            {
                Common.ErrorMessage(response?.ErrorMessage ?? "Failed to add items to cart.");
                return;
            }
            await CreateNewOrder();
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


    private async Task CreateNewOrder()
    {
        try
        {
            await PopLoaderService.ShowAsync();
            var mOrder = new Order()
            {
                StoreId = Common.mUser.StoreId.Value,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id,
            };

            var mResponse = await new APIService.ApiServiceHandler().PostAsync<Order>(EndPointsUrl.CreateOrder, mOrder, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Order created successfully!");
                
                var categoryPage = Navigation.NavigationStack.OfType<CategoryDetailListPage>().LastOrDefault();

                if (categoryPage != null)
                {
                    Navigation.RemovePage(categoryPage);
                }
                await Navigation.PopAsync();
                var homePage = GetHomeTabbedPage();
                if (homePage != null)
                {
                    await homePage.SwitchTab(TabType.Orders);
                }
            }
            else
            {
                Common.ErrorMessage($"{mResponse.ErrorMessage}");
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
    private async Task CreateNewOrderV1()
    {
        try
        {
            await PopLoaderService.ShowAsync();
            var mOrder = new Order()
            {
                StoreId = Common.mUser.StoreId.Value,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id,
            };

            var selectedItems = _mCartItems.Where(x => x.Quantity > 0).ToList();

            if (selectedItems.Count == 0)
            {
                Common.ErrorMessage("Please select at least one item.");
                return;
            }

            foreach (var item in selectedItems)
            {
                mOrder.Lines.Add(new OrderLine()
                {
                    ItemId = item.ItemId,
                    QuantityRequested = item.Quantity,
                    Price = item.Rate,
                    TotalPrice = item.Rate * item.Quantity,
                    UnitOfMeasureId = item.UnitOfMeasureId,
                    Rate = item.Rate,
                    Amount = item.Amount,
                    CartItemId = item.Id,
                    CreatedBy = Common.mUser.Id,
                    LastModifiedBy = Common.mUser.Id
                });
            }

            if (mOrder.Lines.Count == 0)
            {
                Common.ErrorMessage("Please select at least one item.");
                return;
            }

            var mResponse = await new APIService.ApiServiceHandler().PostAsync<Order>(EndPointsUrl.CreateOrder, mOrder, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Order created successfully!");
                await Navigation.PopAsync();
                //await Common.GetHomeTabbedPage(this)?.NavigateToTab(TabType.Orders);
                var homePage = GetHomeTabbedPage();
                if (homePage != null)
                {
                    await homePage.SwitchTab(TabType.Orders);
                }
            }
            else
            {
                Common.ErrorMessage($"{mResponse.ErrorMessage}");
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

    private HomeTabbedPage? GetHomeTabbedPage()
    {
        if (Application.Current?.Windows[0].Page is HomeTabbedPage directPage)
            return directPage;

        if (Application.Current?.Windows[0].Page is NavigationPage navPage &&
            navPage.CurrentPage is HomeTabbedPage navHomePage)
            return navHomePage;

        // Walk up navigation stack
        foreach (var page in Navigation.NavigationStack)
        {
            if (page is HomeTabbedPage homePage)
                return homePage;
        }

        return null;
    }

    private void UpdateTotals()
    {
        _grandTotal = _mCartItems.Sum(x => x.Amount);

        lblGrandTotal.Text = $"₹ {_grandTotal:0.00}";
    }

    private async void BtnBack_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void BdrMinusPartyOrder_Tapped(object sender, TappedEventArgs e)
    {
        //if (sender is Border bdrMinus && bdrMinus.BindingContext is CartItem item)
        //{
        //    int step = item.Multiply > 0 ? item.Multiply : 1;

        //    if (item.Quantity <= 0)
        //        return;

        //    item.Quantity -= step;

        //    if (item.Quantity < 0)
        //        item.Quantity = 0;

        //    item.UnitOfMeasureId -= step;

        //    if (item.UnitOfMeasureId < step)
        //        item.UnitOfMeasureId = step;

        //    item.Amount = item.Rate * item.Quantity;

        //    UpdateTotals();
        //}
        if (sender is Border bdrMinus && bdrMinus.BindingContext is CartItem item)
        {
            int step = item.Multiply > 0 ? item.Multiply : 1;

            item.Quantity = Math.Max(0, item.Quantity - step);

            item.Amount = item.Rate * item.Quantity;

            UpdateTotals();
        }
    }

    private void BdrAddPartyOrder_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border bdrPlus && bdrPlus.BindingContext is CartItem item)
        {
            int step = item.Multiply > 0 ? item.Multiply : 1;

            item.Quantity += step;

            item.Amount = item.Rate * item.Quantity;

            UpdateTotals();
        }
    }

    private async void AddToCard_Tapped(object sender, TappedEventArgs e)
    {
        await AddToCartItem();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {

        OrderRefreshView.IsRefreshing = true;
        await PopLoaderService.ShowAsync();
        await GetAllActiveCartItem();
        await PopLoaderService.HideAsync();
        OrderRefreshView.IsRefreshing = false;
    }

    private async void BtnDelete_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Image bdrDeleteAction &&
            bdrDeleteAction.BindingContext is CartItem item &&
            item != null && item.Id > 0)
        {
            //await MopupService.Instance.PushAsync(new CommonPopupPage());
            var data = new CommonPopupPage(true);
            data.logoutEvent += async (s1, e1) =>
            {
                await MopupService.Instance.PopAsync();
                await DeleteOrder(item.Id);
            };
            await MopupService.Instance.PushAsync(data);
        }
    }

    private async Task DeleteOrder(int cartItemId)
    {
        try
        {
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.DeleteCartItem, cartItemId, Common.mUser.Id);
            var mResponse = await new APIService.ApiServiceHandler().DeleteAsync<Response>(url, Common.mUser.Token);

            if (mResponse != null && mResponse.IsSuccess)
            {
                var deletedItem = _mCartItems.FirstOrDefault(x => x.Id == cartItemId);
                if (deletedItem != null)
                {
                    _mCartItems.Remove(deletedItem);

                    clsItems.ItemsSource = null;
                    clsItems.ItemsSource = _mCartItems;

                    UpdateTotals();
                }

                Common.SuccessMessage("Item deleted successfully.");
                // Instant local update — every subscribed page's badge updates immediately
                CartService.Decrement(1);
                // Reconcile with server truth in the background
                _ = CartService.RefreshAsync();


                // If cart is empty, go back
                if (_mCartItems.Count == 0)
                {
                    await Navigation.PopAsync();
                    return;
                }
            }
            else
            {
                Common.ErrorMessage(mResponse?.ErrorMessage ?? "Failed to delete item.");
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