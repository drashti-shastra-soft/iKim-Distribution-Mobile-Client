namespace i_KimMobileClient.Pages;

public partial class AddItemPage : ContentPage
{
    public AddItemPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        clsItems.ItemsSource = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }

    private void BdrBack_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PopAsync();
    }

    private void ImgSearch_Tapped(object sender, TappedEventArgs e)
    {

    }
}