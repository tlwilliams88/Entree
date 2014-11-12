using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Log.EF{
    public class ApplicationLog {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LogTime { get; set; }

        [MaxLength(128), Required]
        public string UserName { get; set; }

        [MaxLength(1000)]
        public string Message { get; set; }
    }
}
