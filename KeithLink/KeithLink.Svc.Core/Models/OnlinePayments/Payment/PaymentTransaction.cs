using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment {
    public class PaymentTransaction {
        [Key, Column("Division", TypeName="char"), MaxLength(5)]
        public string BranchId { get; set; }

        [Key, Column(TypeName="char"), MaxLength(6)]
        public string CustomerNumber { get; set; }

        [Key, MaxLength(30)]
        public string InvoiceNumber { get; set; }

        [Key, MaxLength(17)]
        public string AccountNumber { get; set; }

        [Key, MaxLength(30)]
        public string UserName { get; set; }

        [Key]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal PaymentAmount { get; set; }

        [Required]
        public long ConfirmationId { get; set; }
    }
}
