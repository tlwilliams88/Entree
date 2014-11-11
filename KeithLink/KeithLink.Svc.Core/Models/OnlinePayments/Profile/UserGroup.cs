using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Profile {
    public class UserGroup {
        [Key, MaxLength(30)]
        public string UserName { get; set; }

        [Key, Column("Division", TypeName="char"), MaxLength(5)]
        public string BranchId { get; set; }

        [Key, Column(TypeName="char"), MaxLength(6)]
        public string CustoemrNumber { get; set; }

        public bool? InquireOnly { get; set; }
    }
}
