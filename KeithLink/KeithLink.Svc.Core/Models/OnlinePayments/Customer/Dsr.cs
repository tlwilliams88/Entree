using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class Dsr {
        public string DsrNumber { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }
    }
}
