using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace i_KimMobileClient.Pages.Daily_Sales;

public partial class AddDailySalesPage : ContentPage
{
    private bool _isFromDashboard;

    DailySales _mDailySales = new();
    //public AddDailySalesPage()
    //{
    //    InitializeComponent();

    //    //dpDate.MinimumDate = DateTime.Today;
    //    //dpDate.MaximumDate = DateTime.Today;
    //    //dpDate.Date = DateTime.Today;
    //}


    public AddDailySalesPage(DateTime salesDate, bool isFromDashboard = false)
    {
        InitializeComponent();

        dpDate.Date = salesDate;
        _isFromDashboard = isFromDashboard;
        // Subtitle
        if (salesDate.Date == DateTime.Today)
        {
            lblSubTitle.Text = "Enter Today's Sales Collection";
        }
        else
        {
            lblSubTitle.Text = $"Enter {salesDate:dd/MM/yyyy} Sales Collection";
        }

        if (_isFromDashboard)
        {
            //lblSubTitle.Text = $"Enter {salesDate:dd/MM/yyyy} Sales Collection";
            BdrBtnBack.IsVisible = false;
        }
    }

    public AddDailySalesPage()
    {
        InitializeComponent();

        dpDate.Date = DateTime.Today;
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddDailySalesTapped(object sender, TappedEventArgs e)
    {
        await AddDailySales();
    }

    private bool ValidateSales()
    {

        if (dpDate.Date == DateTime.MinValue)
        {
            Common.ErrorMessage("Please select Sales date.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtCashColl.Text))
        {
            Common.ErrorMessage("Please Enter Cash Amount.");
            return false;
        }

        if (!decimal.TryParse(txtCashColl.Text.Trim(), out decimal cashAmount))
        {
            Common.ErrorMessage("Please Enter a valid Cash Amount.");
            return false;
        }

        if (cashAmount < 0)
        {
            Common.ErrorMessage("Cash Amount cannot be negative.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtOnlineColl.Text))
        {
            Common.ErrorMessage("Please Enter Online Amount.");
            return false;
        }

        if (!decimal.TryParse(txtOnlineColl.Text.Trim(), out decimal onlineAmount))
        {
            Common.ErrorMessage("Please Enter a valid Online Amount.");
            return false;
        }

        if (onlineAmount < 0)
        {
            Common.ErrorMessage("Online Amount cannot be negative.");
            return false;
        }


        return true;
    }


    private async Task AddDailySales()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            if (!ValidateSales())
                return;

            //await PopLoaderService.ShowAsync();
            await PopLoaderService.ShowAsync();

            var orderToSave = new DailySales()
            {
                //Id = 0,
                StoreId = Common.mUser.StoreId.Value,
                Date = GetSalesDate(),
                Cash = decimal.TryParse(txtCashColl.Text, out var cash) ? cash : null,
                Online = decimal.TryParse(txtOnlineColl.Text, out var online) ? online : null,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id,
            };

            var mResponse = await new APIService.ApiServiceHandler()
                .PostAsync<DailySales>(
                    EndPointsUrl.AddDailySales,
                    orderToSave,
                    Common.AuthToken);

           
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Daily Sales Added Successfully!");

                ClearData();
                //if (_isFromDashboard)
                //{
                    await Navigation.PopAsync();
                //}
                //else
                //{
                //    await Navigation.PushAsync(new DailySalesListPage());
                //}
            }
            else
            {
                Common.ErrorMessage(
                    mResponse?.ErrorMessage ?? "Failed to add Daily Sales."
                );
            }


        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
        finally
        {
            //await PopLoaderService.HideAsync();
            await PopLoaderService.HideAsync();
        }
    }


    public void ClearData()
    {

        // Clear after save
        txtCashColl.Text = string.Empty;
        txtOnlineColl.Text = string.Empty;
        dpDate.Date = DateTime.Today;
    }

    private DateTime GetSalesDate()
    {
        var date = dpDate.Date;

        return date;
    }


}