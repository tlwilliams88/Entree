using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Invoice {
    public class Invoice {
        public string InvoiceNumber { get; set; }

        public string BranchId { get; set; }

        public string CustomerNumber { get; set; }

        public Int16 ItemSequence { get; set; }

        public string InvoiceType { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public decimal AmountDue { get; set; }

        public bool DeleteFlag { get; set; }
    }
}
