using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class Customer {
        public string BranchId { get; set; }

        public string CustomerNumber { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string CreditEmailRole { get; set; }

        public string DsrNumber { get; set; }

        public bool Active { get; set; }
    }
}
