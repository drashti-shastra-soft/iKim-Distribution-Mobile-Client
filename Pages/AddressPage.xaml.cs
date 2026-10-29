using i_KimMobileClient.Services;
using i_KimMobileClient.Utilities;
using Mopups.Services;
using System.Diagnostics;
using System.Text.Json;

namespace i_KimMobileClient.Pages;

public partial class AddressPage : ContentPage
{
    Models.Address _address;
    public AddressPage(Models.Address address = null)
	{
		InitializeComponent();
        this._address = address;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_address == null || _address.Id == 0)
        //if (_address == null)
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

    private async Task SaveAddress()
    {
        if (!ValidateControl())
            return;

        await PopLoaderService.ShowAsync();

        try
        {
            var mAddress = new Models.Address()
            {
                StoreId = Common.mUser.StoreId.Value,
                Line1 = txtLineOne.Text,
                Line2 = txtLineTwo.Text,
                Line3 = txtLineThree.Text,
                City = txtCity.Text,
                State = txtState.Text,
                ZipCode = txtZipcode.Text,
                CreatedBy = Common.mUser.Id,
                LastModifiedBy = Common.mUser.Id,
            };

            var mResponse = await new APIService.ApiServiceHandler()
                .PostAsync<Models.Address>(
                    EndPointsUrl.SaveAddress,
                    mAddress,
                    Common.AuthToken);

            //Debug.WriteLine(JsonSerializer.Serialize(mAddress, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //}));

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Address saved successfully!");

                // First close loader
                await PopLoaderService.HideAsync();

                // Then close address popup
                await Navigation.PopAsync();
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
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

    private async void BackButton_Clicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopAsync();
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