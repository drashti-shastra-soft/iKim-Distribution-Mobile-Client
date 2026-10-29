using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class ItemPrice
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int PricingTierId { get; set; }
        public double Price { get; set; }
        public int AuditId { get; set; }
    }
}
