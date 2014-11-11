using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments {
    [Table("Division")]
    public class Branch {
        [Key, Column("Division", TypeName = "char"), MaxLength(5)]
        public string BranchId { get; set; }

        [MaxLength(20), Required]
        public string Description { get; set; }

        [Column(TypeName="char"), MaxLength(3), Required]
        public string Abbreviation { get; set; }
    }
}
