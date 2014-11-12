using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Profile {
    public class UserGroup {
        [Key, Column(Order=1), MaxLength(30)]
        public string UserName { get; set; }

        [Key, Column("Division", TypeName="char", Order=2), MaxLength(5)]
        public string BranchId { get; set; }

        [Key, Column(TypeName="char", Order=3), MaxLength(6)]
        public string CustomerNumber { get; set; }

        public bool? InquireOnly { get; set; }
    }
}
