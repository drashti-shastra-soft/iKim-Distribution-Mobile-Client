using CommunityToolkit.Maui.Views;
using i_KimMobileClient.APIService;
using i_KimMobileClient.CustomeControls;
using i_KimMobileClient.Models;
using i_KimMobileClient.PopupPages;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using Newtonsoft.Json;
using System.Runtime.InteropServices.Marshalling;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Pages;

public partial class AddPartyOrderPage : ContentPage
{
    public PartyOrderLister mPartyOrderLister = new();
    public bool isWithWaiter = false;
    PartyOrder mPartyOrder = new();
    bool _isEdit = false;

    public decimal _totalPrice;
    private decimal _grandTotal = 0;
    private int _waiterCharge = 0;
    private List<Item> _allCategories = new();
    public List<Item> _mItems = new List<Item>();
    public List<PartyOrderLine> _mPartyOrderLines = new List<PartyOrderLine>();
    private bool _isPartyOrderEditable = true;
    public AddPartyOrderPage(PartyOrder partyOrder = null, bool isEdit = false)
    {
        InitializeComponent();
        mPartyOrder = partyOrder;
        _isEdit = isEdit;
        BindingContext = this;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        dpDeliveryDate.MinimumDate = DateTime.Today;
        //dpDeliveryDate.Date = DateTime.Today;

        if (_isEdit)
        {
            CheckEditPermission();
            LblTitle.Text = "Edit Party Order";
            FillObjectToControl();
        }
        else
            LblTitle.Text = "Add Party Order";


        

        //await GetProductList();
        //await GetPartyOrderLines();

        //if (_isEdit)
        //{
        //    MapOrderLinesToItems();
        //}
    }

    //private void MapOrderLinesToItems()
    //{
    //    if (mPartyOrder?.Lines == null || mPartyOrder.Lines.Count == 0)
    //        return;

    //    foreach (var item in _mItems)
    //    {
    //        item.DeleteIconVisibility =
    //            mPartyOrder.Lines.Any(x => x.ItemId == item.Id);
    //    }

    //    TotalItem = 0;

    //    foreach (var line in mPartyOrder.Lines)
    //    {
    //        // Find matching item from product list by ItemId
    //        var matchedItem = _mItems.FirstOrDefault(x => x.Id == line.ItemId);

    //        if (matchedItem != null)
    //        {
    //            // Set quantity from edited order line
    //            matchedItem.QuntityUnit = line.Quantity;
    //            matchedItem.NumberOfQuntity = line.Quantity;

    //            if (line.Quantity > 0)
    //                TotalItem++;
    //        }
    //    }

    //    // Refresh CollectionView to show updated quantities
    //    //clsPartyOrderList.ItemsSource = null;
    //    _allCategories = _mItems;
    //    //clsPartyOrderList.ItemsSource = _mItems;

    //    // Update totals display
    //    UpdateTotals();
    //}

    private void FillObjectToControl()
    {
        txtName.Text = mPartyOrder.Name;
        txtMobileNumber.Text = mPartyOrder.MobileNumber;
        EntryLocation.Text = mPartyOrder.DeliveryLocation;
        dpDeliveryDate.Date = mPartyOrder.DeliveryDateTime;
        tpDeliveryTime.Time = mPartyOrder.DeliveryDateTime.TimeOfDay;
        txtCups.Text = mPartyOrder.CupsDisplay.ToString();
        txtSpoons.Text = mPartyOrder.SpoonsDisplay.ToString();
        txtGlass.Text = mPartyOrder.GlassDisplay.ToString();
        EntryNarration.Text = mPartyOrder.Narration;

        chkWithWaiter.IsChecked = mPartyOrder.WithWaiter?.Equals("Yes", StringComparison.OrdinalIgnoreCase) == true;

        if (mPartyOrder.WithWaiter == "Yes")
        {
            ClearControl();
            isWithWaiter = true;
            vrtStkWaiterCharge.IsVisible = true;
            bdrYes.Background = Color.FromArgb("#FF7A1A");
            lblYes.TextColor = Colors.White;

            txtWaiterCharge.Text = mPartyOrder.WaiterCharge?.ToString() ?? "0";
            _waiterCharge = mPartyOrder.WaiterCharge ?? 0;
        }
        else
        {
            isWithWaiter = false;
            vrtStkWaiterCharge.IsVisible = false;
            bdrNo.Background = Color.FromArgb("#FF7A1A");
            lblNo.TextColor = Colors.White;
        }
    }

    //private async Task GetProductList()
    //{
    //    try
    //    {
    //        await PopLoaderService.ShowAsync();
    //        string url = string.Format(EndPointsUrl.GetAllActiveItemsGetOrders, Common.mUser.PricingTierId);
    //        var mResponse = await new APIService.ApiServiceHandler().GetAsync<List<Item>>(url, Common.mUser.Token);
    //        if (mResponse != null && mResponse.IsSuccess)
    //        {
    //            if (mResponse.Value != null &&
    //                mResponse.Value is List<Item> mobjItems &&
    //                mobjItems != null &&
    //                mobjItems.Count > 0)
    //            {
    //                //stkRecordNotFound.IsVisible = false;
    //                _mItems = mobjItems;
    //                foreach (var item in _mItems)
    //                {

    //                    item.PartyOrderLineId = mPartyOrder != null ? mPartyOrder.PartyOrderLineId : 0;
    //                    item.DeleteIconVisibility = _isEdit &&
    //                                            mPartyOrder?.Lines?.Any(x => x.ItemId == item.Id) == true;
    //                }

    //                //clsPartyOrderList.ItemsSource = _mItems;
    //                _allCategories = _mItems;
    //            }
    //            else
    //            {
    //                //stkRecordNotFound.IsVisible = true;
    //            }
    //        }
    //        else
    //        {
    //            Common.ErrorMessage(mResponse.ErrorMessage);
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

    private void BdrAddItem_Tapped(object sender, TappedEventArgs e)
    {

    }

    private int TotalItem = 0;
    private void BdrPlus_Tapped(object sender, TappedEventArgs e)
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

    private void BdrMinus_Tapped(object sender, TappedEventArgs e)
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

    private async void BdrDelete_Tapped(object sender, TappedEventArgs e)
    {
        #region [ Old ]
        //if (sender is Border border &&
        //        border.BindingContext is Item item)
        //{
        //    //if (item.PartyOrderLineId <= 0)
        //    if (item.Id <= 0)
        //        return;

        //    bool result = await DisplayAlert(
        //        "Delete",
        //        "Are you sure you want to delete this item?",
        //        "Yes",
        //        "No");

        //    if (!result)
        //        return;

        //    var line = mPartyOrder.Lines
        //        .FirstOrDefault(x => x.Id == item.PartyOrderLineId);

        //    if (line != null)
        //    {
        //        mPartyOrder.Lines.Remove(line);

        //        item.QuntityUnit = 0;
        //        item.NumberOfQuntity = 0;
        //        item.DeleteIconVisibility = false;

        //        clsPartyOrderList.ItemsSource = null;
        //        clsPartyOrderList.ItemsSource = _mItems;

        //        UpdateTotals();
        //    }
        //} 
        #endregion

        if (sender is Border bdrDeleteAction &&
            bdrDeleteAction.BindingContext is Item item &&
            item != null && item.Id > 0)
        {
            if (!_isEdit || !_isPartyOrderEditable) return;
            //await MopupService.Instance.PushAsync(new CommonPopupPage());
            var data = new CommonPopupPage(true);
            data.logoutEvent += async (s1, e1) =>
            {
                await MopupService.Instance.PopAsync();
                await DeleteOrder(item.PartyOrderLineId);
                //await DeleteOrder(item.Id);
            };
            await MopupService.Instance.PushAsync(data);
        }
    }

    private async Task DeleteOrder(int partyOrderLineId)
    {
        try
        {
            //if (!Common.CheckInternetConnection())
            //{
            //    Common.InfoMessage("Please check your internet connection and try again.");
            //    return;
            //}

            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.DeletePartyOrderLine, partyOrderLineId, Common.mUser.Id);
            var mResponse = await new APIService.ApiServiceHandler().DeleteAsync<Response>(url, Common.mUser.Token);
            if (mResponse != null && mResponse.IsSuccess)
            {
                var deletedItem = _mItems
                .FirstOrDefault(x => x.OrderLineId == partyOrderLineId);

                if (deletedItem != null)
                {
                    deletedItem.NumberOfQuntity = 0;
                    deletedItem.QuntityUnit = 0;
                }

                UpdateTotals();

                //clsPartyOrderList.ItemsSource = null;
                _allCategories = _mItems;
                //clsPartyOrderList.ItemsSource = _mItems;

                Common.SuccessMessage("Order delete successfully.");
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

    private void UpdateTotals()
    {
        _grandTotal = (decimal)_mItems.Where(x => x.NumberOfQuntity > 0).Sum(x => x.Price * x.QuntityUnit);

        decimal finalTotal = isWithWaiter ? _grandTotal + _waiterCharge : _grandTotal;

        //LblTotalAmount.Text = $"{finalTotal:0.00}";
        //LblTotalItems.Text = TotalItem.ToString();

        //LblTotalAmount.Text = $"{_grandTotal.ToString("0.00")}";
        //LblTotalItems.Text = TotalItem.ToString();
    }

    private async void OnPlaceOrderTapped(object sender, TappedEventArgs e)
    {
        await SavePartyOrder();
    }

    private void BdrWaiterSelection_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is not Border border) return;

        // Reset both
        bdrNo.Background = Colors.Transparent;
        bdrYes.Background = Colors.Transparent;

        lblNo.TextColor = Colors.Black;
        lblYes.TextColor = Colors.Black;

        bool tappedYes = border.ClassId == "Yes";

        isWithWaiter = tappedYes;
        vrtStkWaiterCharge.IsVisible = tappedYes;

        if (tappedYes)
        {
            bdrYes.Background = Color.FromArgb("#FF7A1A");
            lblYes.TextColor = Colors.White;
            //If charge already entered, parse it back (in case re-toggled)
            int.TryParse(txtWaiterCharge.Text, out _waiterCharge);
        }
        else
        {
            bdrNo.Background = Color.FromArgb("#FF7A1A");
            lblNo.TextColor = Colors.White;

            txtWaiterCharge.Text = string.Empty;
            _waiterCharge = 0;
        }
        UpdateTotals();
    }

    private void ClearControl()
    {
        bdrYes.Background = Colors.Transparent;
        bdrNo.Background = Colors.Transparent;
        lblYes.TextColor = Color.FromArgb("#000000");
        lblNo.TextColor = Color.FromArgb("#000000");
    }

    bool isExpnad = false;
    //private void BdrtDetailExpand_Tapped(object sender, TappedEventArgs e)
    //{
    //    if (!isExpnad)
    //    {
    //        stkDetailPart.IsVisible = false;
    //        isExpnad = true;
    //        imgArrow.Rotation = 90;
    //    }
    //    else
    //    {
    //        stkDetailPart.IsVisible = true;
    //        isExpnad = false;
    //        imgArrow.Rotation = -90;
    //    }
    //}

    private bool ValidateOrder()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            Common.ErrorMessage("Please enter name.");
            return false;
        }

        // Get only digits — ignore +91 prefix
        var mobile = txtMobileNumber.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(mobile))
        {
            Common.ErrorMessage("Please enter mobile number.");
            return false;
        }

        // Validate only 10 digits (entry already strips prefix)

        if (mobile.Length != 10 || !mobile.All(char.IsDigit))
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

        if (isWithWaiter)
        {
            if (string.IsNullOrWhiteSpace(txtWaiterCharge.Text))
            {
                Common.ErrorMessage("Please enter waiter charge.");
                return false;
            }

            if (!decimal.TryParse(txtWaiterCharge.Text, out decimal waiterCharge) || waiterCharge <= 0)
            {
                Common.ErrorMessage("Please enter a valid waiter charge.");
                return false;
            }
        }


        return true;
    }

    private async Task SavePartyOrder()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            if (_isEdit && !_isPartyOrderEditable)
            {
                Common.ErrorMessage("Order can only be edited until one day before delivery date.");
                return;
            }

            if (!ValidateOrder())
                return;

            await PopLoaderService.ShowAsync();
            var orderToSave = new PartyOrder()
            {
                Id = _isEdit ? mPartyOrder.Id : 0,
                StoreId = Common.mUser.StoreId.Value,
                Name = txtName.Text.Trim(),
                MobileNumber = $"+91 {txtMobileNumber.Text?.Trim()}",
                //MobileNumber = txtMobileNumber.Text.Trim(),
                //MobileNumber = $"{91} {txtMobileNumber.Text?.Replace(" ", "") ?? ""}",
                DeliveryLocation = EntryLocation.Text,
                DeliveryDateTime = GetDeliveryDateTime(),
                NumOfCups = int.TryParse(txtCups.Text, out var cups) ? cups : null,
                NumOfSpoons = int.TryParse(txtSpoons.Text, out var spoons) ? spoons : null,
                NumOfGlass = int.TryParse(txtGlass.Text, out var glass) ? glass : null,
                //WithWaiter = isWithWaiter ? "Yes" : "No",
                WithWaiter = chkWithWaiter.IsChecked ? "Yes" : "No",
                Narration = EntryNarration.Text,
                WaiterCharge = int.TryParse(txtWaiterCharge.Text, out int waiterCharge) ? waiterCharge : null,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id,
            };

            //var selectedItems = _mItems.Where(x => x.NumberOfQuntity > 0).ToList();

            //if (selectedItems.Count == 0)
            //{
            //    Common.ErrorMessage("Please select at least one item.");
            //    return;
            //}

            //foreach (var item in selectedItems)
            //{
            //    // Check if this item already existed in original order (for edit)

            //    var existingLine = _isEdit ? mPartyOrder.Lines.FirstOrDefault(x => x.ItemId == item.Id) : null;

            //    orderToSave.Lines.Add(new PartyOrderLine()
            //    {
            //        Id = existingLine?.Id ?? 0,
            //        PartyOrderId = _isEdit ? mPartyOrder.Id : 0, // Add this
            //        ItemId = item.Id,
            //        ItemName = item.Name,
            //        //Quantity = item.QuntityUnit,
            //        Quantity = item.NumberOfQuntity,
            //        Rate = item.Price,
            //        //QuantityRequested = item.NumberOfQuntity,
            //        Total = (item.QuntityUnit * item.Price) + _waiterCharge,
            //        //Total = (item.QuntityUnit * item.Price),
            //        UnitOfMeasureId = item.UnitOfMeasureId,
            //        UnitOfMeasure = item.UnitOfMeasure,
            //        CreatedBy = Common.mUser.Id,
            //        LastModifiedBy = Common.mUser.Id
            //    });
            //}

            //if (orderToSave.Lines.Count == 0)
            //{
            //    Common.ErrorMessage("Please select at least one item.");
            //    return;
            //}

            //if (mPartyOrder.Lines.Count == 0)
            //{
            //    Common.ErrorMessage("Please select at least one item.");
            //    return;
            //}

            var mResponse = await new APIService.ApiServiceHandler().PostAsync<PartyOrder>(EndPointsUrl.SavePartyOrder, orderToSave, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage(_isEdit ? "Order updated successfully!" : "Order created successfully!");

                if (_isEdit)
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    ClearData();

                    var partyOrder = JsonConvert.DeserializeObject<PartyOrder>(JsonConvert.SerializeObject(mResponse.Value));

                    int partyOrderId = partyOrder.Id;

                    await Navigation.PushAsync(new AddPartyOrderItemPage(partyOrderId, partyOrder));
                }
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"{ex.Message}");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

    public void ClearData()
    {

        // Clear after save
        txtName.Text = string.Empty;
        txtMobileNumber.Text = string.Empty;
        EntryLocation.Text = string.Empty;
        dpDeliveryDate.Date = DateTime.Today;
        tpDeliveryTime.Time = TimeSpan.Zero;
        txtCups.Text = string.Empty;
        txtGlass.Text = string.Empty;
        txtSpoons.Text = string.Empty;
        chkWithWaiter.IsChecked = false;
        EntryNarration.Text = string.Empty;
        txtWaiterCharge.Text = string.Empty;
    }

    private DateTime GetDeliveryDateTime()
    {
        var date = dpDeliveryDate.Date;
        var time = tpDeliveryTime.Time;

        return date.Add(time);
    }

    private bool _isFormatting;
    //private void OnMobileNumberChanged(object sender, TextChangedEventArgs e)
    //{
    //    try
    //    {
    //        if (sender is not BorderlessEntry entry)
    //            return;

    //        var digits = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());

    //        if (digits.Length > 10)
    //            digits = digits.Substring(0, 10);

    //        if (entry.Text != digits)
    //            entry.Text = digits;
    //    }
    //    catch (Exception ex)
    //    {
    //        Common.ErrorMessage($"{ex.Message}");
    //    }
    //    finally
    //    {
    //        _isFormatting = false;
    //    }
    //}

    private void OnMobileNumberChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (sender is not BorderlessEntry entry)
                return;

            var value = e.NewTextValue ?? "";

            // Remove +91 when loading an existing number
            if (value.StartsWith("+91"))
            {
                value = value.Substring(3).Trim();
            }

            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length > 10)
                digits = digits.Substring(0, 10);

            if (entry.Text != digits)
                entry.Text = digits;
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"{ex.Message}");
        }
    }

    private void UpdateDeliveryDateTimeLabel()
    {
        var deliveryDateTime = GetDeliveryDateTime();

        //LblDateTime.Text = deliveryDateTime.ToString("dd MMM yyyy");
        //LblTime.Text = deliveryDateTime.ToString("HH:mm");
    }

    private void CheckEditPermission()
    {
        if (!_isEdit || mPartyOrder == null)
            return;

        // T-1 logic
        DateTime lastEditableDate = mPartyOrder.DeliveryDateTime.Date.AddDays(-1);

        _isPartyOrderEditable = DateTime.Today <= lastEditableDate;
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

    private async void BackButton_Clicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void txtWaiterCharge_TextChanged(object sender, TextChangedEventArgs e)
    {
        int.TryParse(e.NewTextValue, out _waiterCharge);
        UpdateTotals();
    }

    private void ChkWithWaiter_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        isWithWaiter = e.Value;

        vrtStkWaiterCharge.IsVisible = e.Value;

        if (!e.Value)
        {
            txtWaiterCharge.Text = string.Empty;
            _waiterCharge = 0;
        }
    }

    private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string search = e.NewTextValue?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(search))
            {
                //clsPartyOrderList.ItemsSource = _allCategories;
                return;
            }

            var filtered = _allCategories
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) &&
                            x.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

            //clsPartyOrderList.ItemsSource = filtered;
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }
}