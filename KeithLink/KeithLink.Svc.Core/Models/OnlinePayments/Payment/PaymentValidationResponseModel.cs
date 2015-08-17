// KeithLink

// Core
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment {
    [DataContract(Name = "validation")]
    public class PaymentValidationResponseModel {

        [DataMember(Name = "isvalid")]
        public bool IsValid { get; set; }

        [DataMember( Name = "transactions" )]
        public List<Core.Models.OnlinePayments.Payment.PaymentTransactionModel> PaymentTransactions { get; set; }

    }
}
