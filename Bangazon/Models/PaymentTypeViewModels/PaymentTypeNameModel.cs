using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.PaymentTypeViewModels
{
    public class PaymentTypeNameModel
    {
        public PaymentType PaymentType { get; set; }

        public string PaymentDetails => string.Format("{0} {1}", PaymentType.Description, PaymentType.AccountNumber);
    }
}
