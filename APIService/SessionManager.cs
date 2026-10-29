using i_KimMobileClient.Pages.Accounts;
using i_KimMobileClient.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.APIService;

public static class SessionManager
{
    private static bool _isLoggingOut = false;

    public static async Task LogoutAsync()
    {
        if (_isLoggingOut)
            return;

        _isLoggingOut = true;

        try
        {
            // Clear authentication token
            Common.AuthToken = string.Empty;

            // If you store token in Preferences, clear it
            Preferences.Remove("AuthToken");


            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Application.Current.MainPage =
                    new NavigationPage(new LoginPage());

                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
        finally
        {
            _isLoggingOut = false;
        }
    }
}