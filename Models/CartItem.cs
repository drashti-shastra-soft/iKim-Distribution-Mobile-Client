using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class CartItem: INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ItemId { get; set; }
        private int _unitOfMeasureId;
        public int UnitOfMeasureId
        {
            get => _unitOfMeasureId;
            set
            {
                _unitOfMeasureId = value;
                OnPropertyChanged();
            }
        }

        public string UnitOfMeasure { get; set; }
        public string Item { get; set; }
        public decimal Rate { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Base64String { get; set; }
        public int CreatedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public int Multiply { get; set; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
                Amount = Rate * _quantity;
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
