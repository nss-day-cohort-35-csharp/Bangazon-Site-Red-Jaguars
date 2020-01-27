namespace Bangazon.Models.OrderViewModels {
    public class OrderLineItem {
        //public OrderLineItem(OrderProduct orderProduct)
        //{
        //    Product = orderProduct.Product;
        //    Units = 1;
        //    Cost = orderProduct.Product.Price;
        //}
        public Product Product { get; set; }
        public int Units { get; set; }
        public double Cost { get; set; }
    }
}