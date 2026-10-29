using i_KimMobileClient.Models;
using i_KimMobileClient.Pages.Accounts;
using i_KimMobileClient.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.APIService;

public class ApiResponseHandler
{
    public static async Task<Response> HandlerHttpResponse<T>(HttpResponseMessage responseMessage)
    {
        Response response = new Response();
        try
        {
            string jsonResult = responseMessage.Content != null ? await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

            if (responseMessage.IsSuccessStatusCode &&
                (responseMessage.StatusCode == System.Net.HttpStatusCode.OK ||
                responseMessage.StatusCode == System.Net.HttpStatusCode.NoContent))
            {
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                response.Value = JsonConvert.DeserializeObject<T>(jsonResult);
            }
            else if (responseMessage.IsSuccessStatusCode && responseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            //else if (responseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                response.ErrorMessage = JsonConvert.DeserializeObject<ApiMessage>(jsonResult)?.Message;
            }
            //else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            else if (!responseMessage.IsSuccessStatusCode && responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.IsSuccess = false;
                response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                //response.ErrorMessage = "Session expired.";
                response.ErrorMessage = JsonConvert.DeserializeObject<ApiMessage>(jsonResult)?.Message;
                await SessionManager.LogoutAsync();
            }
            else
            {
                response.IsSuccess = false;
                response.StatusCode = responseMessage.StatusCode;
                response.ErrorMessage = jsonResult;
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
        return response;
    }
}
