using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace i_KimMobileClient.Models
{
    public class PartyOrderLine
    {
        public int Id { get; set; }
        public int PartyOrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Item { get; set; }

        // Total= (Quantity / Rate) + WaiterCharge
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public string FileName { get; set; }
        public string Base64String { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasure { get; set; }
        public int? WaiterCharge { get; set; }

        [JsonIgnore]
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ItemName)) return "?";
                return string.Join("",
                    ItemName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Take(2)
                        .Where(w => !string.IsNullOrEmpty(w))
                        .Select(w => w[0].ToString().ToUpper()));
            }
        }

        [JsonIgnore]
        public ImageSource? ImageSourceCached { get; set; }

        //  Null safe Amount
        [JsonIgnore]
        public string TotalAmountFormatted =>
            $"₹{Total:N2}";

        [JsonIgnore]
        public string QuantityWithUnit =>
            $"{Quantity} {UnitOfMeasure}";

        [JsonIgnore]
        public string RateWithUnit =>
            $"₹{Rate:N2}";
        //public string RateWithUnit =>
        //    $"₹ {Rate:N2}/{UnitOfMeasure}";
    }

    public class PartyOrderLineLister
    {
        public List<PartyOrderLine> List { get; set; } = new List<PartyOrderLine>();

        public PartyOrder SearchCriteria { get; set; } = new PartyOrder();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
