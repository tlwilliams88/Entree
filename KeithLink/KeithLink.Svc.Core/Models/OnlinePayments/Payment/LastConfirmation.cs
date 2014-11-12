using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Payment {
    public class LastConfirmation {
        [Key, Required]
        public long ConfirmationId { get; set; }
    }
}
