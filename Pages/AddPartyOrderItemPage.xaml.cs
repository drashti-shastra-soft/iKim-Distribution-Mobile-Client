using i_KimMobileClient.Models;
using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using i_KimMobileClient.Views;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Xml.Linq;

namespace i_KimMobileClient.Pages;

public partial class AddPartyOrderItemPage : ContentPage
{
    PartyOrderLine mPartyOrderLine = new();
    PartyOrder _mPartyOrder = new();
    public List<Item> _mItems = new List<Item>();
    private int _partyOrderId;
    bool _isEdit = false;
    private bool _isEditLoading = false;

    public AddPartyOrderItemPage(int partyOrderId, PartyOrder partyOrder = null, PartyOrderLine partyOrderLine = null, bool isEdit = false)
    {
        InitializeComponent();
        _partyOrderId = partyOrderId;
        _mPartyOrder = partyOrder;
        mPartyOrderLine = partyOrderLine;
        _isEdit = isEdit;
        imgProductItem.Source = "icon_ice_cream.png";
        BindingContext = this;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        

        await GetProductList();
        if (_isEdit)
        {
            BtnLabel.Text = "Edit Item";
            LblTitle.Text = "Edit Item";
            FillObjectToControl();
        }
        else
        {
            LblTitle.Text = "Add Item";
            BtnLabel.Text = "Add Item";

            ProductView.IsVisible = false;

            _quantity = 0;
            txtQuantity.Text = "0";

            lblSumQuan.Text = "0";
            lblSumRate.Text = "₹0.00";
            txtTotalAmount.Text = "₹0.00";
        }
    }

    //private void FillObjectToControl()
    //{
    //    txtQuantity.Text = mPartyOrderLine.Quantity.ToString();

    //    // Sync quantity variable
    //    _quantity = mPartyOrderLine.Quantity;
    //    txtRate.Text = mPartyOrderLine.Rate.ToString();
    //    txtTotalAmount.Text = mPartyOrderLine.Total.ToString();
    //    txtUnit.Text = mPartyOrderLine.UnitOfMeasure;
    //    itemPicker.SelectedItem = mPartyOrderLine.ItemName;

    //    //var selectedItem = _mItems?.FirstOrDefault(x => x.Name == mPartyOrderLine.ItemName);

    //    var selectedItem = _mItems?.FirstOrDefault(x => x.Id == mPartyOrderLine.ItemId);
    //    if (selectedItem != null)
    //    {
    //        itemPicker.SelectedItem = selectedItem;
    //        txtUnit.Text = selectedItem.UnitOfMeasure;
    //    }
    //}


    private void FillObjectToControl()
    {
        txtRate.Text = mPartyOrderLine.Rate.ToString();

        txtUnit.Text = mPartyOrderLine.UnitOfMeasure;

        var selectedItem = _mItems?
            .FirstOrDefault(x => x.Id == mPartyOrderLine.ItemId);


        if (selectedItem != null)
        {
            itemPicker.SelectedItem = selectedItem;
            txtUnit.Text = selectedItem.UnitOfMeasure;
        }

        txtQuantity.Text = mPartyOrderLine.Quantity.ToString();

        _quantity = mPartyOrderLine.Quantity;

        lblSumQuan.Text =
            $"{mPartyOrderLine.Quantity} {mPartyOrderLine.UnitOfMeasure}";


        lblSumRate.Text =
            $"₹{mPartyOrderLine.Rate:0.00}";


        txtTotalAmount.Text =
            $"₹{mPartyOrderLine.Total:0.00}";

    }
    private async void BackButton_Clicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddItemTapped(object sender, TappedEventArgs e)
    {
        await AddPartyOrderItem();
    }

    private bool ValidateItem()
    {
        if (itemPicker.SelectedItem == null)
        {
            Common.ErrorMessage("Please select item.");
            return false;
        }

        if (!int.TryParse(txtQuantity.Text, out int qty) || qty <= 0)
        {
            Common.ErrorMessage("Please enter a valid quantity.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtQuantity.Text))
        {
            Common.ErrorMessage("Please enter quantity.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtRate.Text))
        {
            Common.ErrorMessage("Please enter rate.");
            return false;
        }

        if (!decimal.TryParse(txtRate.Text, out decimal rate) || rate <= 0)
        {
            Common.ErrorMessage("Please enter a valid rate.");
            return false;
        }

        return true;
    }

    private async Task AddPartyOrderItem()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            if (!ValidateItem())
                return;

            //await PopLoaderService.ShowAsync();
            await PopLoaderService.ShowAsync();

            var selectedItem = itemPicker.SelectedItem as Item;

            //int quantity = int.TryParse(txtQuantity.Text, out var qty) ? qty : 0;

            //decimal rate = decimal.TryParse(txtRate.Text, out var itemRate) ? itemRate : 0;

            int quantity = int.Parse(txtQuantity.Text);
            decimal rate = decimal.Parse(txtRate.Text);

            mPartyOrderLine = new PartyOrderLine()
            {
                Id = _isEdit ? mPartyOrderLine.Id : 0,
                PartyOrderId = _partyOrderId,
                ItemId = selectedItem.Id,
                ItemName = selectedItem.Name,
                Item = selectedItem.Name,
                Quantity = quantity,
                Rate = rate,
                Total = quantity * rate,
                FileName = selectedItem.FileName,
                Base64String = selectedItem.Base64String,
                UnitOfMeasureId = selectedItem.UnitOfMeasureId,
                UnitOfMeasure = selectedItem.UnitOfMeasure,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id
            };


            var mResponse = await new APIService.ApiServiceHandler()
                .PostAsync<PartyOrderLine>(
                    EndPointsUrl.AddPartyOrderItem,
                    mPartyOrderLine,
                    Common.AuthToken);


            if (mResponse != null && mResponse.IsSuccess)
            {
                //Common.SuccessMessage("Item added successfully!");
                Common.SuccessMessage(_isEdit ? "Item updated successfully!" : "Item added successfully!");

                if (_isEdit)
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    await Navigation.PushAsync(new PartyOrderItemPage(_partyOrderId, _mPartyOrder));
                    ClearData();
                }
            }
            else
            {
                Common.ErrorMessage(
                    mResponse?.ErrorMessage ?? "Failed to add item."
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

    #region GetProductList
    private async Task GetProductList()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;
            //await PopLoaderService.ShowAsync();
            await PopLoaderService.ShowAsync();
            string url = string.Format(EndPointsUrl.GetAllActiveItemsGetOrders, Common.mUser.PricingTierId);

            var mResponse = await new APIService.ApiServiceHandler()
                .GetAsync<List<Item>>(url, Common.mUser.Token);

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is List<Item> mobjItems &&
                    mobjItems.Count > 0)
                {
                    _mItems = mobjItems;

                    await Task.Run(() =>
                    {
                        Parallel.ForEach(_mItems, item =>
                        {
                            if (!string.IsNullOrWhiteSpace(item.Base64String))
                            {
                                try
                                {
                                    var bytes = Convert.FromBase64String(
                                        item.Base64String);

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

                    itemPicker.ItemsSource = _mItems;
                }
                else
                {
                    itemPicker.ItemsSource = null;
                    txtUnit.Text = string.Empty;
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
            //await PopLoaderService.HideAsync();
            await PopLoaderService.HideAsync();
        }
    }
    #endregion
    //private void itemPicker_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (itemPicker.SelectedItem is Item selectedItem)
    //    {
    //        txtUnit.Text = selectedItem.UnitOfMeasure;
    //    }
    //    else
    //    {
    //        txtUnit.Text = string.Empty;
    //    }
    //}

    private void itemPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (itemPicker.SelectedItem is Item selectedItem)
        {
            ProductView.IsVisible = true;
            lblTitle.Text = selectedItem.Name.Trim();
            //lblDesc.Text =
            //    selectedItem.Description?.ToString()
            //    ?? string.Empty;

            var description = selectedItem.Description?.ToString().Trim();

            lblDesc.Text = description ?? string.Empty;
            lblDesc.IsVisible = !string.IsNullOrWhiteSpace(description);

            lblPrice.Text =
                $"₹{selectedItem.Price:0.00}/{selectedItem.UnitOfMeasure}";
            imgProductItem.Source =
                selectedItem.ImageSourceCached
                ?? "icon_ice_cream.png";

            txtUnit.Text = selectedItem.UnitOfMeasure;

            _quantity = 0;
            txtQuantity.Text = "0";

            lblSumRate.Text =
                string.IsNullOrWhiteSpace(txtRate.Text)
                ? "₹0.00"
                : $"₹{txtRate.Text}";

            lblSumQuan.Text =
                $"0 {selectedItem.UnitOfMeasure}";


            txtTotalAmount.Text = "₹0.00";


            CalculateTotal();
        }
        else
        {
            ProductView.IsVisible = false;
            lblTitle.Text = "";
            lblDesc.Text = "";
            lblPrice.Text = "";
            imgProductItem.Source = "icon_ice_cream.png";
            txtUnit.Text = "";
            _quantity = 0;
            txtQuantity.Text = "0";
            lblSumQuan.Text = "0";
            lblSumRate.Text = "₹0.00";
            txtTotalAmount.Text = "₹0.00";
        }
    }


    private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        CalculateTotal();
    }

    public void ClearData()
    {
        // Clear after save
        itemPicker.SelectedItem = null;
        txtQuantity.Text = string.Empty;
        txtRate.Text = string.Empty;
        txtTotalAmount.Text = string.Empty;
        txtUnit.Text = string.Empty;
    }

    private int _quantity = 0;

    private void BdrPlus_Tapped(object sender, TappedEventArgs e)
    {
        if (itemPicker.SelectedItem is Item selectedItem)
        {
            int step = selectedItem.Multiply > 0
                ? selectedItem.Multiply
                : 1;

            _quantity += step;

            txtQuantity.Text = _quantity.ToString();

            CalculateTotal();
        }
    }

    private void BdrMinus_Tapped(object sender, TappedEventArgs e)
    {
        if (itemPicker.SelectedItem is Item selectedItem)
        {
            int step = selectedItem.Multiply > 0
                ? selectedItem.Multiply
                : 1;
            if (_quantity <= 0)

                return;
            _quantity -= step;

            if (_quantity < 0)
                _quantity = 0;

            txtQuantity.Text = _quantity.ToString();

            CalculateTotal();
        }
    }

    private void CalculateTotal()
    {
        int qty = int.TryParse(
            txtQuantity.Text,
            out var quantity)
            ? quantity
            : 0;

        decimal rate = decimal.TryParse(
            txtRate.Text,
            out var price)
            ? price
            : 0;

        decimal total = qty * rate;

        lblSumQuan.Text =
            $"{qty} {txtUnit.Text}";

        lblSumRate.Text =
            $"₹{rate:0.00}";

        txtTotalAmount.Text =
            $"₹{total:0.00}";
    }

    private void txtRate_TextChanged(object sender, TextChangedEventArgs e)
    {
        CalculateTotal();
    }

    private void OnPickerTapped(object sender, TappedEventArgs e)
    {
        itemPicker.Focus();
    }
}