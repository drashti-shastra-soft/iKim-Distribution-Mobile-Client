using i_KimMobileClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Utilities
{
    public static class EndPointsUrl
    {
        #region [ Account ]
        public const string Authentication = "api/Authentication";
        public const string ForgotPassword = "api/ForgotPassword";
        public const string ChangePassword = "api/ChangePassword";
        #endregion

        #region [ Orders ]
        public const string GetAllOrder = "api/Order/GetAll";
        public const string GetOrders = "api/Order";
        public const string GetAllActiveItemsGetOrders = "/api/Item/PricingTier/{0}"; //PricingTierId
        public const string CreateOrder = "api/Order/PlaceOrder";
        public const string GetOrderLine = "api/OrderLine/{0}"; // orderId
        public const string DeleteOrder = "api/OrderLine/{0}/LoginUser/{1}"; // orderId 
        #endregion


        #region [ Party Orders ]
        public const string GetPartyOrders = "api/PartyOrder/GetAll";
        public const string GetPartyOrderLineById = "api/PartyOrderLine/{0}"; //Pary order Line Id
        public const string SavePartyOrder = "api/PartyOrder/"; //Pary order Line Id
        public const string GetAllPartyOrder = "api/PartyOrderLine/GetAll"; //Pary order Line Id
        public const string DeletePartyOrder = "api/PartyOrder/{0}/LoginUser/{1}"; // {0}: PartyOrderId {1}: Login UserId
        public const string DeletePartyOrderLine = "api/PartyOrderLine/{0}/LoginUser/{1}"; // {0}: PartyOrderLineId {1}: Login UserId
        public const string AddPartyOrderItem = "api/PartyOrderLine";
        #endregion

        public const string SaveAddress = "api/Address";
        public const string GetAddress = "api/Address/Get/Store/Id/{0}";  //storeID 

        #region Daily Sales
        public const string AddDailySales = "api/DailySales";
        public const string GetDailySales = "api/DailySales/GetAll";
        public const string CheckPendingSales = "api/DailySales/CheckPendingSales/{0}"; //storeID

        #endregion

        #region [ Dashboard Banner ]
        public const string GetAllActiveBanner = "api/Banner";   
        public const string GetNotification = "api/Notification/UserId/{0}";   
        public const string MarkNotificationAsRead = "api/Notification/UpdateIsRead/{0}/LoginUserId/{1}";    // {0}: NotificationId {1}: Login UserId
        public const string SealsChart = "api/Chart/SalesChart/{0}";  //storeID 
        #endregion

        #region [ Categories ]
        public const string GetAllActiveCategory = "api/Category";   
        public const string GetSubCategoryById = "api/Item/CategoryId/{0}/PricingTier/{1}";   
        #endregion
        public const string AddCartItem = "api/CartItem";   
        public const string GetAllCart = "api/CartItem/StoreId/{0}";   
        public const string DeleteCartItem = "api/CartItem/{0}";   
        public const string TotalCartItemCount = "api/CartItem/Count/StoreId/{0}";   
    }
}
