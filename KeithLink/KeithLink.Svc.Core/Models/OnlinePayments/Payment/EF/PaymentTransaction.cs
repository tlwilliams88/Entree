using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment.EF {
    public class PaymentTransaction {
        [Key, Column("Division", TypeName="char", Order=1), MaxLength(5)]
		public string Division { get; set; }

        [Key, Column(TypeName="char", Order = 2), MaxLength(6)]
        public string CustomerNumber { get; set; }

        [Key, Column(Order=3), MaxLength(30)]
        public string InvoiceNumber { get; set; }

        [Key, Column(Order=4), MaxLength(17)]
        public string AccountNumber { get; set; }

        [Key, Column(Order=5), MaxLength(30)]
        public string UserName { get; set; }

        [Key, Column(Order=6)]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal PaymentAmount { get; set; }

        [Required]
        public int ConfirmationId { get; set; }

        [Column(TypeName="date")]
        public DateTime ScheduledPaymentDate { get; set; }
    }
}
