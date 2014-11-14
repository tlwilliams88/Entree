using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF {
    public class Dsr {
        [Key, Column(TypeName="char"), MaxLength(8)]
        public string DsrNumber { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string EmailAddress { get; set; }
    }
}
