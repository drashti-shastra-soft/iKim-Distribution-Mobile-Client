using CommunityToolkit.Maui.Behaviors;
using i_KimMobileClient.CustomeControls;
using i_KimMobileClient.Models;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Views;
using Microsoft.Maui.Controls.Shapes;
using Mopups.Services;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Pages;

public partial class HomeTabbedPage : ContentPage
{
    public static HomeTabbedPage? Current { get; private set; }
    private readonly Stack<TabType> _tabHistory = new();
    private TabType _currentTab = TabType.Home;

    public static event Action<Order>? OnEditOrder;

    private readonly NavBarDrawable _navDrawable = new();

    private readonly Dictionary<TabType, int> _tabIndex = new()
{
    { TabType.Home, 0 },
    { TabType.Orders, 1 },
    { TabType.Add, 2 },
    { TabType.Delivery, 3 },
    { TabType.Profile, 4 }
};

    private View? _homeView;
    private View? _ordersView;
    private NewOrderView? _addView;
    private NewPartyOrderView? _addPartyView;
    private View? _deliveryView;
    private View? _profileView;

    public HomeTabbedPage()
    {
        InitializeComponent();
        LoaderService.Register(this);
        NavBarGraphics.Drawable = _navDrawable;
        _navDrawable.ActiveIndex = 0;
        NavBarGraphics.Invalidate();
        SwitchTab(TabType.Home);
        Current = this;
    }

    private async void OnTabTapped(object sender, EventArgs e)
    {
        if (sender is not VerticalStackLayout tab) return;

        if (Enum.TryParse<TabType>(tab.ClassId, out var selectedTab))
        {
            await SwitchTab(selectedTab);
        }
    }

    //  Separate handler for Add floating button
    private async void OnAddTapped(object sender, EventArgs e)
    {
        //  Animate bounce
        await BtnAdd.ScaleTo(0.9, 80);
        await BtnAdd.ScaleTo(1.0, 80);

        await SwitchTab(TabType.Add);
    }

    public async Task SwitchTab(TabType tabName, bool isEditNavigation = false)
    {
        if (_currentTab != tabName)
        {
            _tabHistory.Push(_currentTab);
        }

        _currentTab = tabName;

        _navDrawable.ActiveIndex = _tabIndex[tabName];
        NavBarGraphics.Invalidate();

        //  Reset all regular tabs
        ResetTab(DotHome, LabelHome, IconHome);
        ResetTab(DotOrders, LabelOrders, IconOrders);
        ResetTab(DotDelivery, LabelDelivery, IconDelivery);
        ResetTab(DotProfile, LabelProfile, IconProfile);

        //  Reset Add button
        BtnAdd.BackgroundColor = Color.FromArgb("#FF6B00");
        LabelAdd.TextColor = Color.FromArgb("#9E9E9E");
        SetIconTint(IconAdd, Colors.White);

        if (tabName == TabType.Add)
        {
            //  Add active state
            LabelAdd.TextColor = Color.FromArgb("#FF6B00");
            LoadView(TabType.Add, isEditNavigation);
            return;
        }

        //  Activate selected regular tab
        var (dot, label, icon) = tabName switch
        {
            TabType.Home => (DotHome, LabelHome, IconHome),
            TabType.Orders => (DotOrders, LabelOrders, IconOrders),
            TabType.Add => (null, LabelAdd, null),
            TabType.Delivery => (DotDelivery, LabelDelivery, IconDelivery),
            TabType.Profile => (DotProfile, LabelProfile, IconProfile),
            _ => (DotHome, LabelHome, IconHome)
        };

        dot.Fill = new SolidColorBrush(Color.FromArgb("#FF6B00"));
        label.TextColor = Color.FromArgb("#FF6B00");
        SetIconTint(icon, Color.FromArgb("#FF6B00"));

        await dot.ScaleTo(1.3, 100);
        await dot.ScaleTo(1.0, 100);

        LoadView(tabName);

    }
    public async Task NavigateToTab(TabType tabType)
    {
        await SwitchTab(tabType);
    }
    private void ResetTab(Ellipse dot, Label label, Image? icon)
    {
        dot.Fill = new SolidColorBrush(Colors.Transparent);
        label.TextColor = Color.FromArgb("#9E9E9E");
        SetIconTint(icon, Color.FromArgb("#161616"));
    }

    private void SetIconTint(Image? icon, Color color)
    {
        if (icon == null) return;
        var existing = icon.Behaviors.OfType<IconTintColorBehavior>().FirstOrDefault();
        if (existing != null) icon.Behaviors.Remove(existing);
        icon.Behaviors.Add(new IconTintColorBehavior { TintColor = color });
    }

    private void LoadView(TabType tabName, bool isEditNavigation = false)
    {
        View view;

        if (tabName == TabType.Home)
        {
            if (_homeView == null)
                _homeView = new DashboardView();
            view = _homeView;
        }
        else if (tabName == TabType.Orders)
        {
            if (_ordersView == null)
                _ordersView = new OrderView();
            view = _ordersView;
        }
        else if (tabName == TabType.Add)
        {
            if (_addView == null)
                _addView = new NewOrderView();
            
            view = _addView;
        }
        else if (tabName == TabType.Delivery)
        {
            if (_deliveryView == null)
                _deliveryView = new DeliveryView();
            view = _deliveryView;
        }
        else if (tabName == TabType.Profile)
        {
            if (_profileView == null)
                _profileView = new ProfileView();
            view = _profileView;
        }
        else
        {
            if (_homeView == null)
                _homeView = new DashboardView();
            view = _homeView;
        }

        GRDMainView.Clear();
        GRDMainView.Add(view);
    }

    public async Task EditOrder(Order order)
    {
        _addView ??= new NewOrderView();

        // Use SwitchTab but skip reset by passing flag
        await SwitchTab(TabType.Add, isEditNavigation: true);
    }
    public async Task EditPartyOrder(Order order)
    {
        _addPartyView ??= new NewPartyOrderView();

        //await _addPartyView.EditOrder(order);

        // Use SwitchTab but skip reset by passing flag
        await SwitchTab(TabType.Add, isEditNavigation: true);
    }

    protected override bool OnBackButtonPressed()
    {
        // If user is on Home tab
        //        if (_currentTab == TabType.Home)
        //        {
        //            MainThread.BeginInvokeOnMainThread(async () =>
        //            {
        //                bool exit = await DisplayAlert(
        //                    "Exit App",
        //                    "Are you sure you want to exit?",
        //                    "Yes",
        //                    "No");

        //                if (exit)
        //                {
        //#if ANDROID
        //                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        //#endif
        //                }
        //            });

        //            // Prevent default back action
        //            return true;
        //        }

        if (_currentTab == TabType.Home)
        {
            Dispatcher.Dispatch(async () =>
            {
                var popup = new CommonPopupPage(isExit: true);

                popup.logoutEvent += async (s, e) =>
                {
                    await MopupService.Instance.PopAsync();

#if ANDROID
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#endif
                };

                await MopupService.Instance.PushAsync(popup);
            });

            return true;
        }

        // Navigate to previous tab
        if (_tabHistory.Count > 0)
        {
            var previousTab = _tabHistory.Pop();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await SwitchTab(previousTab);
            });

            return true;
        }

        return base.OnBackButtonPressed();
    }

    //public async void ShowPageLoader(string message = "Loading...")
    //{
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        await PopLoaderService.ShowAsync();
    //    });
    //}

    //public async void HidePageLoader()
    //{
    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        await PopLoaderService.HideAsync();
    //    });
    //}
}