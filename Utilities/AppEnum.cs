using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Utilities
{
    public static class AppEnum
    {
        public enum TabType
        {
            Home,
            Orders,
            Add,
            Delivery,
            Profile
        }
        public enum OrderStatus
        {
            Pending = 1,
            Fulfilled = 2,
            PartiallyFulfilled = 3,
        }
    }
}
