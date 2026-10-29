using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Utilities
{
    public class AppSetting
    {
        public static void SetRememberMe(bool isRemember)
        {
            Preferences.Default.Set("remember", isRemember);
        }
        public static bool GetRememberMe()
        {
            return Preferences.Default.Get("remember", false);
        }

        public static void SetEmail(string email)
        {
            Preferences.Default.Set("email", email);
        }
        public static string GetEmail()
        {
            return Preferences.Default.Get("email", string.Empty);
        }

        public static void SetPassword(string password)
        {
            Preferences.Default.Set("password", password);
        }
        public static string GetPassword()
        {
            return Preferences.Default.Get("password", string.Empty);
        }
    }
}
