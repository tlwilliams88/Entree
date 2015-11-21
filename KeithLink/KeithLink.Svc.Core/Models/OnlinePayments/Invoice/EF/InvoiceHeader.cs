using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Invoice.EF {
    public class InvoiceHeader {
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

        [Key, Column(TypeName = "char", Order = 2), MaxLength(5)]
        public string Division { get; set; }

        [Key, Column(TypeName = "char", Order = 3), MaxLength(5)]
        public string CustomerNumber { get; set; }

        [Key, Column(TypeName = "char", Order = 5), MaxLength(3)]
        public string InvoiceType { get; set; }

        public string InvoiceStatus { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public decimal AmountDue { get; set; }

        public int TransactionCount { get; set; }
        #endregion
    }
}
