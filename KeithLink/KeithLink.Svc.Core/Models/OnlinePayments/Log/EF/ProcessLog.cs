﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Log.EF {
    public class ProcessLog {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        [MaxLength(9), Required]
        public string Type { get; set; }

        [Required]
        public DateTime ProcessTime { get; set; }
    }
}
