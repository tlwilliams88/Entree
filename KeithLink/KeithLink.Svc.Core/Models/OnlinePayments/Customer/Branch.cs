using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    [Table("Division")]
    public class Branch {
        public string BranchId { get; set; }

        public string Description { get; set; }

        public string Abbreviation { get; set; }
    }
}
