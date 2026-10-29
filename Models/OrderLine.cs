using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;

namespace i_KimMobileClient.Models
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public int QuantityRequested { get; set; }
        public int QuantityFulfilled { get; set; }
        public string QuantityWithUnit { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public decimal TotalPrice { get; set; }
        public int AlreadyFulfilledQty { get; set; }
        public int AuditId { get; set; }
        public int UnitOfMeasureId { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int CartItemId { get; set; }
    }
    public class OrderLineLister
    {
        public List<OrderLine> List { get; set; } = new List<OrderLine>();

        public OrderLine SearchCriteria { get; set; } = new OrderLine();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
