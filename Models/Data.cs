using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class Data
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthString { get; set; }
        public decimal Sales { get; set; }
        public decimal Purchase { get; set; }
        public decimal Item { get; set; }
        public int ItemId { get; set; }
        public int StoreId { get; set; }
        public int VendorId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
    }
}
