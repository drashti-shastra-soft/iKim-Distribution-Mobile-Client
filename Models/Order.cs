using Newtonsoft.Json;
using static i_KimMobileClient.Utilities.AppEnum;

namespace i_KimMobileClient.Models
{
    public class Order
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int StoreId { get; set; }
        public string Store { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public int ItemId { get; set; }
        public int QuantityRequested { get; set; }
        public string Status { get; set; } = "";
        public string TotalItemsText { get; set; } = "";
        public string ItemsSummary { get; set; } = "";
        public string TotalAmount { get; set; } = "";
        public string QuantitySummary { get; set; } = "";
        public decimal Amount { get; set; }

        public List<OrderLine> Lines { get; set; } = new List<OrderLine>();

        [JsonIgnore]
        public string StatusLabel => Status switch
        {
            "Pending" => "Pending",
            "Fulfilled" => "Fulfilled",
            "PartiallyFulfilled" => "Partial",
            _ => Status ?? "Unknown"
        };

        [JsonIgnore]
        public int StatusId => Status switch
        {
            "Pending" => (int)OrderStatus.Pending,
            "Fulfilled" => (int)OrderStatus.Fulfilled,
            "PartiallyFulfilled" => (int)OrderStatus.PartiallyFulfilled,
            _ => 0
        };

        //  Pending = orange bg, Delivered = green bg
        [JsonIgnore]
        public Color StatusBgColor => Status switch
        {
            "Pending" => Color.FromArgb("#FFF3E0"), // Orange light
            "Fulfilled" => Color.FromArgb("#E8F5E9"), // Green light
            "Partially Fulfilled" => Color.FromArgb("#E3F2FD"), // Blue light
            _ => Color.FromArgb("#F5F5F5")  // Gray light
        };

        [JsonIgnore]
        public Color StatusTextColor => Status switch
        {
            "Pending" => Color.FromArgb("#FF6B00"), // Orange
            "Fulfilled" => Color.FromArgb("#2E7D32"), // Green
            "Partially Fulfilled" => Color.FromArgb("#1565C0"), // Blue
            _ => Color.FromArgb("#9E9E9E")  // Gray
        };
    }

    public class OrderLister
    {
        public List<Order> List { get; set; } = new List<Order>();

        public Order SearchCriteria { get; set; } = new Order();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
