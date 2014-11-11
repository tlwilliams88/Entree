using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Invoice {
    public class Invoice {
        [Key, Column(TypeName="char"), MaxLength(30)]
        public string InvoiceNumber { get; set; }

        [Key, Column("Division", TypeName="char"), MaxLength(5)]
        public string BranchId { get; set; }

        [Key, Column(TypeName="char"), MaxLength(5)]
        public string CustomerNumber { get; set; }

        [Key]
        public Int16 ItemSequence { get; set; }

        [Key, Column(TypeName="char"), MaxLength(3)]
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
