using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bangazon.Models
{
  public class PaymentType
  {
    [Key]
    public int PaymentTypeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Display(Name = "Date created")]
    public DateTime DateCreated { get; set; }

    [Required]
    [StringLength(55)]
    public string Description { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Account Number")]
    public string AccountNumber { get; set; }

    [Required]
    public string UserId {get; set;}

    [Required]
    public ApplicationUser User { get; set; }

    public ICollection<Order> Orders { get; set; }

        //added to concat description and account number in multiselect

        [Display(Name = "Payment Information")]

        public string PaymentDetails => string.Format("{0} {1}", Description, AccountNumber);
    }
}
