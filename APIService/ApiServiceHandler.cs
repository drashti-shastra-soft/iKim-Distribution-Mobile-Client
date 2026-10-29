using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using i_KimMobileClient.Models;
using Newtonsoft.Json;

namespace i_KimMobileClient.APIService
{
    public class ApiServiceHandler : IDisposable
    {
        HttpClient httpClient;
        public HttpClient CreateHttpClient1(string authToken)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    //if (message.RequestUri.Host == "ikimwebservice.shastrasoft.net")
                    if (message.RequestUri.Host == "dev-ikim-webservice.shastrasoft.net")
                        return true;

                    return errors == System.Net.Security.SslPolicyErrors.None;
                };

                httpClient = new HttpClient(handler);
                //#endif

                httpClient.Timeout = TimeSpan.FromMinutes(15);
                httpClient.BaseAddress = new Uri(App.baseUrl);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //authToken.IsNormalized() 
                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }

            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return httpClient;
        }

        public HttpClient CreateHttpClient(string authToken)
        {
            try
            {
                var handler = new HttpClientHandler();

                //  Fixed host comparison
                handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) =>
                    {
                        //if (message.RequestUri.Host == "ikimwebservice.shastrasoft.net")
                        if (message.RequestUri.Host == "dev-ikim-web-service.shastrasoft.net")
                            return true;
                        return errors == System.Net.Security.SslPolicyErrors.None;
                    };

                httpClient = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromMinutes(15),
                    BaseAddress = new Uri(App.baseUrl)
                };

                httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authToken);
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return httpClient;
        }

        public async Task<Response> GetAsync<T>(string uri, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);
                var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> PostAsync<T, TR>(string uri, T data, string authToken = "")
        {
            try
            {
                //string endPoint = $"{App.baseUrl}{uri}";
                HttpClient httpClient = CreateHttpClient(authToken);

                var fullUrl = new Uri(httpClient.BaseAddress, uri);
                System.Diagnostics.Debug.WriteLine($"API CALL: {fullUrl}");

                #region [ Ignore Null value in json ]
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                #endregion

                var jsonString = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PostAsync(uri, content).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<TR>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> PostAsync<T>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);

                #region [ Ignore Null value in json ]
                //JsonSerializerSettings settings = new JsonSerializerSettings();
                //settings.NullValueHandling = NullValueHandling.Ignore;
                #endregion

                var jsonString = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PostAsync(uri, content).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> PostAsync(string uri, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);

                var content = new StringContent("");
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/json");

                var response = await httpClient.PostAsync(uri, content);

                return await ApiResponseHandler.HandlerHttpResponse<List<CartItem>>(response);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Response> PatchAsync<T>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);

                var jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);

                var content = new StringContent(
                    jsonString,
                    Encoding.UTF8,
                    "application/json");

                var request = new HttpRequestMessage(
                    new HttpMethod("PATCH"),
                    uri)
                {
                    Content = content
                };

                var responseMessage = await httpClient.SendAsync(request)
                    .ConfigureAwait(false);

                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }
        public async Task<Response> PutAsync<T>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);

                #region [ Ignore Null value in json ]
                //JsonSerializerSettings settings = new JsonSerializerSettings();
                //settings.NullValueHandling = NullValueHandling.Ignore; 
                #endregion

                var jsonString = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PutAsync(uri, content).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> DeleteAsync<T>(string uri, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);
                var responseMessage = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<string> GetBase64(string url)
        {
            string base64String = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await response.Content.ReadAsStreamAsync();
                            var bytes = memoryStream.ToArray();
                            base64String = Convert.ToBase64String(bytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return base64String;
        }

        public void Dispose()
        {
            //httpClient = null;
            //GC.SuppressFinalize(this);
        }
    }
}
