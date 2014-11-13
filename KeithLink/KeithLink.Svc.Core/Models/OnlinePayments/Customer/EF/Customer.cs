using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer.EF {
    public class Customer {
        [Key, Column("Division", TypeName = "char", Order = 1), MaxLength(5)]
        public string BranchId { get; set; }

        //[ForeignKey("Division")]
        //public Branch Branch { get; set; }

        [Key, Column(TypeName = "char", Order = 2), MaxLength(6)]
        public string CustomerNumber { get; set; }

        [MaxLength(50), Required]
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

        [MaxLength(30)]
        public string CreditEmailRole { get; set; }

        [Required, MaxLength(8)]
        public string DsrNumber { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
