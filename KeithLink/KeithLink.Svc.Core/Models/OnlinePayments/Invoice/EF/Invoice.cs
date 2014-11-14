using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF {
    public class Invoice {
        [Key, Column(TypeName="char", Order=1), MaxLength(30)]
        public string InvoiceNumber { get; set; }

        [Key, Column("Division", TypeName="char", Order=2), MaxLength(5)]
        public string BranchId { get; set; }

        [Key, Column(TypeName="char", Order=3), MaxLength(5)]
        public string CustomerNumber { get; set; }

        [Key, Column(Order=4)]
        public Int16 ItemSequence { get; set; }

        [Key, Column(TypeName="char", Order=5), MaxLength(3)]
        public string InvoiceType { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal AmountDue { get; set; }

        [Required]
        public bool DeleteFlag { get; set; }
    }
}
