using Bangazon.Models;
using Bangazon.Data;
using System.ComponentModel.DataAnnotations;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductDetailViewModel
  {
    public Product Product { get; set; }

    [Display(Name = "Inventory Remaining")]
    public int Inventory { get; set; }
  }
}