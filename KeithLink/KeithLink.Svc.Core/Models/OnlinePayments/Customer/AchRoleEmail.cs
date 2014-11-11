using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class AchRoleEmail {
        [Key, MaxLength(30)]
        public string RoleName { get; set; }

        [Key, MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(70), Required]
        public string EmailAddress { get; set; }
    }
}
