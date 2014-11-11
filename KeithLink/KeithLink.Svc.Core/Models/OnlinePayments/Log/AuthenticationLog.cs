using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Log {
    public class AuthenticationLog {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AuthenticationTime { get; set; }

        [MaxLength(128)]
        public string UserName { get; set; }

        [MaxLength(1000), Required]
        public string Message { get; set; }
    }
}
