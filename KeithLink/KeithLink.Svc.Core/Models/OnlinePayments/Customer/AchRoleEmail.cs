using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Customer {
    public class AchRoleEmail {
        public string RoleName { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }
    }
}
