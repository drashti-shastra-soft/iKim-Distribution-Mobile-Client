using System.Collections.ObjectModel;
using System.Diagnostics;
using i_KimMobileClient.Models;
using i_KimMobileClient.Pages.Catregory;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Views;

public partial class NewOrderView : ContentView
{
    private List<Category> _allCategories = new();
    public event EventHandler refreshEvent;

    //public NewOrderView(bool isEditNav = false)
    //{
    //    InitializeComponent();
    //    Loaded += async (s, e) =>
    //    {
    //        await GetActiveCategories();
    //    };
    //    Loaded += OnLoaded;
    //    Unloaded += OnUnloaded;
    //}

    public NewOrderView(bool isEditNav = false)
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    #region [ Methods ]
    //private async void OnLoaded(object? sender, EventArgs e)
    //{
    //    _ = CartService.RefreshAsync();
    //    lblCartItemCount.Text = CartService.Count.ToString();
    //    CartService.CountChanged += OnCartCountChanged;
    //    await GetActiveCategories();
    //}

    //private async void OnLoaded(object? sender, EventArgs e)
    //{
    //    _ = CartService.RefreshAsync();

    //    lblCartItemCount.Text = CartService.Count.ToString();

    //    CartService.CountChanged += OnCartCountChanged;

    //    await CategoryCacheService.LoadAsync();

    //    await ItemCacheService.LoadAsync();

    //    await LoadAllItems();

    //    await GetActiveCategories();
    //}

    private async void OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            await PopLoaderService.ShowAsync();

            _ = CartService.RefreshAsync();

            lblCartItemCount.Text = CartService.Count.ToString();

            CartService.CountChanged += OnCartCountChanged;

            await CategoryCacheService.LoadAsync();
            await ItemCacheService.LoadAsync();
            await GetActiveCategories();
            await LoadAllItems();
           
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"OnLoaded Error: {ex.Message}");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }


    private void OnUnloaded(object? sender, EventArgs e)
    {

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
    private async Task GetActiveCategoriesV2()
    {
        try
        {
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetAllActiveCategory);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<Category>>(url, Common.mUser.Token);

            if (mResponse != null && mResponse.IsSuccess &&
                mResponse.Value is List<Category> categories &&
                categories.Count > 0)
            {
                for (int i = 0; i < categories.Count; i++)
                    categories[i].Index = i;

                // Decode all base64 images off the UI thread, in parallel
                await Task.Run(() =>
                {
                    Parallel.ForEach(categories, cat =>
                    {
                        if (!string.IsNullOrWhiteSpace(cat.Base64String))
                        {
                            try
                            {
                                var bytes = Convert.FromBase64String(cat.Base64String);
                                cat.ImageSourceCached = ImageSource.FromStream(() => new MemoryStream(bytes));
                            }
                            catch
                            {
                                cat.ImageSourceCached = null;
                            }
                        }
                    });
                });

                _allCategories = categories;
                clsCategory.ItemsSource = _allCategories; // UI thread only does layout now, no decoding
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

    private async Task GetActiveCategories(bool forceRefresh = false)
    {
        // Serve from cache — no network call, no loader, instant
        //if (!forceRefresh && CategoryCacheService.IsLoaded && !CategoryCacheService.IsExpired)
        if (!forceRefresh && CategoryCacheService.IsLoaded)
        {
            _allCategories = CategoryCacheService.Categories;
            clsCategory.ItemsSource = _allCategories;
            return;
        }

        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            //await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetAllActiveCategory);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<Category>>(url, Common.mUser.Token);

            if (mResponse != null && mResponse.IsSuccess &&
                mResponse.Value is List<Category> categories &&
                categories.Count > 0)
            {
                for (int i = 0; i < categories.Count; i++)
                    categories[i].Index = i;

                await Task.Run(() =>
                {
                    Parallel.ForEach(categories, cat =>
                    {
                        if (!string.IsNullOrWhiteSpace(cat.Base64String))
                        {
                            try
                            {
                                var bytes = Convert.FromBase64String(cat.Base64String);
                                cat.ImageSourceCached = ImageSource.FromStream(() => new MemoryStream(bytes));
                            }
                            catch
                            {
                                cat.ImageSourceCached = null;
                            }
                        }
                    });
                });

                _allCategories = categories;
                clsCategory.ItemsSource = _allCategories;

                // Populate cache for next time
                //CategoryCacheService.Categories = categories;
                //CategoryCacheService.LastFetched = DateTime.Now;
                await CategoryCacheService.SaveAsync(categories,Common.mUser.Id);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"GetActiveCategories: {ex.Message}");
        }
        //finally
        //{
        //    await PopLoaderService.HideAsync();
        //}
    }




    private async Task LoadAllItems()
    {
        if (ItemCacheService.IsLoaded)
            return;
        try
        {
            //await PopLoaderService.ShowAsync();

            string url = string.Format(
                EndPointsUrl.GetAllActiveItemsGetOrders,
                Common.mUser.PricingTierId
            );
            var response = await new APIService.ApiServiceHandler()
                .GetAsync<List<Item>>(url, Common.mUser.Token);

            if (response != null &&
               response.IsSuccess &&
               response.Value is List<Item> items &&
               items.Count > 0)
            {
                await Task.Run(() =>
                {
                    Parallel.ForEach(items, item =>
                    {
                        if (!string.IsNullOrWhiteSpace(item.Base64String))
                        {
                            try
                            {
                                var bytes = Convert.FromBase64String(item.Base64String );

                                item.ImageSourceCached =ImageSource.FromStream(() => new MemoryStream(bytes));
                            }
                            catch
                            {
                                item.ImageSourceCached = null;
                            }
                        }
                    });
                });

                //await ItemCacheService.SaveAsync(items);
                await ItemCacheService.SaveAsync(items,Common.mUser.Id);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"LoadAllItems Error : {ex.Message}");
        }
        //finally
        //{
        //    await PopLoaderService.HideAsync();
        //}
    }
    #endregion

    #region [ Events ]
    private void Category_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grdSender &&
            grdSender.BindingContext is Category categories &&
            categories != null &&
            categories.Id > 0)
        {
            Navigation.PushAsync(new CategoryDetailListPage(categories.Id, categories));
        }
    }

    private void OpenCartItem_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new AddToCartPage());
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string search = e.NewTextValue?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(search))
            {
                clsCategory.ItemsSource = _allCategories;
                return;
            }

            var filtered = _allCategories
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) &&
                            x.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

            clsCategory.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }

    //private async void OnRefreshing(object sender, EventArgs e)
    //{
    //    OrderRefreshView.IsRefreshing = true;
    //    await PopLoaderService.ShowAsync();
    //    await GetActiveCategories(forceRefresh: true);
    //    await PopLoaderService.HideAsync();
    //    OrderRefreshView.IsRefreshing = false;
    //}

    private async void OnRefreshing(object sender, EventArgs e)
    {
        OrderRefreshView.IsRefreshing = true;
        await PopLoaderService.ShowAsync();
        await GetActiveCategories();
        await PopLoaderService.HideAsync();
        OrderRefreshView.IsRefreshing = false;

    }

    private async void OnRefreshin1g(object sender, EventArgs e)
    {
        try
        {
            OrderRefreshView.IsRefreshing = true;

            await CategoryCacheService.LoadAsync();

            await GetActiveCategories();
        }
        finally
        {
            OrderRefreshView.IsRefreshing = false;
        }
    }
    #endregion
}



