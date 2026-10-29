using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Pages;
using Mopups.Services;

namespace i_KimMobileClient.PopupPages;

public partial class AddressPopupPage : PopupPage
{
    public event EventHandler AddressEvent;
    Models.Address _address;
    public AddressPopupPage(Models.Address address = null)
    {
        InitializeComponent();
        this._address = address;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_address == null)
        {
            LblTitle.Text = "Add Address";
        }
        else
        {
            LblTitle.Text = "Edit Address";
            txtLineOne.Text = _address.Line1;
            txtLineTwo.Text = _address.Line2;
            txtLineThree.Text = _address.Line3;
            txtCity.Text = _address.City;
            txtState.Text = _address.State;
            txtZipcode.Text = _address.ZipCode;
        }

    }

    private bool ValidateControl()
    {
        if (String.IsNullOrWhiteSpace(txtLineOne.Text) &&
            String.IsNullOrWhiteSpace(txtCity.Text) &&
            String.IsNullOrWhiteSpace(txtState.Text) &&
            String.IsNullOrWhiteSpace(txtZipcode.Text))
        {
            LblLine1.IsVisible = true;
            LblCity.IsVisible = true;
            LblState.IsVisible = true;
            LblZipCode.IsVisible = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtLineOne.Text))
        {
            LblLine1.IsVisible = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtCity.Text))
        {
            LblCity.IsVisible = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtState.Text))
        {
            LblState.IsVisible = true;
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtZipcode.Text))
        {

            LblZipCode.IsVisible = true;
            return false;
        }
        return true;
    }

    //private async Task SaveAddress()
    //{
    //    if (!ValidateControl())
    //        return;

    //    await PopLoaderService.ShowAsync();
    //    var mAddress = new Models.Address()
    //    {
    //        StoreId = Common.mUser.Id,
    //        Line1 = txtLineOne.Text,
    //        Line2 = txtLineTwo.Text,
    //        Line3 = txtLineThree.Text,
    //        City = txtCity.Text,
    //        State = txtState.Text,
    //        ZipCode = txtZipcode.Text,
    //    };
    //    var mResponse = await new APIService.ApiServiceHandler().PostAsync<Models.Address>(EndPointsUrl.SaveAddress, mAddress, Common.AuthToken);
    //    if (mResponse != null && mResponse.IsSuccess)
    //    {
    //        AddressEvent?.Invoke(this, EventArgs.Empty);
    //        Common.SuccessMessage("Address saved successfully!");
    //    }
    //    await PopLoaderService.HideAsync();
    //}

    private async Task SaveAddress()
    {
        if (!ValidateControl())
            return;

        await PopLoaderService.ShowAsync();

        try
        {
            var mAddress = new Models.Address()
            {
                StoreId = Common.mUser.Id,
                Line1 = txtLineOne.Text,
                Line2 = txtLineTwo.Text,
                Line3 = txtLineThree.Text,
                City = txtCity.Text,
                State = txtState.Text,
                ZipCode = txtZipcode.Text,
            };

            var mResponse = await new APIService.ApiServiceHandler()
                .PostAsync<Models.Address>(
                    EndPointsUrl.SaveAddress,
                    mAddress,
                    Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Address saved successfully!");

                // First close loader
                await PopLoaderService.HideAsync();

                // Then close address popup
                await MopupService.Instance.PopAsync();
            }
            else
            {
                await PopLoaderService.HideAsync();
            }
        }
        catch (Exception ex)
        {
            await PopLoaderService.HideAsync();
            Console.WriteLine(ex.Message);
        }
    }

    private async void BtnClose_Cliked(object sender, TappedEventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }

    private async void BtnSubmit_Clicked(object sender, EventArgs e)
    {
        await Common.ControlBounceEffect(BtnSubmit);
        await SaveAddress();
    }

    private void txtLineOne_TextChanged(object sender, TextChangedEventArgs e)
    {
        LblLine1.IsVisible = false;
    }

    private void txtCity_TextChanged(object sender, TextChangedEventArgs e)
    {
        LblCity.IsVisible = false;
    }

    private void txtState_TextChanged(object sender, TextChangedEventArgs e)
    {
        LblState.IsVisible = false;
    }

    private void txtZipcode_TextChanged(object sender, TextChangedEventArgs e)
    {
        LblZipCode.IsVisible = false;
    }
}
