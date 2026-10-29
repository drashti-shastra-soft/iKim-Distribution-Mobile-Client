

using i_KimMobileClient.Models;
using i_KimMobileClient.Utilities;

namespace i_KimMobileClient.APIService
{
    public class AuthenticationAPI
    {
        Response requestResponse;

        public AuthenticationAPI()
        {
            requestResponse = new Response();
        }

        public async Task<Response> AuthenticateAsync(Credential mAuthentication)
        {
            if (Common.CheckInternetConnection())
            {
                using (ApiServiceHandler apiServiceHandler = new ApiServiceHandler())
                {
                    requestResponse = await apiServiceHandler.PostAsync<Credential, User>(EndPointsUrl.Authentication, mAuthentication);

                    if (requestResponse == null && !requestResponse.IsSuccess)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", $"{requestResponse.ErrorMessage}", "Ok");
                    }
                }
            }
            else
            {
                //Common.ErrorMessage(AppConstraints.NoInternet_Connection);
            }
            return requestResponse;
        }
    }
}
