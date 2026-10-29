using System.Xml.Linq;
using i_KimMobileClient.APIService;
using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Views;

public partial class NewPartyOrderView : ContentView
{

    private List<PartyOrder> _mPartyOrders = new();
    public PartyOrderLister mPartyOrderLister = new();
    public bool isWithWaiter = false;
    public NewPartyOrderView()
    {
        InitializeComponent();
        GetProductList();
    }


    //private async Task GetPartyOrders()
    //{
    //    try
    //    {
    //        await PopLoaderService.ShowAsync();
    //        var mResponse = await new ApiServiceHandler().PostAsync<PartyOrderLister>(EndPointsUrl.GetPartyOrders, mPartyOrderLister, Common.AuthToken);
    //        if (mResponse != null && mResponse.IsSuccess)
    //        {
    //            if (mResponse.Value != null && mResponse.Value is PartyOrderLister mPartyOrderLister && mPartyOrderLister.List != null && mPartyOrderLister.List.Count > 0)
    //            {
    //                foreach (var item in mPartyOrderLister.List)
    //                {
    //                    var url = string.Format(EndPointsUrl.GetPartyOrderLineById, item.Id);
    //                    var mOrderlineResponse = await new ApiServiceHandler().GetAsync<List<PartyOrderLine>>(url, Common.AuthToken);
    //                    if (mOrderlineResponse != null && mOrderlineResponse.IsSuccess)
    //                    {
    //                        item.Lines = (List<PartyOrderLine>)mOrderlineResponse.Value;
    //                    }
    //                }
    //                _mPartyOrders = mPartyOrderLister.List;
    //                clsPartyOrderList.ItemsSource = null;
    //                clsPartyOrderList.ItemsSource = _mPartyOrders;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Common.ErrorMessage(ex.Message);
    //    }
    //    finally
    //    {
    //        await PopLoaderService.HideAsync();
    //    }
    //}

    public List<Item> _mItems = new List<Item>();
    private async Task GetProductList()
    {
        try
        {
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetAllActiveItemsGetOrders, Common.mUser.PricingTierId);
            var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<PartyOrderLine>>(url, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null && mResponse.Value is List<PartyOrderLine> mobjItems && mobjItems != null && mobjItems.Count > 0)
                {
                    //_mItems = mobjItems;
                    //_mPartyOrders = mobjItems;
                    clsPartyOrderList.ItemsSource = mobjItems;
                }
                else
                {

                }
            }
            else
            {
                Common.ErrorMessage(mResponse.ErrorMessage);
            }
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
    private void BdrAddItem_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BdrPlus_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BdrMinus_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BdrDelete_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void OnPlaceOrderTapped(object sender, TappedEventArgs e)
    {
        await SavePartyOrder();
    }

    private void BdrWaiterSelection_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border bdrWaiterSelection)
        {
            ClearControl();
            if (bdrWaiterSelection.ClassId == "Yes")
            {
                bdrYes.Background = Color.FromArgb("#FF7A1A");
                lblYes.TextColor = Color.FromArgb("#FFFFFF");
                isWithWaiter = true;
            }
            else
            {
                bdrNo.Background = Color.FromArgb("#FF7A1A");
                lblNo.TextColor = Color.FromArgb("#FFFFFF");
                isWithWaiter = false;
            }
        }
    }

    private void ClearControl()
    {
        bdrYes.Background = bdrNo.Background = Colors.Transparent;
        lblYes.TextColor = lblNo.TextColor = Color.FromArgb("#000000");
    }

    bool isExpnad = false;
    private void BdrtDetailExpand_Tapped(object sender, TappedEventArgs e)
    {
        if (!isExpnad)
        {
            stkDetailPart.IsVisible = false;
            isExpnad = true;
            imgArrow.Rotation = 90;
        }
        else
        {
            stkDetailPart.IsVisible = true;
            isExpnad = false;
            imgArrow.Rotation = -90;
        }
    }

    private bool ValidateOrder()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            Common.ErrorMessage("Please enter name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtMobileNumber.Text))
        {
            Common.ErrorMessage("Please enter mobile number.");
            return false;
        }

        if (txtMobileNumber.Text.Length != 10)
        {
            Common.ErrorMessage("Please enter a valid 10-digit mobile number.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(EntryLocation.Text))
        {
            Common.ErrorMessage("Please enter delivery location.");
            return false;
        }

        if (dpDeliveryDate.Date == DateTime.MinValue)
        {
            Common.ErrorMessage("Please select delivery date.");
            return false;
        }

        if (tpDeliveryTime.Time == TimeSpan.Zero)
        {
            Common.ErrorMessage("Please select delivery time.");
            return false;
        }
        if (!int.TryParse(txtCups.Text, out _))
        {
            Common.ErrorMessage("Please enter valid number of cups.");
            return false;
        }

        if (!int.TryParse(txtSpoons.Text, out _))
        {
            Common.ErrorMessage("Please enter valid number of spoons.");
            return false;
        }

        if (!int.TryParse(txtGlass.Text, out _))
        {
            Common.ErrorMessage("Please enter valid number of glasses.");
            return false;
        }

        return true;
    }

    private async Task SavePartyOrder()
    {
        try
        {
            if (!ValidateOrder())
                return;

            var mPartyOrder = new PartyOrder()
            {
                StoreId = Common.mUser.StoreId.Value,
                Name = txtName.Text,
                MobileNumber = txtMobileNumber.Text,
                DeliveryLocation = EntryLocation.Text,
                DeliveryDateTime = GetDeliveryDateTime(),
                NumOfCups = int.TryParse(txtCups.Text, out var cups) ? cups : null,
                NumOfSpoons = int.TryParse(txtSpoons.Text, out var spoons) ? spoons : null,
                NumOfGlass = int.TryParse(txtGlass.Text, out var glass) ? glass : null,
                WithWaiter = isWithWaiter ? "Yes" : "No",
                Narration = EntryNarration.Text,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id,
            };

            var selectedItems = _mPartyOrders.Where(x => x.NumberOfQuntity > 0).ToList();

            if (selectedItems.Count == 0)
            {
                Common.ErrorMessage("Please select at least one item.");
                return;
            }

            //foreach (var item in selectedItems)
            //{
            //    mPartyOrder.Lines.Add(new PartyOrderLine()
            //    {
            //        ItemId = item.Id,
            //        ItemName = item.Name,
            //        Quantity = item.QuntityUnit,
            //        Rate = 0,
            //        Total = item.TotalAmount,
            //        CreatedBy = Common.mUser.Id,
            //        LastModifiedBy = Common.mUser.Id
            //    });
            //}

            //if (mPartyOrder.Lines.Count == 0)
            //{
            //    Common.ErrorMessage("Please select at least one item.");
            //    return;
            //}

            var mResponse = await new APIService.ApiServiceHandler().PostAsync<PartyOrder>(EndPointsUrl.SavePartyOrder, mPartyOrder, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Order created successfully!");
                //ResetAllItems();
                await Common.GetHomeTabbedPage(this)?.NavigateToTab(TabType.Orders);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"{ex.Message}");
        }
    }

    private DateTime GetDeliveryDateTime()
    {
        var date = dpDeliveryDate.Date;
        var time = tpDeliveryTime.Time;

        return date.Add(time);
    }


    private void UpdateDeliveryDateTimeLabel()
    {
        var deliveryDateTime = GetDeliveryDateTime();

        //LblDateTime.Text = deliveryDateTime.ToString("dd MMM yyyy");
        //LblTime.Text = deliveryDateTime.ToString("HH:mm");
    }

    private void dpDeliveryDate_DateSelected(object sender, DateChangedEventArgs e)
    {
        //UpdateDeliveryDateTimeLabel();
    }

    private void tpDeliveryTime_TimeSelected(object sender, TimeChangedEventArgs e)
    {
        //UpdateDeliveryDateTimeLabel();
    }

    private void OnDateTapped(object sender, TappedEventArgs e)
    {
        dpDeliveryDate.Focus();
        dpDeliveryDate.IsVisible = true;
        //UpdateDeliveryDateTimeLabel();
    }

    private void OnDateTimeTapped(object sender, TappedEventArgs e)
    {
        tpDeliveryTime.Focus();
        tpDeliveryTime.IsVisible = true;
        //UpdateDeliveryDateTimeLabel();
    }

    private void OnFilterTapped(object sender, TappedEventArgs e)
    {

    }

    private void BdrSelectedObject_Tapped(object sender, TappedEventArgs e)
    {

    }
}