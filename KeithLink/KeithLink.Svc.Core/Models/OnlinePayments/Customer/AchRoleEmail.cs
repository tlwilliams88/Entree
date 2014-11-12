using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class AchRoleEmail {
        [Key, Column(Order=1), MaxLength(30)]
        public string RoleName { get; set; }

        [Key, Column(Order=2), MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(70), Required]
        public string EmailAddress { get; set; }
    }
}
