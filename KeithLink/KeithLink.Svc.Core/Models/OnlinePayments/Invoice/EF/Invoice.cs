using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF {
    public class Invoice {
        #region properties
        [Key, Column(TypeName = "varchar", Order = 1), MaxLength(30)]
        public string InvoiceNumber { get; set; }

        public string TrimmedInvoiceNumber {
            get {
                if (InvoiceNumber == null) {
                    return null;
                } else {
                    return InvoiceNumber.Trim();
                }
            }
        }

        [Key, Column(TypeName="char", Order=2), MaxLength(5)]
        public string Division { get; set; }

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
        #endregion
    }
}
