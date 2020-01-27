using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangazon.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public bool LocalDeliveryAvailable { get; set; }

        [Required(ErrorMessage = "Date Created is required")]
        [Display(Name = "Date created")]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "Product description is required")]
        [Display(Name = "Description")]
        [StringLength(255, ErrorMessage = "Please shorten the product description to 255 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product title is required")]
        [Display(Name = "Title")]
        [StringLength(55, ErrorMessage = "Please shorten the product title to 55 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(18, 10000, ErrorMessage = "Price should have positive value")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Available product quantity is required")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "User Id is required")]
        [Display(Name = "User Id")]
        public string UserId { get; set; }

        //[Required(ErrorMessage = "City name is required")]
        //[Display(Name = "City")]
        //[StringLength(99, ErrorMessage = "City name has to have less than 99 characters")]
        public string City { get; set; }

        public string ImagePath { get; set; }

        public bool Active { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Product Category")]
        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public Product()
        {
            Active = true;
        }
    }
}
