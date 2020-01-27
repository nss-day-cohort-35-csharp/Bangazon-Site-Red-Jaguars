using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProducTypesViewModels
{
    public class TypeWithProducts
    {
        public int TypeId { get; set; }
        [Display(Name = "Categories")]
        public string TypeName { get; set; }

        
        public int ProductCount { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
