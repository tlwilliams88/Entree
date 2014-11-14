using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Profile {
    public class UserGroup {
        public string UserName { get; set; }

        public string BranchId { get; set; }

        public string CustomerNumber { get; set; }

        public bool? InquireOnly { get; set; }
    }
}
