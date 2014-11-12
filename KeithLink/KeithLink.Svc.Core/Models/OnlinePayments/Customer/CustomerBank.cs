using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class CustomerBank {
        [Key, Column("Division", TypeName="char", Order = 1), MaxLength(5)]
        public string BranchId { get; set; }

        [Key, Column(TypeName="char", Order=2), MaxLength(6)]
        public string CustomerNumber { get; set; }

        [Key, Column(Order=3), MaxLength(17)]
        public string AccountNumber { get; set; }

        [MaxLength(12)]
        public string TransitNumber { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(55)]
        public string Address1 { get; set; }

        [MaxLength(55)]
        public string Address2 { get; set; }

        [MaxLength(30)]
        public string City { get; set; }

        [Column(TypeName="char"), MaxLength(2)]
        public string State { get; set; }

        [Column(TypeName = "char"), MaxLength(5)]
        public string Zip { get; set; }

        public bool DefaultAccount { get; set; }
    }
}
