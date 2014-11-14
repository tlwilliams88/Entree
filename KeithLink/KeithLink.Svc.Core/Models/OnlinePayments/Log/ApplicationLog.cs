using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeithLink.Svc.Core.Models.OnlinePayments.Log {
    public class ApplicationLog {
        public long LogId { get; set; }

        public DateTime LogTime { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }
    }
}
