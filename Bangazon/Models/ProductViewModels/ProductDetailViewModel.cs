using Bangazon.Models;
using Bangazon.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductDetailViewModel
  {
    public Product Product { get; set; }

    [Display(Name = "Inventory Remaining")]
    public int Inventory { get; set; }

    public string ImagePath { get; set; }
        public IFormFile File { get; set; }

    }
}