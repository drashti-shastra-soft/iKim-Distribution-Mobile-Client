namespace i_KimMobileClient.Pages.Dashboard;
using System.Collections;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Microcharts;
using SkiaSharp;

public partial class DashboardPage : ContentPage
{
    private int _currentPosition = 0;
   

    public DashboardPage()
    {
        InitializeComponent();
    }

    private void StartAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            if (bannerCarousel.ItemsSource is IList items &&
                items.Count > 0)
            {
                _currentPosition++;

                if (_currentPosition >= items.Count)
                    _currentPosition = 0;

                bannerCarousel.Position = _currentPosition;
            }

            return true;
        });
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        StartAutoScroll();
    }

    private void VtkStkQuikAction_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VerticalStackLayout stkQuikAction)
        {
            if (stkQuikAction.ClassId == "NewOrder")
            {

            }
            else if (stkQuikAction.ClassId == "ViewOrders")
            {

            }
            else if (stkQuikAction.ClassId == "Deliveries")
            {

            }
            else if (stkQuikAction.ClassId == "Payments")
            {

            }
        }
    }
}