using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductViewModels
{
    public class ProductCreateViewModel
    {
        public IFormFile File { get; set; }
        [Key]
        public int ProductId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [StringLength(55, ErrorMessage = "Please shorten the product title to 55 characters")]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string UserId { get; set; }

        public string City { get; set; }

        [Display(Name = "Local Delivery")]
        public bool LocalDelivery { get; set; }

        public string ImagePath { get; set; }



        public bool Active { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Error: Must choose a product category")]
        [Display(Name = "Product Category")]
        public int ProductTypeId { get; set; }

        [Display(Name = "Product Type")]
        public ProductType ProductType { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
