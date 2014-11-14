using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment {
    public class PaymentTransaction {
        public string BranchId { get; set; }

        public string CustomerNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string AccountNumber { get; set; }

        public string UserName { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

        public long ConfirmationId { get; set; }
    }
}
