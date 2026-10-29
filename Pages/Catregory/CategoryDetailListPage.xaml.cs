using System.Diagnostics;
using System.Xml.Linq;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using i_KimMobileClient.Views;


namespace i_KimMobileClient.Pages.Catregory;

public partial class CategoryDetailListPage : ContentPage
{
    int _categoryId = 0;
    Category _mCategory = new();
    private List<Item> _mItems = new();
    private decimal _grandTotal = 0;
    private List<Item> _allCategories = new();
    DashboardView dashboardView = new DashboardView();
    public CategoryDetailListPage(int categoryId, Category mCategory)
    {
        InitializeComponent();
        _categoryId = categoryId;
        _mCategory = mCategory;
        LblTital.Text = _mCategory.Name;
        CartService.CountChanged += OnCartCountChanged;
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            lblCartItemCount.Text = CartService.Count.ToString();
            await dashboardView.GetCartItemCount();
            await GetActiveCategories();
        });
    }
    private void OnCartCountChanged(object? sender, int newCount)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            lblCartItemCount.Text = newCount.ToString();
        });
    }
    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);
        if (args.NewHandler == null)
        {
            CartService.CountChanged -= OnCartCountChanged;
        }
    }
    private async Task GetActiveCategories2()
    {
        try
        {
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetSubCategoryById, _categoryId, Common.mUser.PricingTierId);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<Item>>(url, Common.mUser.Token);

           

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is List<Item> items &&
                    items.Count > 0)
                {
                    _mItems = items;
                    _allCategories = _mItems.ToList();
                    clsCategoryItems.ItemsSource = items.ToList();
                }
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"GetActiveCategories: {ex.Message}");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

    private async Task GetActiveCategories()
    {
        try
        {
            await PopLoaderService.ShowAsync();

            if (!ItemCacheService.IsLoaded)
                await ItemCacheService.LoadAsync();

            _mItems = ItemCacheService.Items
                .Where(x => x.CategoryId == _categoryId)
                .ToList();

            _allCategories = _mItems.ToList();

            clsCategoryItems.ItemsSource = _mItems;
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
    private void OpenCartItem_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new AddToCartPage());
    }
    private async void BtnBack_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }
    private int TotalItem = 0;
    private void BdrAddPartyOrder_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border bdrPlus && bdrPlus.BindingContext is Item item)
        {
            int step = item.Multiply > 0 ? item.Multiply : 1;

            if (item.QuntityUnit == 0)
            {
                TotalItem++;
            }

            // Jump by multiplier each tap: 0  5  10  15
            item.QuntityUnit += step;
            item.NumberOfQuntity = item.QuntityUnit; // keep in sync
            UpdateTotals();
        }
    }

    private void UpdateTotals()
    {
        _grandTotal = (decimal)_mItems
       .Where(x => x.NumberOfQuntity > 0)
       .Sum(x => x.Price * x.QuntityUnit);
    }

    private void BdrMinusPartyOrder_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border bdrMinus && bdrMinus.BindingContext is Item item)
        {
            int step = item.Multiply > 0 ? item.Multiply : 1;

            if (item.QuntityUnit == 0) return;

            item.QuntityUnit -= step;

            // Clamp to 0 (avoid negative)
            if (item.QuntityUnit < 0) item.QuntityUnit = 0;

            if (item.QuntityUnit == 0)
            {
                TotalItem--;
            }

            item.NumberOfQuntity = item.QuntityUnit; // keep in sync

            UpdateTotals();
        }
    }

    private async void AddToCard_Tapped(object sender, TappedEventArgs e)
    {
        await AddToCartItem();
    }

    private async Task AddToCartItem()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();
            var selectedItems = _mItems.Where(x => x.QuntityUnit > 0).ToList();

            if (!selectedItems.Any())
            {
                Common.ErrorMessage("Please select at least one item.");
                return;
            }
            List<CartItem> cartItems = new List<CartItem>();
            foreach (var item in selectedItems)
            {
                cartItems.Add(new CartItem
                {
                    StoreId = Common.mUser.StoreId.Value,
                    ItemId = item.Id,
                    Quantity = item.QuntityUnit,
                    UnitOfMeasureId = item.UnitOfMeasureId,
                    UnitOfMeasure = item.UnitOfMeasure,
                    Item = item.Name,
                    Rate = item.Price,
                    Amount = item.Price * item.QuntityUnit,
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
            Common.SuccessMessage("Items added successfully.");
            // Instant local update — every subscribed page's badge updates immediately
            CartService.Increment(selectedItems.Count);

            // Optional: reconcile with server truth in the background (covers edge cases
            // like partial failures, concurrent carts, etc.) without blocking navigation
            _ = CartService.RefreshAsync();
            await Navigation.PopAsync();
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

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string search = e.NewTextValue?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(search))
            {
                clsCategoryItems.ItemsSource = _allCategories;
                return;
            }

            var filtered = _allCategories
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) &&
                            x.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

            clsCategoryItems.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }


    //private async Task AddItemInTocart()
    //{
    //    try
    //    {
    //        await PopLoaderService.ShowAsync();
    //        var addCart = new CartItem()
    //        {
    //            // = addCart.Id,
    //            StoreId = Common.mUser.StoreId.Value,
    //            ItemId= 1,
    //            CreatedBy = Common.mUser.Id,
    //            LastModifiedBy = Common.mUser.Id,
    //        };

    //        var selectedItems = _mItems.Where(x => x.NumberOfQuntity > 0).ToList();

    //        if (selectedItems.Count == 0)
    //        {
    //            Common.ErrorMessage("Please select at least one item.");
    //            return;
    //        }

    //        foreach (var item in selectedItems)
    //        {
    //            // Check if this item already existed in original order (for edit)

    //            var existingLine = _isEdit ? mPartyOrder.Lines.FirstOrDefault(x => x.ItemId == item.Id) : null;

    //            orderToSave.Lines.Add(new PartyOrderLine()
    //            {
    //                Id = existingLine?.Id ?? 0,
    //                PartyOrderId = _isEdit ? mPartyOrder.Id : 0, // Add this
    //                ItemId = item.Id,
    //                ItemName = item.Name,
    //                //Quantity = item.QuntityUnit,
    //                Quantity = item.NumberOfQuntity,
    //                Rate = item.Price,
    //                //QuantityRequested = item.NumberOfQuntity,
    //                Total = (item.QuntityUnit * item.Price) + _waiterCharge,
    //                //Total = (item.QuntityUnit * item.Price),
    //                UnitOfMeasureId = item.UnitOfMeasureId,
    //                UnitOfMeasure = item.UnitOfMeasure,
    //                CreatedBy = Common.mUser.Id,
    //                LastModifiedBy = Common.mUser.Id
    //            });
    //        }

    //        if (orderToSave.Lines.Count == 0)
    //        {
    //            Common.ErrorMessage("Please select at least one item.");
    //            return;
    //        }

    //        //if (mPartyOrder.Lines.Count == 0)
    //        //{
    //        //    Common.ErrorMessage("Please select at least one item.");
    //        //    return;
    //        //}

    //        var mResponse = await new APIService.ApiServiceHandler().PostAsync<PartyOrder>(EndPointsUrl.SavePartyOrder, orderToSave, Common.AuthToken);
    //        if (mResponse != null && mResponse.IsSuccess)
    //        {
    //            Common.SuccessMessage("Order updated successfully!");
    //            await Navigation.PopAsync();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Common.ErrorMessage(ex.Message);
    //    }
    //   finally
    //    {
    //        await PopLoaderService.HideAsync();
    //    }
    //}
}