using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Services;

public static  class LoaderService
{
    private static HomeTabbedPage? _homePage;

    public static void Register(HomeTabbedPage page)
    {
        _homePage = page;
    }

    //public static void Show(string message = "Loading...")
    //{
    //    _homePage?.ShowPageLoader(message);
    //}

    //public static void Hide()
    //{
    //    _homePage?.HidePageLoader();
    //}
}
