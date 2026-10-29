using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;


namespace i_KimMobileClient.Models
{
    public class PartyOrder : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string Store { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string DeliveryLocation { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public int? NumOfCups { get; set; }
        public int? NumOfSpoons { get; set; }
        public int? NumOfGlass { get; set; }
        public decimal TotalAmount { get; set; }
        public string WithWaiter { get; set; }
        public string Narration { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public int? WaiterCharge { get; set; }
        //public List<PartyOrderLine> Lines { get; set; } = new List<PartyOrderLine>();

        //Extra

        //  Null safe Initials
        public bool ShowDeleteButton { get; set; }
        public int PartyOrderLineId { get; set; }

        [JsonIgnore]
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name)) return "?";
                return string.Join("",
                    Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Take(2)
                        .Where(w => !string.IsNullOrEmpty(w))
                        .Select(w => w[0].ToString().ToUpper()));
            }
        }

        // Null safe DateTime format
        [JsonIgnore]
        public string DeliveryDateTimeFormatted =>
            DeliveryDateTime == default
                ? "Not set"
                : DeliveryDateTime.ToString("dd MMM yyyy  hh:mm tt");

        //  Null safe Amount
        [JsonIgnore]
        public string TotalAmountFormatted =>
            $"₹{TotalAmount:N2}";

        //  Null safe Waiter
        [JsonIgnore]
        public Color WaiterBgColor => WithWaiter == "Yes"
            ? Color.FromArgb("#E8F5E9")
            : Color.FromArgb("#FCEBEB");
        
        [JsonIgnore]
        public Color WaiterTextColor => WithWaiter == "Yes"
            ? Color.FromArgb("#27500A")
            : Color.FromArgb("#791F1F");

        [JsonIgnore]
        public string WaiterLabel => WithWaiter == "Yes"
            ? "With Waiter" : "Without Waiter";

        // Null safe display values
        [JsonIgnore]
        public string StoreName => Store ?? "-";
        [JsonIgnore] 
        public string CustomerName => Name ?? "-";
        [JsonIgnore] 
        public string CustomerMobile => MobileNumber ?? "-";
        [JsonIgnore] 
        public string LocationDisplay => DeliveryLocation ?? "-";
        [JsonIgnore] 
        public string CupsDisplay => NumOfCups?.ToString() ?? "0";
        [JsonIgnore] 
        public string SpoonsDisplay => NumOfSpoons?.ToString() ?? "0";
        [JsonIgnore] 
        public string GlassDisplay => NumOfGlass?.ToString() ?? "0";
    
        private int _numberOfQuntity;
        [JsonIgnore]
        public int NumberOfQuntity
        {
            get => _numberOfQuntity;
            set
            {
                if (_numberOfQuntity != value)
                {
                    _numberOfQuntity = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _quntityUnit;
        [JsonIgnore]
        public int QuntityUnit
        {
            get => _quntityUnit;
            set
            {
                if (_quntityUnit != value)
                {
                    _quntityUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PartyOrderLister
    {
        public List<PartyOrder> List { get; set; } = new List<PartyOrder>();
        //public List<PartyOrderLine> List { get; set; } = new List<PartyOrderLine>();

        public PartyOrder SearchCriteria { get; set; } = new PartyOrder();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
