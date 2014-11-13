using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class CustomerBank {
        public string BranchId { get; set; }

        public string CustomerNumber { get; set; }

        public string AccountNumber { get; set; }

        public string TransitNumber { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public bool DefaultAccount { get; set; }
    }
}
