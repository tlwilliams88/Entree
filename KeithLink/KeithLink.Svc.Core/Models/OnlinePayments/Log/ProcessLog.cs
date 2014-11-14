using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Log {
    public class ProcessLog {
        public long LogId { get; set; }

        public string Type { get; set; }

        public DateTime ProcessTime { get; set; }
    }
}
