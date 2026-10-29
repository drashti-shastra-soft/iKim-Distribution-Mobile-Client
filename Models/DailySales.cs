using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class DailySales
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Cash { get; set; }
        public decimal? Online { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public int? DeletedBy { get; set; }

        //  Null safe Amount

        [JsonIgnore]
        public decimal Total => (Cash ?? 0) + (Online ?? 0);

        [JsonIgnore]
        public string TotalFormatted =>
            $"₹ {Total:N2}";

        [JsonIgnore]
        public string CashFormatted =>
            $"₹ {Cash:N2}";

        [JsonIgnore]
        public string OnlineFormatted =>
            $"₹ {Online:N2}";

        [JsonIgnore]
        public string SalesDay =>
            Date == default
                ? "Not set"
                : "(" + Date.ToString("dddd") + ")";
        //: Date.ToString("dddd");

        [JsonIgnore]
        public string SalesDate =>
            Date == default
                ? "Not set"
                : Date.ToString("dd MMM yyyy");

    }


    public class DailySalesLister
    {
        public List<DailySales> List { get; set; } = new List<DailySales>();

        public DailySales SearchCriteria { get; set; } = new DailySales();

        public Pagination Pagination { get; set; } = new Pagination();
    }

}
