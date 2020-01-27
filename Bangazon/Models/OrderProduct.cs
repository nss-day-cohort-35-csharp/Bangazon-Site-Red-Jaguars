using System.ComponentModel.DataAnnotations;

namespace Bangazon.Models {
    public class OrderProduct {

        //primary key
        [Key]
        public int OrderProductId { get; set; }
        //with required orderId, EF will cascade delete
        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

    }
}