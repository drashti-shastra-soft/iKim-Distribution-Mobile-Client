using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace i_KimMobileClient.Models
{
    public class Item : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public int UnitOfMeasureId { get; set; }
        public int PricingTierId { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string UnitOfMeasure { get; set; }
        public int HSNCode { get; set; }
        public double GST { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double? Stock { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
        public object Status { get; set; }
        public List<ItemPrice> Prices { get; set; }
        public int CreatedBy { get; set; }
        public int OrderLineId { get; set; }
        public object LastModifiedBy { get; set; }
        public int Multiply { get; set; }
        public bool IsRawMaterial { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string ImagePath { get; set; }
        public string Base64String { get; set; }


        //extra
        [JsonIgnore]
        public int PartyOrderLineId { get; set; }


        [JsonIgnore]
        public ImageSource? ImageSourceCached { get; set; }

        private bool _deleteIconVisibility = false; 
        public bool DeleteIconVisibility
        {
            get => _deleteIconVisibility;
            set
            {
                if (_deleteIconVisibility != value)
                {
                    _deleteIconVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        //extra
        private int _numberOfQuntity;
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
        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set
            {
                _isEditable = value;
                OnPropertyChanged(nameof(IsEditable));
            }
        }

        // Opposite for binding disable state
        public bool IsReadOnly => !_isEditable;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
