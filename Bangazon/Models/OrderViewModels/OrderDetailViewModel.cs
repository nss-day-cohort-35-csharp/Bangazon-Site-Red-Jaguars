using System.Collections.Generic;

namespace Bangazon.Models.OrderViewModels
{
    public class OrderDetailViewModel

    {
        public OrderDetailViewModel(Order order)
        {
            Order = order;
            LineItems = new List<OrderLineItem>();
        }
        public Order Order { get; set; }

        public OrderLineItem Fun { get; set; }

        public IEnumerable<OrderLineItem> LineItems { get; set; }

        
    }
}