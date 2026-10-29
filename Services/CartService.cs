using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i_KimMobileClient.Utilities;

namespace i_KimMobileClient.Services
{
    public static class CartService
    {
        private static int _count = 0;
        public static int Count
        {
            get => _count;
            private set
            {
                if (_count == value) return;
                _count = value;
                CountChanged?.Invoke(null, _count);
            }
        }

        // Any page can subscribe to this — fires whenever the count changes
        public static event EventHandler<int>? CountChanged;

        // Call this after login, app start, or when you explicitly need a fresh fetch
        public static async Task RefreshAsync()
        {
            try
            {
                string url = string.Format(EndPointsUrl.TotalCartItemCount, Common.mUser.StoreId);
                var mResponse = await new APIService.ApiServiceHandler().GetAsync<int>(url, Common.mUser.Token);

                if (mResponse != null && mResponse.IsSuccess)
                {
                    Count = (int)mResponse.Value; // triggers CountChanged if different
                }
            }
            catch (Exception ex)
            {
                Common.ErrorMessage($"CartService.RefreshAsync: {ex.Message}");
            }
        }

        // Call this locally right after add/delete, for an instant UI update
        // without waiting on a round-trip — then optionally reconcile with RefreshAsync
        public static void SetLocal(int newCount)
        {
            Count = newCount;
        }

        public static void Increment(int by = 1) => Count = Count + by;
        public static void Decrement(int by = 1) => Count = Math.Max(0, Count - by);

        public static void Reset() => Count = 0; // e.g. on logout
    }
}
